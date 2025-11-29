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
                    _fishingProgress.GameProgressBar.Value += 0.25;
                    GD.Print("Mouse pressed detected");
                }
                else
                    _fishingProgress.GameProgressBar.Value -= 0.2;
                    
                // _progressBar.Value += 0.16667; // 10 per second
            }
            else
            {
                _fishingProgress.GameActionPrompt.Text = ClicksNeeded.ToString();
                if (Input.IsActionJustPressed(_behaviourInputPair[CurrentBehaviour]))
                {
                    ClicksNeeded -= 1;
                    // Halt increase until clicks needed are fulfilled, only decrease while not met
                    // _progressBar.Value += 0.25;
                }
                else
                    _fishingProgress.GameProgressBar.Value -= 0.2; // 20 per second
                
                if (ClicksNeeded <= 0)
                {
                    SignalBus.Instance.OnFishWrestleCompleted(this, EventArgs.Empty);
                    _fishingProgress.GameActionPrompt.Text = "";
                }
                    
            }

            // it should be if value becomes 0 (min) --> Lose
            // value becomes >= 100 (max) --> Win

            // try win only first
            if (_fishingProgress.GameProgressBar.Value >= _fishingProgress.GameProgressBar.MaxValue)
                WinMinigame();
        }
    }

    public void StartQTE(object sender, EventArgs e)
    {
        QTE.StartQuickTimeEvent(0.7f, "Action");  
    }

    public void StartMinigame(FishingProgress fishingProgress)
    {
        _fishingProgress = fishingProgress;
        _gameStarted = true;
        // SignalBus.Instance.FishBehaviourChanged += HandleFishBehaviourChanged;
    }

    public void EndMinigame()
    {
        _gameStarted = false;
        // _progressBar.QueueFree();
        _fishingProgress.QueueFree();
        // SignalBus.Instance.FishBehaviourChanged -= HandleFishBehaviourChanged;
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

    
}