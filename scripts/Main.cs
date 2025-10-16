using GameWorld;
using Godot;
using System;

public partial class Main : Node2D
{
	[Export] Label FPSCount;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		float refreshRate = DisplayServer.ScreenGetRefreshRate();
		if (refreshRate < 0)
			Engine.MaxFps = 120;
		else
			Engine.MaxFps = (int) refreshRate; // + 60 or not?
			// Engine.MaxFps = 60;
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		FPSCount.Text = Engine.GetFramesPerSecond().ToString();
	}
}
