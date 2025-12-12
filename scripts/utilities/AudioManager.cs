using System;
using System.Collections.Generic;
using GamePlayer;
using Godot;
using SignalBusNS;

public partial class AudioManager : Node
{
    public static AudioManager Instance {get; private set;}

    public readonly AudioStreamPlayer BgmAudioPlayer = new ();
    public readonly AudioStreamPlayer AmbienceAudioPlayer = new ();

    // Hardcode it here?
    private readonly Dictionary<SoundEffect, AudioStream> SfxAudioStreams = new ();

    private readonly List<AudioStream> DayBgmAudioStreams = new ();
    private readonly List<AudioStream> NightBgmAudioStreams = new ();

    private bool _isBgmFading = false;
    private TimeOfDay _currentTimeOfDay;

    public override void _Ready()
    {
        Instance = this;

        ProcessMode = ProcessModeEnum.Always; // Continue playing even when paused (time of day won't change so no matter)

        // Initialise audio stream collections
        InitialiseDayTimeBgms();
        InitialiseNightTimeBgms();
        InitialiseSfxWavs();

        AddChild(BgmAudioPlayer);

        _currentTimeOfDay = InGameTime.Instance.GetCurrentTimeOfDay();

        BgmAudioPlayer.Finished += () =>
        {
            if (!_isBgmFading) // a flag to prevent switching abruptly if music finished when tweening
            {
                GetTree().CreateTimer(GD.RandRange(5, 10), true, true).Timeout += () =>
                {
                    if (_currentTimeOfDay == TimeOfDay.Day || _currentTimeOfDay == TimeOfDay.Dawn)
                        BgmAudioPlayer.Stream = GetRandomDayBgm();
                    else if (_currentTimeOfDay == TimeOfDay.Night || _currentTimeOfDay == TimeOfDay.Dusk)
                        BgmAudioPlayer.Stream = GetRandomNightBgm();

                    BgmAudioPlayer.Play();
                };            
            }

        };

        if (_currentTimeOfDay == TimeOfDay.Day || _currentTimeOfDay == TimeOfDay.Dawn)
            BgmAudioPlayer.Stream = GetRandomDayBgm();
        else
            BgmAudioPlayer.Stream = GetRandomNightBgm();
        
        BgmAudioPlayer.Bus = "Music";
        BgmAudioPlayer.Play();

        SignalBus.Instance.TimeOfDayChanged += HandleTimeOfDayChanged;
    }

    public void PlaySfx(Node parent, SoundEffect sfx)
    {
        AudioStreamPlayer audioPlayer = new AudioStreamPlayer()
        {
            Stream = SfxAudioStreams[sfx],
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
            DayBgmAudioStreams.Add(AudioStreamMP3.LoadFromFile($"res://assets/audio/music/bgm_day_{i}.mp3"));
    }

    private void InitialiseNightTimeBgms()
    {
        // Same for night time's
        for (int i = 0; i < 3; i++)
            NightBgmAudioStreams.Add(AudioStreamMP3.LoadFromFile($"res://assets/audio/music/bgm_night_{i}.mp3"));
    }

    private void InitialiseSfxWavs()
    {
        // This one gonna hard code it
        // There are better ways I guess
        // Organise the directory structure and use loop to load (Idk if this is good enough)
        // But for now, just hard code it first (here or use the collection expression?)
        SfxAudioStreams.Add(SoundEffect.MinigameSuccess, AudioStreamWav.LoadFromFile("res://assets/audio/sound_effects/minigame_success.wav"));
        SfxAudioStreams.Add(SoundEffect.MinigameFailure, AudioStreamWav.LoadFromFile("res://assets/audio/sound_effects/minigame_failure.wav"));

        // https://sfxr.me/ (sfx)
    }

    private void HandleTimeOfDayChanged(object sender, EventArgs e)
    {
        // Time of day changed
        // Ease out(?) the current playing bgm stream
        // Stop bgm player
        // after some delay(?)
        // switch to a new one and start playing

        // Dawn -> Day (no change)
        // Dusk -> Night (no change)
        // It's always gonna be Dawn -> Day -> Dusk -> Night -> ... cycle back
        // so only day to dusk and night to dawn, we change the bgm? (think so)
        // so need to initialise the track based on time of day on startup

        TimeOfDay nextTimeOfDay = InGameTime.Instance.GetCurrentTimeOfDay(); // well, this could've been part of the event's argument (oh well...)

        if ((_currentTimeOfDay == TimeOfDay.Day && nextTimeOfDay == TimeOfDay.Dusk) ||
            (_currentTimeOfDay == TimeOfDay.Night && nextTimeOfDay == TimeOfDay.Dawn))
        {
            _isBgmFading = true;

            // actually if we leave it as AudioStream, it'll probably work
            AudioStreamMP3 stream = nextTimeOfDay == TimeOfDay.Dusk ? GetRandomNightBgm() as AudioStreamMP3 : GetRandomDayBgm() as AudioStreamMP3;
            // so tween volume until complete then reset volume back, stop the music and change stream then play
            // but what if when tweening, music finished?
            // music finished after tweening, not possible? (cuz calling stop won't emit the finished signal)
            // Guess I can use a flag to handle that

            Tween volumeTween = CreateTween();
            // Tolol, it's DB (decibel), and I somehow use the linear value (should be -1.0f)
            volumeTween.TweenProperty(BgmAudioPlayer, "volume_db", -80.0f, 5.0); // the volume value is an offset to the stream's volume (-80db guaranteed to be non-audible)
            volumeTween.Finished += () =>
            {
                BgmAudioPlayer.Stop();
                BgmAudioPlayer.VolumeDb = 0.0f; // reset back
                BgmAudioPlayer.Stream = stream;
                BgmAudioPlayer.Play();

                _isBgmFading = false;
            };
        }

        _currentTimeOfDay = nextTimeOfDay;
    }

    private AudioStream GetRandomDayBgm()
    {
        int selection = GD.RandRange(0, DayBgmAudioStreams.Count - 1); // remember to -1 cuz of inclusive
        ShuffleListWithSideEffect<AudioStream>(DayBgmAudioStreams);

        return DayBgmAudioStreams[selection];
    }

    private AudioStream GetRandomNightBgm()
    {
        int selection = GD.RandRange(0, NightBgmAudioStreams.Count - 1);
        ShuffleListWithSideEffect<AudioStream>(NightBgmAudioStreams);

        return NightBgmAudioStreams[selection];
    }

    // actually this can be an extension method for List OR should be in another utility kinda class
    // shuffle using this: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    // named "...WithSideEffect" because it modifie the list argument (not pure)
    private void ShuffleListWithSideEffect<T>(List<T> list)
    {
        Random rand = new Random();

        // stop at i >= 1 because we want it to try and swap if the list is not empty or only has 1 element
        // basically we start from the back and gradually try to swap with elements in front, until we reach the second one (no more swapping to be done)
        for (int i = list.Count - 1; i >= 1; i--)
        {
            int j = rand.Next(i + 1);

            T temp = list[i]; // can use tuple to swap values
            list[i] = list[j];
            list[j] = temp;
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