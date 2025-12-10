using Godot;
using System;

public partial class PauseMenu : Control
{
	[Export] public TextureButton actualButton;
	[Export] public Label PauseMsgLabel;
    [Export] public TextureRect PauseMenuContainer;

    [Export] public CheckButton FullscreenToggle;
    [Export] public Button QuitButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        actualButton.Pressed += () =>
        {
            if (GetTree().Paused)
            {
                GetTree().Paused = false;
                ReleaseFocus();
				// PauseMsgLabel.Visible = false;
                PauseMenuContainer.Visible = false;
            }	
			else
            {
				GetTree().Paused = true;
                GrabFocus();
                // PauseMsgLabel.Visible = true;
                PauseMenuContainer.Visible = true;
            }
				
        };

        FullscreenToggle.Toggled += (bool isToogled) =>
        {
            if (isToogled)
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
            else
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        };

        QuitButton.Pressed += () =>
        {
            // Invoke Save operation here
            // then quit
            GetTree().Quit();
        };
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }
}
