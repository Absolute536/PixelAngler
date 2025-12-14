using System;
using System.Collections.Generic;
using Godot;
using SignalBusNS;

public partial class AudioManager : Node
{
    public static AudioManager Instance {get; private set;}

    private readonly AudioStreamPlayer _bgmAudioPlayer = new ();

    private readonly Dictionary<PlayerActionAudio, AudioStreamPlayer> _playerActionAudioPlayers = new ();
    private readonly Dictionary<SoundEffect, AudioStream> _sfxAudioStreams = new ();

    private readonly List<AudioStream> _dayBgmAudioStreams = new ();
    private readonly List<AudioStream> _nightBgmAudioStreams = new ();

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
        InitialisePlayerActionPlayers();

        InitialiseAudioBusVolumes();

        AddChild(_bgmAudioPlayer);

        _currentTimeOfDay = InGameTime.Instance.GetCurrentTimeOfDay();

        _bgmAudioPlayer.Finished += () =>
        {
            if (!_isBgmFading) // a flag to prevent switching abruptly if music finished when tweening
            {
                GetTree().CreateTimer(GD.RandRange(5, 10), true, true).Timeout += () =>
                {
                    if (_currentTimeOfDay == TimeOfDay.Day || _currentTimeOfDay == TimeOfDay.Dawn)
                        _bgmAudioPlayer.Stream = GetRandomDayBgm();
                    else if (_currentTimeOfDay == TimeOfDay.Night || _currentTimeOfDay == TimeOfDay.Dusk)
                        _bgmAudioPlayer.Stream = GetRandomNightBgm();

                    _bgmAudioPlayer.Play();
                };            
            }

        };

        if (_currentTimeOfDay == TimeOfDay.Day || _currentTimeOfDay == TimeOfDay.Dawn)
            _bgmAudioPlayer.Stream = GetRandomDayBgm();
        else
            _bgmAudioPlayer.Stream = GetRandomNightBgm();
        
        _bgmAudioPlayer.Bus = "Music";

        SignalBus.Instance.TimeOfDayChanged += HandleTimeOfDayChanged;
        SignalBus.Instance.WorldLoaded += HandleWorldLoaded; // Only start playing bgm when the tilesets are initialised
    }

    public void PlaySfx(Node parent, SoundEffect sfx, bool enablePitchShift)
    {
        // Introduce some pitch shift per sfx play
        AudioEffectPitchShift pitchShift = AudioServer.GetBusEffect(AudioServer.GetBusIndex("Sfx"), 0) as AudioEffectPitchShift;
        if (enablePitchShift)
            pitchShift.PitchScale = (float) GD.RandRange(0.9995, 1.0005);
        else
            pitchShift.PitchScale = 1.0f;

        AudioStreamPlayer audioPlayer = new AudioStreamPlayer()
        {
            Stream = _sfxAudioStreams[sfx],
            Bus = "Sfx",
        };
        audioPlayer.ProcessMode = ProcessModeEnum.Always;
        parent.AddChild(audioPlayer);
        audioPlayer.Play();
        audioPlayer.Finished += audioPlayer.QueueFree;
    }

    public void PlayActionAudio(PlayerActionAudio action)
    {
        AudioEffectPitchShift pitchShift = AudioServer.GetBusEffect(AudioServer.GetBusIndex("Sfx"), 0) as AudioEffectPitchShift;
        pitchShift.PitchScale = 1.0f; // reset to normal for the action's sfx
        _playerActionAudioPlayers[action].Play();
    }

    public void StopActionAudio(PlayerActionAudio action)
    {
        _playerActionAudioPlayers[action].Stop();
    }

    private void InitialiseDayTimeBgms()
    {
        // Imma hard code it now (loop 3 times, cuz only 3)
        // Proper way should be get directory, count files, then loop until count while loading each audio file
        for (int i = 0; i < 3; i++)
            // _dayBgmAudioStreams.Add(AudioStreamMP3.LoadFromFile($"res://assets/audio/music/bgm_day_{i}.mp3"));
            _dayBgmAudioStreams.Add(GD.Load<AudioStreamMP3>($"res://assets/audio/music/bgm_day_{i}.mp3"));
    }

    private void InitialiseNightTimeBgms()
    {
        // Same for night time's
        for (int i = 0; i < 3; i++)
            // _nightBgmAudioStreams.Add(AudioStreamMP3.LoadFromFile($"res://assets/audio/music/bgm_night_{i}.mp3"));
            _nightBgmAudioStreams.Add(GD.Load<AudioStreamMP3>($"res://assets/audio/music/bgm_night_{i}.mp3"));
    }

    private void InitialiseSfxWavs()
    {
        // Better version: iterate through the enum values that represent each sound effect and load the corresponding audio file
        foreach(SoundEffect effect in Enum.GetValues(typeof(SoundEffect)))
        {
            string effectName = effect.ToString().ToSnakeCase(); //So that the SoundEffect (CamelCase) becones sound_effect (snake_case). Thanks Godot!
            _sfxAudioStreams.Add(effect, GD.Load<AudioStreamWav>($"res://assets/audio/sound_effects/{effectName}.wav"));
        }

        // https://sfxr.me/ (sfx)
    }

    private void InitialisePlayerActionPlayers()
    {
        AudioStreamPlayer walkingPlayer = new AudioStreamPlayer()
        {
            Stream = GD.Load<AudioStreamWav>("res://assets/audio/sound_effects/action_walking.wav"),
            Bus = "Sfx",
            ProcessMode = ProcessModeEnum.Pausable
        };
        _playerActionAudioPlayers.Add(PlayerActionAudio.Walking, walkingPlayer);
        walkingPlayer.Finished += () => {walkingPlayer.Play();}; // somehow need this to loop, despite already set in the import menu
        AddChild(walkingPlayer);

        AudioStreamPlayer fishGreenPlayer = new AudioStreamPlayer()
        {
            Stream = GD.Load<AudioStreamWav>("res://assets/audio/sound_effects/action_fishing_green.wav"),
            Bus = "Sfx",
            ProcessMode = ProcessModeEnum.Pausable
        };
        _playerActionAudioPlayers.Add(PlayerActionAudio.FishingGreen, fishGreenPlayer);
        AddChild(fishGreenPlayer);

        AudioStreamPlayer fishYellowPlayer = new AudioStreamPlayer()
        {
            Stream = GD.Load<AudioStreamWav>("res://assets/audio/sound_effects/action_fishing_yellow.wav"),
            Bus = "Sfx",
            ProcessMode = ProcessModeEnum.Pausable,
        };
        
        _playerActionAudioPlayers.Add(PlayerActionAudio.FishingYellow, fishYellowPlayer);
        AddChild(fishYellowPlayer);

        AudioStreamPlayer fishRedPlayer = new AudioStreamPlayer()
        {
            Stream = GD.Load<AudioStreamWav>("res://assets/audio/sound_effects/action_fishing_red.wav"),
            Bus = "Sfx",
            ProcessMode = ProcessModeEnum.Pausable
        };
        _playerActionAudioPlayers.Add(PlayerActionAudio.FishingRed, fishRedPlayer);
        AddChild(fishRedPlayer);
    }

    private void InitialiseAudioBusVolumes()
    {
        float masterVolume = (float) SaveLoadUtil.Instance.LoadedSettings.GetValue("Audio", "master_volume");
        float musicVolume = (float) SaveLoadUtil.Instance.LoadedSettings.GetValue("Audio", "music_volume");
        float sfxVolume = (float) SaveLoadUtil.Instance.LoadedSettings.GetValue("Audio", "sfx_volume");
        AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("Master"), masterVolume / 10.0f);
        AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("Music"), musicVolume / 10.0f);
        AudioServer.SetBusVolumeLinear(AudioServer.GetBusIndex("Sfx"), sfxVolume / 10.0f * Mathf.DbToLinear(-18.0f));
        // GD.Print("Master: " + AudioServer.GetBusVolumeDb(0));
        // GD.Print("Music: " + AudioServer.GetBusVolumeDb(1));
        // GD.Print("Sfx: " + AudioServer.GetBusVolumeDb(2));
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
            volumeTween.TweenProperty(_bgmAudioPlayer, "volume_db", -80.0f, 5.0); // the volume value is an offset to the stream's volume (-80db guaranteed to be non-audible)
            volumeTween.Finished += () =>
            {
                _bgmAudioPlayer.Stop();
                _bgmAudioPlayer.VolumeDb = 0.0f; // reset back
                _bgmAudioPlayer.Stream = stream;
                _bgmAudioPlayer.Play();

                _isBgmFading = false;
            };
        }

        _currentTimeOfDay = nextTimeOfDay;
    }

    private void HandleWorldLoaded(object sender, EventArgs e)
    {
        _bgmAudioPlayer.Play();
    }

    private AudioStream GetRandomDayBgm()
    {
        int selection = GD.RandRange(0, _dayBgmAudioStreams.Count - 1); // remember to -1 cuz of inclusive
        ShuffleListWithSideEffect<AudioStream>(_dayBgmAudioStreams);

        return _dayBgmAudioStreams[selection];
    }

    private AudioStream GetRandomNightBgm()
    {
        int selection = GD.RandRange(0, _nightBgmAudioStreams.Count - 1);
        ShuffleListWithSideEffect<AudioStream>(_nightBgmAudioStreams);

        return _nightBgmAudioStreams[selection];
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
    QteSuccess,
    BobberSplash,
    CastMarkProgress,
    ForwardCast,
    ReverseCast,
    FishNibble,
    FishCaught,
    ShowOff, // ba ba ba ba ~ sth like that?
    ButtonHover,
    ButtonClick,
    PaperFlip,
    PaperPlacedDown
}

public enum PlayerActionAudio
{
    Walking,
    FishingGreen,
    FishingYellow,
    FishingRed
}