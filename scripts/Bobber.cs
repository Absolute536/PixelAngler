using GamePlayer;
using Godot;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

public partial class Bobber : Sprite2D
{
	// OK. I know what's going on. Because of the difference in coordinate system (y axis positive on the usual is negative here)
	// So, instead of doing this hacky thing, we just need to negate the Endpoint y?
	// Nope, we need to do this
	[Export]
	public float LaunchAngle
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
	private float _launchAngle = 0; // Launch angle is 0 because it starts horizonally from the starting point (only cos 0 = 1)
	private float _sinLaunchAngle;
	private float _cosineLaunchAngle;
	private double _initialVelocity;
	private double _gravity;
	private double _timeElapsed = 0;
	private bool _hasStopped = false; // Boolean flag to determine if the bobber has stopped (hmm ~)
	private Vector2 _endPosition;
	private Vector2 _startPosition;
	private Player _player;
	private const double TimeLimit = 0.5; // Must reach the _endPosition at TimeLimit (seconds)
	private bool _inWater = true; // This one is a bit hacky, an extra flag to indicate whether to hide early or not
	public bool InWater
    {
		get => _inWater;
    }
	[Export] public FishingLine FishingLine;

	public override void _Ready()
	{
		// Initialise _player
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;

		// Setup Bobber Properties
		Visible = false;
		// TopLevel = true;
		// Compute the Sin and Cos values of the launch angle on setup (cuz more expensive), instead of per Physics Frame
		_sinLaunchAngle = Mathf.Sin(_launchAngle);
		_cosineLaunchAngle = Mathf.Cos(_launchAngle);

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
	public void StartBobberMotion(Vector2 endPosition, bool inWater)
	{
		// Specify end position of the bobber (where it should land)
		_endPosition = endPosition;
		_inWater = inWater;
		Visible = true;

		LaunchAngle = 0;
		_sinLaunchAngle = Mathf.Sin(_launchAngle);
		_cosineLaunchAngle = Mathf.Cos(_launchAngle);

		// Calculation of _initialVelocity and _gravity parameters of the motion
		// IMPORTANT: displacement = end - start
		// Before this, we just used the end point, which is not actually correct.
		// It worked because the starting point is (0, 0) - the origin 
		_initialVelocity = (_endPosition.X - _startPosition.X) / TimeLimit * (1 / _cosineLaunchAngle);
		_gravity = 2 * (_initialVelocity * TimeLimit * _sinLaunchAngle - (_endPosition.Y - _startPosition.Y)) / (TimeLimit * TimeLimit);

		// Start Physics Process to initiate the bobber's motion
		SetPhysicsProcess(true);
		FishingLine.InitiateFishingLine();
	}

	public void ReverseBobberMotion()
	{
		Vector2 tempStart = _startPosition;
		_startPosition = _endPosition;
		// _endPosition = _player.GlobalPosition + new Vector2(0, -16);
		_endPosition = tempStart;

		LaunchAngle = 0; // Ok, it probably have sth to do with the facing direction as well (O mean if we want to have some sort of angle)
		// Update: maybe facing direction is an issue, but it's more towards the floating point values when not using 0, 90, 180 angles
		// New Update: Ok, so if we use IsApproxEqual to compare the positions, it's ok for angle = 0. How about other angles
		// _sinLaunchAngle = Mathf.Sin(_launchAngle);
		// _cosineLaunchAngle = Mathf.Cos(_launchAngle);

		_initialVelocity = (_endPosition.X - _startPosition.X) / TimeLimit * (1 / _cosineLaunchAngle);
		_gravity = 2 * (_initialVelocity * TimeLimit * _sinLaunchAngle - (_endPosition.Y - _startPosition.Y)) / (TimeLimit * TimeLimit);

		_timeElapsed = 0;
		_hasStopped = false;
		Visible = true;
		SetPhysicsProcess(true);
    }

	// Function to reset the bobber's status (occurs on Action click)
	public void ResetBobberStatus()
	{
		Visible = false;
		_startPosition = _player.GlobalPosition + new Vector2(0, -16); // starting position is the player's position + offset of 16 pixels upwards
		GlobalPosition = _startPosition;
		_timeElapsed = 0;
		_hasStopped = false;
		_inWater = true;
	}

	// PhysicsProcess to start the parabolic motion of the bobber
	public override void _PhysicsProcess(double delta)
	{
		// Sample _timeElapsed based on delta
		// instead of delta, maybe we can try some other value?
		// Yep, cuz delta is 0.01666 sth, so for some values, it may not converge (in terms of the endPosition)
		// 60 per second, so + 0.6 per second
		// _timeElapsed += 0.05;
		// Nvm it's so unstable
		// I think it's because of floating point inaccuracies (might be)

		// NEVERMIND, the problem is only at the reversal path, so something needs tweaking there
		// yeah
		_timeElapsed += delta;

		if (!GlobalPosition.Snapped(0.00001f).IsEqualApprox(_endPosition.Snapped(0.00001f)) && !_hasStopped)
		{
			Vector2 Displacement = CalculateBobberDisplacement(_timeElapsed);

			// Set the (Global) Position per Physics Frame as the starting position + displacement vector
			GlobalPosition = _startPosition + Displacement;
			GD.Print("Bobber Global Position: " + GlobalPosition);
		}
		else
		{
			_hasStopped = true;
			SetPhysicsProcess(false); // Stop the Physics Proces to stop the bobber motion (more towards to save CPU cycle?)
									  // Might need to remove this later, if we want to make it move?

			if (!_inWater)
			{
				Visible = false;
				FishingLine.TerminateFishingLine();
				GD.Print("Bobber not landed in Water. Hide it back.");
			}
		}

		if (GlobalPosition.Snapped(0.00001f).IsEqualApprox(_endPosition.Snapped(0.00001f)))
			GD.Print("Bobber " + GlobalPosition.Snapped(0.00001f) + " overlapped with end position: " + _endPosition.Snapped(0.00001f));
		else
			GD.Print("Bobber " + GlobalPosition.Snapped(0.00001f) + " not overlapped with end position: " + _endPosition.Snapped(0.00001f));
	}
	
	private Vector2 CalculateBobberDisplacement(double time)
    {
		float displacementX = (float)CalculateDisplacementX(time);
		float displacementY = (float)CalculateDisplacementY(time);

		return new Vector2(displacementX, displacementY);
    }

	private double CalculateDisplacementX(double time)
	{
		return _initialVelocity * time * _cosineLaunchAngle;
	}

	private double CalculateDisplacementY(double time)
	{
		return _initialVelocity * time * _sinLaunchAngle - (0.5 * _gravity * time * time);
	}

}
