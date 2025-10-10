using Godot;
using System;
using GamePlayer;

public partial class CastMarker : Sprite2D
{
	private Player TargetPlayer;
	[Export] public Timer castTimer;
	[Export] public PlayerActionManager ActionManager;

	[Export] public int MaxCastingLength = 80;

	private Vector2 castDirection;
	private Vector2 initialPosition;
	private int castLength = 0;

	public CastMarker()
	{
		Visible = false; // hide it on instantiation (before entering scene tree & _Ready I believe)
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ActionManager.CastActionStart = CastingStart;
		ActionManager.CastActionEnd = CastingEnd;

		TargetPlayer = GetTree().GetFirstNodeInGroup("Player") as Player;
		GD.Print("Ready is called when entering scene tree"); // So it works

		// GlobalPosition += TargetPlayer.FacingDirection * new Vector2(16, 16);

		// So upon joining the scene tree, connect to Timeout signal and start the timer
		castTimer.Timeout += CastingProcess;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		// if (Input.IsActionPressed("Action"))
		// {
		// 	castTimer.Start(); 
		// 	CastingMarker();
		// }

		// if (Input.IsActionJustReleased("Action"))
		// {
		// 	GlobalPosition = player.GlobalPosition + player.FacingDirection * new Vector2(16, 16);
		// 	Visible = false;
		// 	castLength = 0;
		// 	castTimer.Stop();
		// }
	}
	private void CastingStart()
	{
		castTimer.Start();
		
		castDirection = TargetPlayer.FacingDirection;
		initialPosition = TargetPlayer.Position + (castDirection * new Vector2(16, 16) * 2); // initial position 3 tiles away from the facing direction of the player
		Position = initialPosition;
		Visible = true;
	}

	private void CastingEnd()
	{
		castLength = 0; // reset cast length
		castTimer.Stop();
		Visible = false;
		Position = TargetPlayer.Position; // reset position back to origin of parent (Player node)
	}

	private void CastingProcess()
    {
        if (TargetPlayer != null && castLength < MaxCastingLength)
		{
			castLength += 16; // 16 pixels per 0.5s (1 tile)
			// Right. Cuz I compound the castLenght per call, so the marker extension increases
			// Need to start from origin

			if (castDirection == Vector2.Up)
				Position = initialPosition + new Vector2(0, -castLength);
			else if (castDirection == Vector2.Down)
				Position = initialPosition + new Vector2(0, castLength);
			else if (castDirection == Vector2.Right)
				Position = initialPosition + new Vector2(castLength, 0);
			else
				Position = initialPosition + new Vector2(-castLength, 0);

			// Position += castDirection * new Vector2(16, 16);
		}
		GD.Print("Direction: " + TargetPlayer.FacingDirection);
		GD.Print("Player: " + TargetPlayer.Position);
		GD.Print("Marker: " + Position);
    }

}
