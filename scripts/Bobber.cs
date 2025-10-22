using GamePlayer;
using Godot;
using System;
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
	private const double TimeLimit = 0.3; // Must reach the _endPosition at TimeLimit (seconds)
	private bool _inWater = true; // This one is a bit hacky, an extra flag to indicate whether to hide early or not

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
	 * I forgor my Physics class content already RIP
	 * Reference: https://phys.libretexts.org/Bookshelves/University_Physics/Physics_(Boundless)/3%3A_Two-Dimensional_Kinematics/3.3%3A_Projectile_Motion
	 */
	public void StartBobberMotion(Vector2 endPosition, bool inWater)
	{
		// Specify end position of the bobber (where it should land)
		_endPosition = endPosition;
		_inWater = inWater;
		Visible = true;

		// Calculation of _initialVelocity and _gravity parameters of the motion
		// IMPORTANT: displacement = end - start
		// Before this, we just used the end point, which is not actually correct.
		// It worked because the starting point is (0, 0) - the origin 
		_initialVelocity = (_endPosition.X - _startPosition.X) / TimeLimit * (1 / _cosineLaunchAngle);
		_gravity = 2 * (_initialVelocity * TimeLimit * _sinLaunchAngle - (_endPosition.Y - _startPosition.Y)) / (TimeLimit * TimeLimit);

		// Start Physics Process to initiate the bobber's motion
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
		_timeElapsed += delta;

		if (GlobalPosition != _endPosition && !_hasStopped)
		{
			Vector2 Displacement = Vector2.Zero;
			float displacementX = (float)CalculateDisplacementX(_timeElapsed);
			float displacementY = (float)CalculateDisplacementY(_timeElapsed);

			Displacement.X = displacementX;
			Displacement.Y = displacementY;

			// Set the (Global) Position per Physics Frame as the starting position + displacement vector
			GlobalPosition = _startPosition + Displacement;
			GD.Print("Bobber Global Position: " + GlobalPosition);
		}
		else
		{
			_hasStopped = true;
			SetPhysicsProcess(false); // Stop the Physics Proces to stop the bobber motion (more towards to save CPU cycle?)
			FishingLine.StartDraw(Position);
			if (!_inWater)
			{
				Visible = false;
				GD.Print("Bobber not landed in Water. Hide it back.");
			}
		}
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
