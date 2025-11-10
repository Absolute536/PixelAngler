using Godot;
using System;

public partial class Testing : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Vector2 end = new(0.33333335f, 134);
		Vector2 start = new(-62.199997f, 133.98222f);

		double initialVelocity = (end.X - start.X) / 0.5 * (1 / Mathf.Cos(45));
		double gravity = 2 * (initialVelocity * 0.5 * Mathf.Sin(45) - (end.Y - start.Y)) / 0.25;
		GD.Print("Velocity: " + initialVelocity);
		GD.Print("Gravity: " + gravity);

		Vector2 temp = end;
		end = start;
		start = temp;

		initialVelocity = (end.X - start.X) / 0.5 * (1 / Mathf.Cos(45));
		gravity = 2 * (initialVelocity * 0.5 * Mathf.Sin(45) - (end.Y - start.Y)) / 0.25;
		GD.Print("Velocity: " + initialVelocity);
		GD.Print("Gravity: " + gravity);

		Vector2 p1 = new Vector2(0, -19);
		Vector2 p2 = new Vector2(10, -8);
		GD.Print(Mathf.RadToDeg(p1.AngleToPoint(p2)));

	}

    public override void _UnhandledInput(InputEvent @event)
    {
		// base._UnhandledInput(@event);
		// GD.Print("1 TICK.");
		// GD.Print("Event is pressed: " + @event.IsPressed()); // true if key press (because the callback is only invoked once per key press / sample period)
		// GD.Print("Event is released: " + @event.IsReleased());
		// GD.Print("Event is action: " + @event.IsAction("Left")); // true cuz the callback argument is the specified action
		// GD.Print("Is action pressed: " + (@event.IsActionPressed("Left") && @event.IsActionPressed("Up")));
		// GD.Print("Is action released: " + @event.IsActionReleased("Left"));
		// Ok, so if I don't allow echo, IsActionPressed will return true for only the "frame" / "tick" when I pressed the action key
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Action"))
        {
			GD.Print("LMB detected in _Process()");
        }
	}
	

	
}
