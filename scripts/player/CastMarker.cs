using Godot;
using System;
using GamePlayer;

public partial class CastMarker : Sprite2D
{
	private Player player;
	[Export] public Timer castTimer;

	private const int MaxLength = 80;
	private int castLength = 0;

	public CastMarker()
	{
		Visible = false; // hide it on instantiation (before entering scene tree & _Ready I believe)
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("Player") as Player;
		GD.Print("Ready is called when entering scene tree"); // So it works

		GlobalPosition += player.FacingDirection * new Vector2(16, 16);

		// So upon joining the scene tree, connect to Timeout signal and start the timer
		castTimer.Timeout += CastingMarker;
		castTimer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionPressed("Action"))
		{
			CastingMarker();
		}

		if (Input.IsActionJustReleased("Action"))
		{
			GlobalPosition = player.GlobalPosition + player.FacingDirection * new Vector2(16, 16);
			Visible = false;
			castLength = 0;
			castTimer.Stop();
		}
	}

	private void CastingMarker()
	{
		if (player != null && castLength < MaxLength)
		{
			castLength += 16; // 16 pixels per 0.5s (1 tile)

			if (player.FacingDirection == Vector2.Up)
				GlobalPosition = player.GlobalPosition.Round() + new Vector2(0, -castLength);
			else if (player.FacingDirection == Vector2.Down)
				GlobalPosition = player.GlobalPosition.Round() + new Vector2(0, castLength);
			else if (player.FacingDirection == Vector2.Right)
				GlobalPosition = player.GlobalPosition.Round() + new Vector2(castLength, 0);
			else
				GlobalPosition = player.GlobalPosition.Round() + new Vector2(-castLength, 0);

			GlobalPosition += player.FacingDirection * new Vector2(16, 16);
		}
		GD.Print("Direction: " + player.FacingDirection);
		GD.Print("Player: " + player.Position);
		GD.Print("Marker: " + Position);
	}


}
