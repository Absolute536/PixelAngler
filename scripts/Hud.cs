using GamePlayer;
using Godot;
using System;

public partial class Hud : CanvasLayer
{
	[Export] Label TestLabel;
	[Export] public DayNightCycleUi DayNightUi;
	private Player _player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		_player = GetTree().GetFirstNodeInGroup("Player") as Player;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
		string tiletype = GameInfo.Instance.GetTileType(_player.GlobalPosition).ToString();
		string location = GameInfo.Instance.GetWorldLocation(_player.GlobalPosition);

		TestLabel.Text = tiletype + "\n" + location;
    }
}
