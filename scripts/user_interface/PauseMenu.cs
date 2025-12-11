using Godot;
using System;

public partial class PauseMenu : Control
{
	[Export] public TextureButton actualButton;
	[Export] public Label PauseMsgLabel;
    [Export] public TextureRect PauseMenuContainer;
    [Export] public HSlider MasterVolume;
    [Export] public HSlider MusicVolume;
    [Export] public HSlider SfxVolume;

    [Export] public CheckButton FullscreenToggle;
    [Export] public Button QuitButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        actualButton.Pressed += ShowPauseMenu;

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
            GetTree().CallGroup("PersistentState", "SaveState");
            SaveLoadUtil.Instance.SaveGameStateToFile();
            
            SaveState(); // or we overwrite the config file in memory per action? (probably better? or not?)
            SaveLoadUtil.Instance.SaveSettingsConfig(); // after saving to util

            GetTree().Quit();
        };

        InitialiseSettings();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ShowPause"))
        {
            if  (PauseMenuContainer.Visible)
            {
                PauseMenuContainer.Visible = false;
                ReleaseFocus();

                GetTree().Paused = false;
            }
        }
    }


    private void InitialiseSettings()
    {
        // for this one, we want toggled to be triggered
        FullscreenToggle.ButtonPressed = (bool) SaveLoadUtil.Instance.LoadedSettings.GetValue("Window", "fullscreen"); 
        MasterVolume.Value = (double) SaveLoadUtil.Instance.LoadedSettings.GetValue("Audio", "master_volume");
        MusicVolume.Value = (double) SaveLoadUtil.Instance.LoadedSettings.GetValue("Audio", "music_volume");
        SfxVolume.Value = (double) SaveLoadUtil.Instance.LoadedSettings.GetValue("Audio", "sfx_volume");

    }

    // also part of the persistent state group
    private void SaveState()
    {
        SaveLoadUtil.Instance.LoadedSettings.SetValue("Window", "fullscreen", FullscreenToggle.ButtonPressed);
        SaveLoadUtil.Instance.LoadedSettings.SetValue("Audio", "master_volume", MasterVolume.Value);
        SaveLoadUtil.Instance.LoadedSettings.SetValue("Audio", "music_volume", MusicVolume.Value);
        SaveLoadUtil.Instance.LoadedSettings.SetValue("Audio", "sfx_volume", SfxVolume.Value);
    }

    public void ShowPauseMenu()
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
    }
}
