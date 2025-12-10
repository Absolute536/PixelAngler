using System;
using System.Reflection.Metadata.Ecma335;
using GamePlayer;
using Godot;
using SignalBusNS;

public partial class Bobber : Area2D
{
	[Export] Player Player;
	[Export] public FishingLine FishingLine;

	// OK. I know what's going on. Because of the difference in coordinate system (y axis positive on the usual is negative here)
	// So, instead of doing this hacky thing, we just need to negate the Endpoint y?
	// Nope, we need to do this
	[Export] public float SetLaunchAngle;
	[Export] public CollisionShape2D BobberCollisionShape;
	private float LaunchAngle
	{
		set
		{
			if (value != 0)
				_launchAngle = -Mathf.DegToRad(value); // Negate if zero, so that +ve is clockwise from first quadrant and -ve is anticlockwise
			else
				_launchAngle = value;
		}

		get
		{
			return _launchAngle;
		}
	}
	
	// Bobber Projectile Motion Parameters
	private float _launchAngle;
	private float _sinLaunchAngle;
	private float _cosineLaunchAngle;
	private float _negativeSinLaunchAngle;
	private float _negativeCosineLaunchAngle;
	private double _initialVelocity;
	private double _gravity;
	private const double TimeLimit = 0.5; // Must reach the _endPosition at TimeLimit (seconds)
	private double _timeElapsed = 0;
	private Vector2 _endPosition;
	private Vector2 _startPosition;

	// Internal Flags

	// Indicate if the bobber should land in water
	// This one is a bit hacky, cuz we use it to decide whether to hide early or not
	private bool _inWater = false;
	public bool InWater
    {
		set => _inWater = value;
		get => _inWater;
    }
	private bool _hasStopped = false; // Boolean flag to determine if the bobber has stopped (hmm ~)
	private bool _reverseMotion = false; // Indicate if we are in reverse motion;

	public bool IsLatchedOn = false; // A boolean flag to indicate if a fish has latched on (Kinda Fragile and risky, but let's test it first)

	[Export] public GpuParticles2D waterSplash;
	public override void _Ready()
	{
		// Setup Bobber Properties
		Visible = false;

		// Turn off Processes Callback on load
		SetProcess(false);
		SetPhysicsProcess(false);
	}

	/*
	 * Calculate bobber properties to ensure it will land at the specified _endPosition within the TimeLimit
	 * The formula is derived from the displacement formula for projectile motion.
	 * With the pre-determined _endPosition & TimeLimit parameters, we can calculate the _initialVelocity and _gravity parameters
	 * for determining the displacement of the bobber from the _startPosition per Physics Frame and adjust its position accordingly.
	 *
	 * Full Formula:
	 * initialVelocity = EndPoint.X / TimeLimit * (1 / cosineLaunchAngle);
	 * gravity = 2 * (initialVelocity * TimeLimit * sinLaunchAngle - EndPoint.Y) / (TimeLimit * TimeLimit);
	 *
	 * Simplified version if launch angle is 0:
	 * initialVelocity = EndPoint.X / TimeLimit;
	 * gravity = -2 * EndPoint.Y / (TimeLimit * TimeLimit);
	 *
	 * RIP my Physics class
	 * Reference: https://phys.libretexts.org/Bookshelves/University_Physics/Physics_(Boundless)/3%3A_Two-Dimensional_Kinematics/3.3%3A_Projectile_Motion
	 */
	public void StartBobberMotion(Vector2 endPosition)
	{
		// Specify end position of the bobber (where it should land)
		_endPosition = endPosition;
		// _inWater = inWater;
		Visible = true;

		// Calculation of _initialVelocity and _gravity parameters of the motion
		// IMPORTANT: displacement = end - start
		// Before this, we just used the end point, which is not actually correct.
		// It worked because the starting point is (0, 0) - the origin

		// Negate launch angle when facing left, because the formula is for launching towards the right (positive x)
		// For up and down directions, there are no differences, cuz the velocity would be 0, only the gravity component matters
		if (Player.FacingDirection == Vector2.Left)
			LaunchAngle = -SetLaunchAngle;
		else
			LaunchAngle = SetLaunchAngle;

		// Precompute the Sin and Cos values of the launch angle on start (cuz more expensive), instead of per Physics Frame
		// Previously, it was in _Ready(), but then that was pretty inflexible
		_sinLaunchAngle = Mathf.Sin(_launchAngle);
		_cosineLaunchAngle = Mathf.Cos(_launchAngle);
		_negativeSinLaunchAngle = Mathf.Sin(-_launchAngle);
		_negativeCosineLaunchAngle = Mathf.Cos(-_launchAngle);

		_initialVelocity = (_endPosition.X - _startPosition.X) / TimeLimit * (1 / _cosineLaunchAngle);
		_gravity = 2 * (_initialVelocity * TimeLimit * _sinLaunchAngle - (_endPosition.Y - _startPosition.Y)) / (TimeLimit * TimeLimit);

		// Start Physics Process to initiate the bobber's motion
		SetPhysicsProcess(true);

		waterSplash.Emitting = false;
		// Initiate the fishing line physics as well
		FishingLine.InitiateFishingLine();
	}

	// Initiate reverse motion by recalculating all the motion parameters and resetting some flags
	public void ReverseBobberMotion()
	{
		// Swap the starting and ending position
		// new start position should be the current position
		(_endPosition, _startPosition) = (_startPosition, GlobalPosition);
		
		// Recalculate parameters using the negative angle (values)
        _initialVelocity = (_endPosition.X - _startPosition.X) / TimeLimit * (1 / _negativeCosineLaunchAngle);
		_gravity = 2 * (_initialVelocity * TimeLimit * _negativeSinLaunchAngle - (_endPosition.Y - _startPosition.Y)) / (TimeLimit * TimeLimit);

		_timeElapsed = 0;
		_hasStopped = false;
		_reverseMotion = true; // Flip the flag to indicate we're in reverse motion
		Visible = true;

		SetPhysicsProcess(true);
    }

	// Function to reset the bobber's status (occurs on Action click)
	public void ResetBobberStatus()
	{
		Visible = false;

		_startPosition = Player.GlobalPosition + new Vector2(0, -16); // starting position is the player's position + offset of 16 pixels upwards
		GlobalPosition = _startPosition;

		_timeElapsed = 0;
		_hasStopped = false;
		_inWater = false; // reset to false you idiot!!!!
		_reverseMotion = false;

		waterSplash.Emitting = false;

		// try disabling the collision shape
		BobberCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	// PhysicsProcess to start the parabolic motion of the bobber
	public override void _PhysicsProcess(double delta)
	{
		/*
		 * Summary of what's been happenning
		 * We sample _timeElapsed based on delta
		 * So it is incremented by 0.016666666...7 something like that
		 * Initially, I thought the start and end position may not converge because of that, but it turns out ok
		 *
		 * Then, there are some discrepancy in the Vectors of the start and end positions when checking for the stop condition
		 * Especially during the reverse motion
		 * Actually there are some, like 0.00000x discrepancy on the forward motion as well, but we're using IsEqualApprox, so there's no problem
		 * But on the reverse motion, the discrepancy increased, like +- 0.000002/3 differences
		 * I don't know how it happened, it is because of the float imprecision or rounding error of the calculations?
		 * Fortunately, we are able to clutch up by checking for the rounded position instead
		 * Since we are sampling based on delta, and each position won't collide (as in the difference is >= 1 in integer)
		 * So, rounding does work
		 * HOWEVER, it feels kinda wrong. (Idk if this will come back and bite me later, but for now it works)
		 */
		_timeElapsed += delta;

		// If current (global) position is not at the end position AND the bobber hasn't stopped, continue the motion
		if (!GlobalPosition.Round().IsEqualApprox(_endPosition.Round()) && !_hasStopped)
		{
			Vector2 Displacement = CalculateBobberDisplacement(_timeElapsed);

			// Set the (Global) Position per Physics Frame as the starting position + displacement vector
			GlobalPosition = _startPosition + Displacement;
			GD.Print("Bobber Global Position: " + GlobalPosition);
		}
		else
		{
			_hasStopped = true;

			// Stop the Physics Proces to stop the bobber motion (more towards to save CPU cycle?)
			// Might need to remove this later, if we want to make it move? (Or... we can just adjust the position?)
			SetPhysicsProcess(false);

			// If bobber is casted not into water, hide both the bobber and fishing line back
			if (!_inWater)
			{
				// Visible = false;
				ResetBobberStatus();
				FishingLine.TerminateFishingLine();
				GD.Print("Bobber not landed in Water. Hide it back.");
			}


			// // Raise the corresponding BobberMotionEnded event if it is the end of the reverse motion
			if (_reverseMotion)
			{
				SignalBus.Instance.OnReverseBobberMotionEnded(this, EventArgs.Empty);
				Visible = false;
				FishingLine.TerminateFishingLine();
				BobberCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			}
			else // For forward motion
            {
				SignalBus.Instance.OnForwardBobberMotionEnded(this, EventArgs.Empty);
				
				if (_inWater)
                {
					// GD.Print("Print if landed in water");
                    BobberCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, false); // it's the area2d of bobber actually, not the collsion(wait?)
					// yeah, just disabling the collision won't stop fish from detecting
					waterSplash.Emitting = true; // need to reset to false afterwards to ensure it emits once only
                }
					
            }


		}

		// FOR DEBUG PURPOSES
		if (GlobalPosition.Round().IsEqualApprox(_endPosition.Round()))
			GD.Print("Bobber " + GlobalPosition + " overlapped with end position: " + _endPosition);
		else
			GD.Print("Bobber " + GlobalPosition + " not overlapped with end position: " + _endPosition);
	}
	
	private Vector2 CalculateBobberDisplacement(double time)
    {
		float displacementX = (float)CalculateDisplacementX(time);
		float displacementY = (float)CalculateDisplacementY(time);

		return new Vector2(displacementX, displacementY);
    }

	private double CalculateDisplacementX(double time)
	{
		if (_reverseMotion)
			return _initialVelocity * time * _negativeCosineLaunchAngle;

		return _initialVelocity * time * _cosineLaunchAngle;
	}

	private double CalculateDisplacementY(double time)
	{
		if (_reverseMotion)
			return _initialVelocity * time * _negativeSinLaunchAngle - (0.5 * _gravity * time * time);

		return _initialVelocity * time * _sinLaunchAngle - (0.5 * _gravity * time * time);
	}
}
