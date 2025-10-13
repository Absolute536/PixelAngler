using Godot;
using System;

public partial class Bobber : Sprite2D
{
	public Vector2 Velocity = Vector2.Zero;
	public int InitialVelocityMagnitude = 200; // let's try 20 first
	public float LaunchAngle = Mathf.DegToRad(0); // Launch angle is 0 because it starts horizonally from the starting point (only cos 0 = 1)

	public const float TimeLimit = 1.5f; // 1 second from start point to end point?
	private double TimeTaken = 0;
	private Vector2 startPoint = new Vector2(0, -32); // Not really (start point is 0, 0)
	private Vector2 endPoint = new Vector2(100, 100);
	private int range = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
		TimeTaken += delta;
		// if (Position.X < endPoint.X && Position.Y < endPoint.Y)
		if (TimeTaken < TimeLimit)
		{
			Vector2 currentPosition = Vector2.Zero;
			float displacementX = (float)CalculateDisplacementX(TimeTaken);
			float displacementY = (float)CalculateDisplacementY(TimeTaken);
			
			currentPosition.X = displacementX;
			currentPosition.Y = Mathf.Tan(LaunchAngle) * displacementX - (-100 / (2 * InitialVelocityMagnitude * InitialVelocityMagnitude * Mathf.Cos(LaunchAngle) * Mathf.Cos(LaunchAngle)) * displacementX * displacementX);
			// currentPosition.Y = displacementY;

			Position = currentPosition;
			GD.Print(Position);
        }
	}

	public void SetPoints(Vector2 start, Vector2 end)
	{
		startPoint = start;
		endPoint = end;

	}

	public double CalculateDisplacementX(double time)
	{
		return InitialVelocityMagnitude * time * Mathf.Cos(LaunchAngle);
	}

	public double CalculateDisplacementY(double time)
	{
		return InitialVelocityMagnitude * time * Mathf.Sin(LaunchAngle) - (0.5 * -100 * time * time);
	}
	// Ok I think this works, just need more fine tuning to decide the gravity vector and cleaning up
	// Also need to implement signals
}
