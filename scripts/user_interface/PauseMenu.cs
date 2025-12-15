using Godot;
using System;

public partial class PauseMenu : Control
{
	[Export] public TextureButton PauseButton;
	[Export] public Label PauseMsgLabel;
    [Export] public TextureRect PauseMenuContainer;
    [Export] public HSlider MasterVolume;
    [Export] public HSlider MusicVolume;
    [Export] public HSlider SfxVolume;

    [Export] public CheckButton FullscreenToggle;
    [Export] public Button HelpButton;
    [Export] public Button QuitButton;

    private const float DefaultMasterVolumeDb = 0.0f;
    private const float DefaultMusicVolumeDb = 0.0f;
    private const float DefaultSfxVolumeDb = -15.0f;

    private const float VolumeDbAdjustment = 3.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        PauseButton.Pressed += ShowHidePauseMenu;

        FullscreenToggle.Toggled += (bool isToogled) =>
        {
            if (isToogled)
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
            else
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
        };

        HelpButton.Pressed += () =>
        {
            ShowHelpMenu();
            AudioManager.Instance.PlaySfx(this, SoundEffect.PaperPlacedDown, false);
        };

        HelpButton.MouseEntered += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};

        QuitButton.Pressed += () =>
        {
            // Invoke Save operation here
            // Interesting fact, quit on tree won't propagate the notificationWmQuit, need another method
            // So we might be able to trigger the saving process without the use of groups?
            // Eh, whatever, the current approach works
            GetTree().CallGroup("PersistentState", "SaveState");
            SaveLoadUtil.Instance.SaveGameStateToFile();
            
            SaveState(); // or we overwrite the config file in memory per action? (probably better? or not?)
            SaveLoadUtil.Instance.SaveSettingsConfig(); // after saving to util

            GetTree().Quit();
        };

        MasterVolume.DragEnded += (bool hasChanged) =>
        {
            if (hasChanged)
            {
                float ratio = (float) (MasterVolume.Value / MasterVolume.MaxValue); // 0.0 ~ 1.0 (0 / 10 ~ 10 / 10, with steps of 1.0)
                AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("Master"), ratio);
            }
        };

        MusicVolume.DragEnded += (bool hasChanged) =>
        {
            if (hasChanged)
            {
                float ratio = (float) (MusicVolume.Value / MusicVolume.MaxValue);
                AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("Music"), ratio);
            }
        };

        SfxVolume.DragEnded += (bool hasChanged) =>
        {
            if (hasChanged)
            {
                float ratio = (float) (SfxVolume.Value / SfxVolume.MaxValue);
                AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("Sfx"), ratio * Mathf.DbToLinear(-18.0f)); // cuz base value is -18db (scale from it)
            }
        };

        InitialiseSettings();
        InitialiseUiSoundEffects();
    }


    private void InitialiseSettings()
    {
        // for this one, we want toggled to be triggered
        FullscreenToggle.ButtonPressed = (bool) SaveLoadUtil.Instance.LoadedSettings.GetValue("Window", "fullscreen"); 
        MasterVolume.Value = (double) SaveLoadUtil.Instance.LoadedSettings.GetValue("Audio", "master_volume");
        MusicVolume.Value = (double) SaveLoadUtil.Instance.LoadedSettings.GetValue("Audio", "music_volume");
        SfxVolume.Value = (double) SaveLoadUtil.Instance.LoadedSettings.GetValue("Audio", "sfx_volume");

    }

    private void InitialiseUiSoundEffects()
    {
        PauseButton.MouseEntered += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};
        PauseButton.MouseExited += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};
        PauseButton.Pressed += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonClick, false);};

        FullscreenToggle.Pressed += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonClick, false);};

        QuitButton.MouseEntered += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};
        QuitButton.MouseExited += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};
        QuitButton.Pressed += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonClick, false);};

        MasterVolume.MouseEntered += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};
        // MasterVolume.MouseExited += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};
        MasterVolume.DragStarted += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonClick, false);};
        MasterVolume.DragEnded += (bool isChanged) => {if (isChanged) {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonClick, false);}};

        MusicVolume.MouseEntered += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};
        // MusicVolume.MouseExited += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};
        MusicVolume.DragStarted += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonClick, false);};
        MusicVolume.DragEnded += (bool isChanged) => {if (isChanged) {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonClick, false);}};

        SfxVolume.MouseEntered += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};
        // SfxVolume.MouseExited += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};
        SfxVolume.DragStarted += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonClick, false);};
        SfxVolume.DragEnded += (bool isChanged) => {if (isChanged) {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonClick, false);}};
    }

    // also part of the persistent state group
    private void SaveState()
    {
        SaveLoadUtil.Instance.LoadedSettings.SetValue("Window", "fullscreen", FullscreenToggle.ButtonPressed);
        SaveLoadUtil.Instance.LoadedSettings.SetValue("Audio", "master_volume", MasterVolume.Value);
        SaveLoadUtil.Instance.LoadedSettings.SetValue("Audio", "music_volume", MusicVolume.Value);
        SaveLoadUtil.Instance.LoadedSettings.SetValue("Audio", "sfx_volume", SfxVolume.Value);
    }

    public void ShowHidePauseMenu()
    {
        // Dang it. THIS is REALLY BAD (duplicated), but I just want to get it done now.
        HelpMenu helpMenu = GetNode<HelpMenu>("/root/Main/HUD/HelpMenu");
        if (helpMenu.Visible)
            return; // do nothing if help menu is visible

        if (PauseMenuContainer.Visible) // change to visible to prevent unpausing when catalogue is open
        {
            FishCatalogueUi catalogue = GetNode<FishCatalogueUi>("/root/Main/HUD/FishCatalogue");
            if (!catalogue.Visible) // only unpause if catalogue is not shown
                GetTree().Paused = false;

            ReleaseFocus();
            // PauseMsgLabel.Visible = false;
            PauseMenuContainer.Visible = false;
            AudioManager.Instance.PlaySfx(this,SoundEffect.PaperPlacedDown, false);
        }	
        else
        {
            GetTree().Paused = true;
            GrabFocus();
            // PauseMsgLabel.Visible = true;
            PauseMenuContainer.Visible = true;
            AudioManager.Instance.PlaySfx(this,SoundEffect.PaperPlacedDown, false);
        }
    }

    private void ShowHelpMenu()
    {
        HelpMenu helpMenu = GetNode<HelpMenu>("/root/Main/HUD/HelpMenu");
        helpMenu.ShowHideHelpMenu();
        ReleaseFocus();

    }
}
