using Godot;
using System;

public partial class TestProgress : ProgressBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        Control test = GD.Load<PackedScene>("res://scenes/fishing_progress.tscn").Instantiate<Control>();
        AddChild(test);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    // Called every physics frame. 'delta' is the elapsed time since the previous physics frame.
    public override void _PhysicsProcess(double delta)
    {
        
    }

}
