using Godot;
using System;

public partial class PauseMenu : Control
{
	[Export] public TextureButton actualButton;
	[Export] public Label PauseMsgLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        actualButton.Pressed += () =>
        {
            if (GetTree().Paused)
            {
                GetTree().Paused = false;
				PauseMsgLabel.Visible = false;
            }	
			else
            {
				GetTree().Paused = true;
                PauseMsgLabel.Visible = true;
            }
				
        };
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }
}
