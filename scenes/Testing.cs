using Godot;
using System;

public partial class Testing : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Vector2 v = new Vector2(0, 1);
		GD.Print("Normalised: " + v.Normalized());
		v = new Vector2(1, 1);
		GD.Print("Normalised Again: " + v.Normalized());

	}

    public override void _UnhandledInput(InputEvent @event)
    {
		base._UnhandledInput(@event);
		GD.Print(@event.AsText());
    }


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
