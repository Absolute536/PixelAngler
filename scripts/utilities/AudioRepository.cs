using System;
using System.Collections.Generic;
using Godot;
using SignalBusNS;

public partial class AudioRepository : Node
{
    public static AudioRepository Instance {get; private set;}

    public readonly AudioStreamPlayer BgmAudioPlayer = new ();
    public readonly AudioStreamPlayer AmbienceAudioPlayer = new ();
    public readonly AudioStreamPlayer PlayerActionAudioPlayer = new ();

    // Hardcode it here?
    private readonly Dictionary<SoundEffect, AudioStreamWav> SfxAudioStreamWavs = new ();

    private readonly List<AudioStreamMP3> DayBgmAudioStreams = new ();
    private readonly List<AudioStreamMP3> NightBgmAudioStreams = new ();

    // Keep track of previous Bgm selections so that we won't repeat the same one twice
    private int _previousDayBgm = -1;
    private int _previousNightBgm = -1;

    public override void _Ready()
    {
        Instance = this;

        InitialiseDayTimeBgms();
        InitialiseNightTimeBgms();
        InitialiseSfxWavs();

        AddChild(BgmAudioPlayer);
        AddChild(AmbienceAudioPlayer);
        AddChild(PlayerActionAudioPlayer);

        SignalBus.Instance.TimeOfDayChanged += HandleTimeOfDayChanged;
    }

    public void PlaySfx(Node parent, SoundEffect sfx)
    {
        AudioStreamPlayer audioPlayer = new AudioStreamPlayer()
        {
            Stream = SfxAudioStreamWavs[sfx],
            Bus = "Sfx",
        };

        parent.AddChild(audioPlayer);
        audioPlayer.Play();
        audioPlayer.Finished += audioPlayer.QueueFree;
    }

    private void InitialiseDayTimeBgms()
    {
        // Imma hard code it now (loop 3 times, cuz only 3)
        // Proper way should be get directory, count files, then loop until count while loading each audio file
        for (int i = 0; i < 3; i++)
            DayBgmAudioStreams.Add(GD.Load<AudioStreamMP3>($"res://assets/audio/music/bgm_day_{i}.mp3"));
    }

    private void InitialiseNightTimeBgms()
    {
        // Same for night time's
        for (int i = 0; i < 3; i++)
            NightBgmAudioStreams.Add(GD.Load<AudioStreamMP3>($"res://assets/audio/music/bgm_night_{i}.mp3"));

    }

    private void InitialiseSfxWavs()
    {
        // This one gonna hard code it
        // There are better ways I guess
        // Organise the directory structure and use loop to load (Idk if this is good enough)
        // But for now, just hard code it first (here or use the collection expression?)
        SfxAudioStreamWavs.Add(SoundEffect.MinigameSuccess, GD.Load<AudioStreamWav>("res://assets/audio/sound_effects/minigame_success.wav"));
        SfxAudioStreamWavs.Add(SoundEffect.MinigameFailure, GD.Load<AudioStreamWav>("res://assets/audio/sound_effects/minigame_failure.wav"));
    }

    private void HandleTimeOfDayChanged(object sender, EventArgs e)
    {
        // Time of day changed
        // Ease out(?) the current playing bgm stream
        // Stop bgm player
        // after some delay(?)
        // switch to a new one and start playing
        TimeOfDay timeOfDay = InGameTime.Instance.GetCurrentTimeOfDay();
        if (timeOfDay == TimeOfDay.Day || timeOfDay == TimeOfDay.Dawn)
        {
            // https://sfxr.me/ (sfx)
        }
    }
}

public enum SoundEffect
{
    MinigameSuccess,
    MinigameFailure,
    QteNotification,
    WaterSplash,
    ForwardCast,
    ReverseCast,
    FishCaughtNotification,
    


}