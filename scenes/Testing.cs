using Godot;
using System;

public partial class Testing : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Vector2 v = new Vector2(0, 1);
		// GD.Print("Normalised: " + v.Normalized());
		// v = new Vector2(1, 1);
		// GD.Print("Normalised Again: " + v.Normalized());

	}

    public override void _UnhandledInput(InputEvent @event)
    {
		base._UnhandledInput(@event);
		GD.Print("1 TICK.");
		GD.Print("Event is pressed: " + @event.IsPressed()); // true if key press (because the callback is only invoked once per key press / sample period)
		GD.Print("Event is released: " + @event.IsReleased());
		GD.Print("Event is action: " + @event.IsAction("Left")); // true cuz the callback argument is the specified action
		GD.Print("Is action pressed: " + (@event.IsActionPressed("Left") && @event.IsActionPressed("Up")));
		GD.Print("Is action released: " + @event.IsActionReleased("Left"));
		// Ok, so if I don't allow echo, IsActionPressed will return true for only the "frame" / "tick" when I pressed the action key
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
