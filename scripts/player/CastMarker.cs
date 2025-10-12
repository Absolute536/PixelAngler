using Godot;
using System;
using GamePlayer;
using GameWorld;
using SignalBusNS;

public partial class CastMarker : Sprite2D
{	
	// Since CastMarker is a part of Player (Player has-a CastMarker, we make it an exported property instead)
	[Export] public Player TargetPlayer;
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
		// Connect to action manager's cast action (Or can we try putting these in the SignalBus instead?)
		// ActionManager.CastActionStart = CastMarkingStart;
		SignalBus.Instance.CastActionStart += HandleCastActionStart;
		SignalBus.Instance.CastActionEnd += HandleCastActionEnd;
		// ActionManager.CastActionEnd = CastMarkingEnd;

		// So upon joining the scene tree, connect to Timeout signal
		castTimer.Timeout += CastMarkingProcess;

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
	private void CastMarkingStart()
	{
		castTimer.Start();

		castDirection = TargetPlayer.FacingDirection;
		initialPosition = TargetPlayer.Position + (castDirection * new Vector2(16, 16) * 2); // initial position 3 tiles away from the facing direction of the player
		Position = initialPosition;
		Visible = true;
	}

	protected virtual void HandleCastActionStart(object sender, EventArgs e)
	{
		castTimer.Start();

		castDirection = TargetPlayer.FacingDirection;
		initialPosition = TargetPlayer.Position + (castDirection * new Vector2(16, 16) * 2); // initial position 3 tiles away from the facing direction of the player
		Position = initialPosition;
		Visible = true;
	}
	
	protected virtual void HandleCastActionEnd(object sender, EventArgs e)
    {
        castLength = 0; // reset cast length
		castTimer.Stop();
		Visible = false;
		// Record position first
		// Find out if the end position is on water
		// if it is, commence the casting animation
		// else, do nothing (maybe pop up message)

		Vector2 endPosition = Position;
		
		TileType tileType = GameInfo.Instance.GetTileType(endPosition);

		if (tileType != TileType.Water)
        {
			Label message = new Label()
			{
				Text = "Bobber not casted in water.",
				Size = new Vector2(50, 50),
				Visible = true
			};

			TargetPlayer.AddChild(message);
        }	
		GD.Print("Marker landed on " + tileType.ToString());
		Position = TargetPlayer.Position; // reset position back to origin of parent (Player node)
    }

	private void CastMarkingEnd()
	{
		castLength = 0; // reset cast length
		castTimer.Stop();
		Visible = false;
		// Record position first
		// Find out if the end position is on water
		// if it is, commence the casting animation
		// else, do nothing (maybe pop up message)

		Vector2 endPosition = Position;
		
		TileType tileType = GameInfo.Instance.GetTileType(endPosition);

		if (tileType != TileType.Water)
        {
			Label message = new Label()
			{
				Text = "Bobber not casted in water.",
				Size = new Vector2(50, 50),
				Visible = true
			};

			TargetPlayer.AddChild(message);
        }	
		GD.Print("Marker landed on " + tileType.ToString());
		Position = TargetPlayer.Position; // reset position back to origin of parent (Player node)
	}

	private void CastMarkingProcess()
    {
		if (TargetPlayer != null && castLength < MaxCastingLength)
		{
			castLength += 16; // 16 pixels per 0.5s (1 tile)
							  // Right. Cuz I compound the castLength per call, so the marker increases more and more (if not using assignment)
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
		
		// Console Output
		GD.Print("Direction: " + TargetPlayer.FacingDirection);
		GD.Print("Player: " + TargetPlayer.Position);
		GD.Print("Marker: " + Position);
    }

}
