using Godot;
using System;
using GamePlayer;
using GameWorld;
using SignalBusNS;

public partial class CastMarker : Sprite2D
{	
	// Since CastMarker is a part of Player (Player has-a CastMarker, we make it an exported property instead)
	[Export] public Player TargetPlayer;
	[Export] public Timer CastTimer;
	private const int MaxCastingLength = 80;

	// Temporary might change later (to signal?)
	[Export] public Bobber Bobber;

	private Vector2 _castDirection;
	private Vector2 _initialPosition;
	private int _castLength = 0;

	public CastMarker()
	{
		Visible = false;
		//TopLevel = false; // Fucking hell, it's because the parent (grouping node) is a plain Node, WITHOUT any transform properties (god damn itttttt!!!)
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Connect to CastingStarted/Ended signals
		// SignalBus.Instance.CastMarkingStarted += HandleCastMarkingStarted;
		// SignalBus.Instance.SubscribeToCastMarkingEnded(this, HandleCastMarkingEnded);

		// Connect to Timeout signal of the marker's Timer
		CastTimer.Timeout += CastMarkingProcess;
		GlobalPosition = TargetPlayer.GlobalPosition;
	}

	public void StartCastMarking()
	{
		Visible = true;
		CastTimer.Start();

		// Initialise casting marker's direction and position (player's position + 2 tiles away from the facing direction)
		_castDirection = TargetPlayer.FacingDirection;
		_initialPosition = TargetPlayer.GlobalPosition + (_castDirection * new Vector2(16, 16) * 2);
		GlobalPosition = _initialPosition;

		// Reset bobber and fishing line statuses
		Bobber.ResetBobberStatus();
		// FishingLine.ResetLineStatus(); ~ something like this maybe
	}
	
	public Tuple<TileType, Vector2> StopCastMarking()
    {
        _castLength = 0; // reset cast length
		CastTimer.Stop();
		Visible = false;

		// Note: use GlobalPosition instead of Position, else bobber's position will become the offset vector of marker
		// Bobber.StartBobberMotion(GlobalPosition);
		// OK, now I see. I need it to reset on STARTING the action (click), not when the action ends (release)
		// because now the Start action and resetting is on release

		// We don't really need this here, it is handled by the casting state (by raising the event and get the return value of this)
		TileType tileType = GameInfo.Instance.GetTileType(GlobalPosition);

		GD.Print("Marker landed on " + tileType.ToString());
		return new (tileType, GlobalPosition);
    }

	private void CastMarkingProcess()
    {
		if (_castLength < MaxCastingLength)
		{
			// 16 pixels per Timeout (1 tile)
			// Right. Cuz I compound the castLength per call, so the marker increases more and more, becoming the displacement instead
			// Need to start from the initial position of the marker and add the castLength
			_castLength += 16; 

			if (_castDirection == Vector2.Up)
				GlobalPosition = _initialPosition + new Vector2(0, -_castLength);
			else if (_castDirection == Vector2.Down)
				GlobalPosition = _initialPosition + new Vector2(0, _castLength);
			else if (_castDirection == Vector2.Right)
				GlobalPosition = _initialPosition + new Vector2(_castLength, 0);
			else
				GlobalPosition = _initialPosition + new Vector2(-_castLength, 0);
		}
		
		// Console Output
		GD.Print("Direction: " + TargetPlayer.FacingDirection);
		GD.Print("Player: " + TargetPlayer.Position);
		GD.Print("Marker Offset FROM Player: " + Position);
    }

}
