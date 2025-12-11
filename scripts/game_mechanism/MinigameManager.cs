using Godot;
using System;
using SignalBusNS;
using System.Collections.Generic;
using System.Linq;

public partial class MinigameManager : Node
{
    public static MinigameManager Instance { get; private set; }
    private QuickTimeEvent QTE;

    private FishingProgress _fishingProgress;

    private bool _gameStarted = false;

    // private string _currentBehaviour = "Action";
    public FishBehaviour CurrentBehaviour { get => _currentBehaviour; set => _currentBehaviour = value; }
    private FishBehaviour _currentBehaviour;

    public FishSpecies CurrentSpeciesInGame{get => _currentSpeciesInGame; set => _currentSpeciesInGame = value;}
    private FishSpecies _currentSpeciesInGame;

    public int ClicksNeeded { get => _clicksNeeded; set => _clicksNeeded = value; }
    private int _clicksNeeded;

    // Backup plan (below)
    private readonly Dictionary<FishBehaviour, string> _behaviourInputPair = new ()
    {
      {FishBehaviour.Green, "Action"},
      {FishBehaviour.Yellow, "Action"},
      {FishBehaviour.Red, "Interact"}
    };

    public override void _Ready()
    {
        Instance = this;
        QTE = QuickTimeEvent.Instance; // get the static instance of quick time event (autoload)
        SignalBus.Instance.FishBite += StartQTE;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_gameStarted)
        {
            // Retrieve the actionName for the behaviour
            // string[] actionList = _behaviourInputPair[_currentBehaviour];
            // // Check if the input matches with any of the actionName for the behaviour
            // bool isInputValid = actionList.Any((element) => Input.IsActionPressed(element));

            if (CurrentBehaviour != FishBehaviour.Yellow)
            {
                if (Input.IsActionPressed(_behaviourInputPair[CurrentBehaviour]))
                {
                    // Only increase the progress when green
                    // For red, the progress won't decrease when RMB is held (try it first?) -> this is better
                    // also scale based on aggressiveness?
                    if (CurrentBehaviour == FishBehaviour.Green)
                        _fishingProgress.GameProgressBar.Value += 0.40 * (1 - _currentSpeciesInGame.Aggressiveness);

                    GD.Print("Mouse pressed detected");
                }
                else
                    _fishingProgress.GameProgressBar.Value -= 0.30 * _currentSpeciesInGame.Aggressiveness;
                
                // just quick test
                StyleBoxFlat promptStyleBox = _fishingProgress.GameActionPrompt.GetThemeStylebox("normal").Duplicate() as StyleBoxFlat;
                // Color textColour = _fishingProgress.GameActionPrompt.GetThemeColor("default_color");
                if (CurrentBehaviour == FishBehaviour.Green)
                {
                    promptStyleBox.BgColor = Colors.Green;
                }
                else
                    promptStyleBox.BgColor = Colors.Red;

                _fishingProgress.GameActionPrompt.AddThemeStyleboxOverride("normal", promptStyleBox);
                _fishingProgress.GameActionPrompt.AddThemeColorOverride("default_color", Colors.White);
                // _progressBar.Value += 0.16667; // 10 per second
            }
            else
            {
                // quick test
                StyleBoxFlat promptStyleBox = _fishingProgress.GameActionPrompt.GetThemeStylebox("normal").Duplicate() as StyleBoxFlat;
                promptStyleBox.BgColor = Colors.Yellow;
                _fishingProgress.GameActionPrompt.AddThemeStyleboxOverride("normal", promptStyleBox);
                _fishingProgress.GameActionPrompt.AddThemeColorOverride("default_color", Colors.Black);

                _fishingProgress.GameActionPrompt.Text = ClicksNeeded.ToString();
                if (Input.IsActionJustPressed(_behaviourInputPair[CurrentBehaviour]))
                {
                    ClicksNeeded -= 1;
                    // Halt increase until clicks needed are fulfilled, only decrease while not met
                    // _progressBar.Value += 0.25;
                }
                else
                    _fishingProgress.GameProgressBar.Value -= 0.20 * _currentSpeciesInGame.Aggressiveness; // 20 per second
                
                if (ClicksNeeded <= 0)
                {
                    SignalBus.Instance.OnFishWrestleCompleted(this, EventArgs.Empty);
                    _fishingProgress.GameActionPrompt.Text = "";
                }
                    
            }

            // it should be if value becomes 0 (min) --> Lose
            // value becomes >= 100 (max) --> Win

            // hmm, idk about the freeze frame
            if (_fishingProgress.GameProgressBar.Value >= _fishingProgress.GameProgressBar.MaxValue)
            {
                // Play win sound cue

                SceneTreeTimer freezeFrameTimer = GetTree().CreateTimer(0.25, true, true);
                freezeFrameTimer.Timeout += () =>
                {
                    GetTree().Paused = false;
                    WinMinigame();
                };
                GetTree().Paused = true;
            }
                
            
            if (_fishingProgress.GameProgressBar.Value == _fishingProgress.GameProgressBar.MinValue)
            {
                // Play lose sound cue

                SceneTreeTimer freezeFrameTimer = GetTree().CreateTimer(0.25, true, true);
                freezeFrameTimer.Timeout += () =>
                {
                    GetTree().Paused = false;
                    LoseMinigame();
                };
                GetTree().Paused = true;
            }
                
        }
    }

    public void StartQTE(object sender, EventArgs e)
    {
        QTE.StartQuickTimeEvent(0.7f, "Action"); // 0.7 seconds 
    }

    public void StartMinigame(FishingProgress fishingProgress)
    {
        _fishingProgress = fishingProgress;
        _gameStarted = true;
    }

    public void EndMinigame()
    {
        _gameStarted = false;
        _fishingProgress.QueueFree();
    }

    // public void HandleFishBehaviourChanged(FishBehaviour behaviour, int repeatNumber)
    // {
    //     CurrentBehaviour = behaviour;
    //     _actionRepeat = repeatNumber;
    // }

    private void WinMinigame()
    {
        EndMinigame();
        SignalBus.Instance.OnFishCaught(this, EventArgs.Empty);
    }

    private void LoseMinigame()
    {
        EndMinigame();
        SignalBus.Instance.OnFishLost(this, EventArgs.Empty);
    }

    
}