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
		SignalBus.Instance.CastingStarted += HandleCastActionStart;
		// SignalBus.Instance.CastActionEnd += HandleCastActionEnd;
		SignalBus.Instance.SubscribeCastingEnded(this, HandleCastActionEnd);
		// ActionManager.CastActionEnd = CastMarkingEnd;

		// So upon joining the scene tree, connect to Timeout signal
		castTimer.Timeout += CastMarkingProcess;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{

	}

	private void HandleCastActionStart(object sender, EventArgs e)
	{
		Visible = true;
		castTimer.Start();

		// Initial testing: if fishing line exist, free it
		// if (TargetPlayer.GetChild(-1) is Line2D)
		// 	TargetPlayer.GetChild(-1).QueueFree();

		castDirection = TargetPlayer.FacingDirection;
		initialPosition = TargetPlayer.GlobalPosition + (castDirection * new Vector2(16, 16) * 2); // initial position 2 tiles away from the facing direction of the player
		GlobalPosition = initialPosition;
		Visible = true;
	}
	
	private TileType HandleCastActionEnd(object sender, EventArgs e)
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
			// Label message = new Label()
			// {
			// 	Text = "Bobber not casted in water.",
			// 	Size = new Vector2(50, 50),
			// 	Visible = true
			// };

			// TargetPlayer.AddChild(message);
		}
		else // Experimenting with fishing line (simple line2d)
		{
			// If landed on Water, instantiate the fishing line I guess?
			Line2D fishingLine = new Line2D()
			{
				Name = "FishingLine",
				Points = [new Vector2(0, -16), (endPosition - TargetPlayer.Position)], // start from (0, 0) instead of player's position, cuz you are a child of player, it will offset by the set position if you start from player's POS
				// end position - player position to get the difference between the two (in terms of global position? cuz local = global in this case)
				// then, we need to difference or else it will offest from the player by the specified vector
				Width = 1.0f,
				DefaultColor = Colors.White,
				Visible = true
			};
			Bobber bobber = Bobber.GetBobberInstance(endPosition);
			
			bobber.GlobalPosition = TargetPlayer.GlobalPosition;
			TargetPlayer.AddChild(bobber);
			bobber.Position = Vector2.Zero;
			// TargetPlayer.AddChild(fishingLine);
        }
		GD.Print("Marker landed on " + tileType.ToString());
		GlobalPosition = TargetPlayer.GlobalPosition; // reset position back to origin of parent (Player node)

		return tileType;
    }

	private void CastMarkingProcess()
    {
		if (TargetPlayer != null && castLength < MaxCastingLength)
		{
			castLength += 16; // 16 pixels per 0.5s (1 tile)
							  // Right. Cuz I compound the castLength per call, so the marker increases more and more (if not using assignment)
							  // Need to start from origin

			if (castDirection == Vector2.Up)
				GlobalPosition = initialPosition + new Vector2(0, -castLength);
			else if (castDirection == Vector2.Down)
				GlobalPosition = initialPosition + new Vector2(0, castLength);
			else if (castDirection == Vector2.Right)
				GlobalPosition = initialPosition + new Vector2(castLength, 0);
			else
				GlobalPosition = initialPosition + new Vector2(-castLength, 0);

			// Position += castDirection * new Vector2(16, 16);
		}
		
		// Console Output
		GD.Print("Direction: " + TargetPlayer.FacingDirection);
		GD.Print("Player: " + TargetPlayer.Position);
		GD.Print("Marker: " + Position);
    }

}
