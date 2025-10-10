using Godot;
using System;

public partial class FishingLine : Line2D
{
	[Export] public Node2D TraceTarget; // End point of the line - the bobber
	[Export] public Vector2 AnchorPoint; // Anchor (First) point of the line - at the end of the fishing rod

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _Process(double delta)
    // {
    // }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

}
