using GamePlayer;
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
	// to record sin and cos of launch angle instead of invoking the method every time because sin and cos operations are more expensive
	private float sinLaunchAngle;
	private float cosineLaunchAngle;
	private double initialVelocity;
	private double gravity;
	public const double TimeLimit = 0.3; // At 0.5 second, must reach the specified end point
	private double TimeElapsed = 0;
	private bool HasStopped = false;

	private Player player;

	private Vector2 startPoint;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("Player") as Player;
		GD.Print("Player Global Position in Bobber: " + player.GlobalPosition);

		
		Visible = false;
		TopLevel = true;
		sinLaunchAngle = Mathf.Sin(_launchAngle);
		cosineLaunchAngle = Mathf.Cos(_launchAngle);
		SetProcess(false);
		SetPhysicsProcess(false); // turn off physics process first on load

		// Calculate initialVelocity and gravity parameters to ensure bobber will land at the specified EndPoint within the TimeLimit

		// Actual formula
		// initialVelocity = EndPoint.X / TimeLimit * (1 / cosineLaunchAngle);
		// gravity = 2 * (initialVelocity * TimeLimit * sinLaunchAngle - EndPoint.Y) / (TimeLimit * TimeLimit);

		// Simplified version if launch angle is 0
		// initialVelocity = EndPoint.X / TimeLimit;
		// gravity = -2 * EndPoint.Y / (TimeLimit * TimeLimit);
		
	}

	/*
	 * What we need now
	 * - an initialiser method (to reset the bobber to initial state) --> can just put them inside the else block of physicsProcess (can extract out later)
	 * - a starting method (to trigger the boobber action)
	 */



	public void InitiateBobberAction(Vector2 endPosition)
	{
		// GlobalPosition = player.GlobalPosition; // idk how this will work
		EndPoint = endPosition;
		Visible = true;
		// TimeElapsed = 0;

		// don't need to calculate cuz it's already fixed on _Ready() (NOPE!!!!) -> need to cuz end point changes every time
		// UNLESS, we want to be able to change it during runtime

		// Important: displacement = end - start
		// Before this, we just used the end point, because the starting point is (0, 0) - the origin
		// It worked before, cuz the end point is the displacement
		initialVelocity = (EndPoint.X - startPoint.X) / TimeLimit * (1 / cosineLaunchAngle);
		gravity = 2 * (initialVelocity * TimeLimit * sinLaunchAngle - (EndPoint.Y - startPoint.Y)) / (TimeLimit * TimeLimit);
		SetPhysicsProcess(true);
		// GD.Print("Initiate Bobber Position: " + GlobalPosition);
	}
	
	public void ResetBobber()
    {
		startPoint = player.GlobalPosition + new Vector2(0, -16);
		GlobalPosition = startPoint;
		// GD.Print("Reset Bobber: " + GlobalPosition);
		// GD.Print("Reset Bobber Player: " + player.GlobalPosition);
		TimeElapsed = 0;
		HasStopped = false;
    }

	// Don't know if we still need the BobberInstance method
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
	 * Neh, doesn't matter, cuz it depends on where the end point is, and the (initial) velocity & gravity will be calculated accordingly
	 */


	// PhysicsProcess to start the parabolic motion of the bobber
	public override void _PhysicsProcess(double delta)
	{
		// Sample based on delta
		// if (Position.X < endPoint.X && Position.Y < endPoint.Y)
		// actually, it probably won't cuz delta is 1 /60 = 0.0166 (won't coincide with the time limit)
		TimeElapsed += delta;

		if (GlobalPosition != EndPoint && !HasStopped)
		{
			Vector2 Displacement = Vector2.Zero;
			float displacementX = (float)CalculateDisplacementX(TimeElapsed);
			float displacementY = (float)CalculateDisplacementY(TimeElapsed);

			Displacement.X = displacementX;
			Displacement.Y = displacementY;

			GlobalPosition = startPoint + Displacement;
			GD.Print("Global Position during: " + GlobalPosition);
			GD.Print("Position during: " + Position);
		}
		else
		{
			// well actually, I can't put it here because even if it lands on water, still gonna be reset

			// Visible = false;
			// GlobalPosition = player.GlobalPosition;
			HasStopped = true;
			// TimeElapsed = 0; // rest time elapsed tracking variable as well
			SetPhysicsProcess(false);
		}
		

		// Oh yeah, also need to include a stopping condition.
		// Else, if we move, the bobber's GlobalPosition will move and cause it to fall down (physics process still running)
	}

	public double CalculateDisplacementX(double time)
	{
		return initialVelocity * time * cosineLaunchAngle;
	}

	public double CalculateDisplacementY(double time)
	{
		return initialVelocity * time * sinLaunchAngle - (0.5 * gravity * time * time);
	}
	// Ok I think this works, just need more fine tuning to decide the gravity vector and cleaning up
	// Also need to implement signals
}
