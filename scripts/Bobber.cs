using Godot;
using System;

public partial class Bobber : Sprite2D
{
	[Export] public Vector2 EndPoint;

	// OK. I know what's going on. Because of the difference in coordinate system (y axis positive on the usual is negative here)
	// So, instead of doing this hacky thing, we just need to negate the Endpoint y?
	// Nope, we need to do this
	[Export]
	public float LaunchAngle
	{
		set
		{
			if (value != 0)
				_launchAngle = -Mathf.DegToRad(value); // Negate if zero
			else
				_launchAngle = value;
		}
		get
		{
			return _launchAngle;
		}

	}
	private float _launchAngle = 0; // Launch angle is 0 because it starts horizonally from the starting point (only cos 0 = 1)
	private double initialVelocity;
	private double gravity;
	public const double TimeLimit = 0.3; // At 0.5 second, must reach the specified end point
	private double TimeElapsed = 0;
	private bool HasStopped = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Calculate initialVelocity and gravity parameters to ensure bobber will land at the specified EndPoint within the TimeLimit

		// Simplified version for zero launch angle
		// initialVelocity = EndPoint.X / TimeLimit;
		// gravity = -2 * EndPoint.Y / (TimeLimit * TimeLimit);
		Visible = false;
		TopLevel = true;
		// Actual formula
		initialVelocity = EndPoint.X / TimeLimit * (1 / Mathf.Cos(_launchAngle));
		gravity = 2 * (initialVelocity * TimeLimit * Mathf.Sin(_launchAngle) - EndPoint.Y) / (TimeLimit * TimeLimit);
	}

	public void InitiateBobberAction(Vector2 startPosition)
	{
		GlobalPosition = startPosition;
		SetPhysicsProcess(true);
		initialVelocity = EndPoint.X / TimeLimit * (1 / Mathf.Cos(_launchAngle));
		gravity = 2 * (initialVelocity * TimeLimit * Mathf.Sin(_launchAngle) - EndPoint.Y) / (TimeLimit * TimeLimit);
    }

	public static Bobber GetBobberInstance(Vector2 endPoint)
	{
		PackedScene bobberScene = ResourceLoader.Load<PackedScene>("res://scenes/bobber.tscn");
		if (bobberScene is not null)
        {
			Bobber bobber = bobberScene.Instantiate() as Bobber;
			bobber.Name = "Bobber";
			bobber.EndPoint = endPoint;
			bobber.TopLevel = true;
			return bobber;
        }

		return null;
	}
	
	public static Bobber GetBobberInstance(float x, float y)
    {
		return GetBobberInstance(new Vector2(x, y));
    }

	/*
	 * OK. Let's set down the parameters we know and what we need to find
	 * - initial velocity [known] -> fixed velocity
	 * - gravity [known] -> also a constant +9.8 (positive cuz +ve y is downwards)
	 * - launch angle [known] -> also fixed as 45 degree (test first, then adjust later)
	 * - displacement [unknwon] -> we need to calculate this to identify the bobber's position at anytime
	 *
	 * First, let's try to make the bobber go right, regardless of the casting direction, just to test things out
	 */

	public override void _PhysicsProcess(double delta)
	{
		// Sample based on delta
		// if (Position.X < endPoint.X && Position.Y < endPoint.Y)
		// actually, it probably won't cuz delta is 1 /60 = 0.0166 (won't coincide with the time limit)
		TimeElapsed += delta;

		if (GlobalPosition != EndPoint && !HasStopped) 
		{
			Vector2 positionDuringMotion = Vector2.Zero;
			float displacementX = (float)CalculateDisplacementX(TimeElapsed);
			float displacementY = (float)CalculateDisplacementY(TimeElapsed);

			positionDuringMotion.X = displacementX;
			// positionDuringMotion.Y = Mathf.Tan(LaunchAngle) * displacementX - (-100 / (2 * InitialVelocityMagnitude * InitialVelocityMagnitude * Mathf.Cos(LaunchAngle) * Mathf.Cos(LaunchAngle)) * displacementX * displacementX);
			positionDuringMotion.Y = displacementY;

			GlobalPosition = positionDuringMotion;
			GD.Print(Position);
		}
		else
			HasStopped = true;

		// Oh yeah, also need to include a stopping condition.
		// Else, if we move, the bobber's GlobalPosition will move and cause it to fall down (physics process still running)
	}

	public double CalculateDisplacementX(double time)
	{
		return initialVelocity * time * Mathf.Cos(_launchAngle);
	}

	public double CalculateDisplacementY(double time)
	{
		return initialVelocity * time * Mathf.Sin(_launchAngle) - (0.5 * gravity * time * time);
	}
	// Ok I think this works, just need more fine tuning to decide the gravity vector and cleaning up
	// Also need to implement signals
}
