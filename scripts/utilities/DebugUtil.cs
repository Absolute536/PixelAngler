using GamePlayer;
using Godot;
using System;

public partial class DebugUtil : Control
{

	[Export] public CheckButton DebugVisibilityToogle;
	[Export] public VBoxContainer DebugMenu;

	[Export] public Label TileLabel;
	[Export] public Label LocationLabel;
	[Export] public Label FpsLabel;

	[Export] public ButtonGroup GameSpeedToogle;

	private Player DebugPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DebugPlayer = GetTree().GetFirstNodeInGroup("Player") as Player;

		DebugVisibilityToogle.Toggled += toogledOn => { if (toogledOn) {DebugMenu.Visible = true;} else {DebugMenu.Visible = false;}};
		GameSpeedToogle.Pressed += AdjustGameSpeed;
	}

    public override void _PhysicsProcess(double delta)
    {
        if (DebugMenu.Visible)
		{
			TileLabel.Text = "Tile: " + GameInfo.Instance.GetTileType(DebugPlayer.GlobalPosition).ToString();
			LocationLabel.Text = "Location: " + GameInfo.Instance.GetWorldLocation(DebugPlayer.GlobalPosition);
			FpsLabel.Text = "FPS: " + Engine.GetFramesPerSecond().ToString();
		}
    }

	

	private void AdjustGameSpeed(BaseButton button)
	{
		// yeah, we'll just use the node name here to make it quick
		if (!button.ButtonPressed)
		{
			InGameTime.Instance.GameSpeed = 1.0f;
		}
		else
		{
			if (button.Name == "Speed10")
				InGameTime.Instance.GameSpeed = 10.0f;
			else if (button.Name == "Speed25")
				InGameTime.Instance.GameSpeed = 25.0f;
		}
	}
}
