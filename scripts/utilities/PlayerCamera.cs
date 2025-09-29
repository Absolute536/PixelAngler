using Godot;
using System;
using System.Security.Cryptography;

public partial class PlayerCamera : Camera2D
{
	[Export] public CharacterBody2D target;
	private Vector2 targetPosition = new Vector2();
	private const float speed = 25.0f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TopLevel = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		// Physics tick 60 so 1 / 60 per tick
		// fraction = Engine.GetPhysicsInterpolationFraction
		// Call every frame, but player's position is already there
		// Just that physics interpolation will draw "fake" frames in between

		// SO we use lerp smoothing for camera movement ding ding ding report

		// Godot Doc
		// Vector2 playerPosition = target.GlobalPosition;
		// float weight = 1.0f - Mathf.Exp(-speed * (float)delta);
		// weight = (1 - Mathf.Cos(Mathf.Pi * weight)) / 2;
		// GlobalPosition = GlobalPosition.Lerp(playerPosition, weight);

		/*
		 * Welp. Even with all the lerp smoothing approaches, it still can't adapt dynamically
		 * The main problem lies with the frame rate and refresh rate mismatch
		 * FPS vs. Physics Tick - Physics Interpolation solves it all
		 * FPS vs. Refresh Rate - Only works if FPS is higher than refresh rate, if it's lower, there's some jitter
		 */

		// Lerp Smoothing is Broken video
		GlobalPosition = lerpSmooth(GlobalPosition, target.GlobalPosition, (float)delta);

	}

	private Vector2 lerpSmooth(Vector2 from, Vector2 to, float delta)
	{
		return to + (from - to) * Mathf.Exp(-speed * delta);
	}
}
