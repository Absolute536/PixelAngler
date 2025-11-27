using Godot;
using System;
using SignalBusNS;
using System.Collections.Generic;
using System.Linq;

public partial class MinigameManager : Node
{
    public static MinigameManager Instance { get; private set; }
    private QuickTimeEvent QTE;

    private ProgressBar _progressBar;
    private Control _progressBarParent;

    private bool _gameStarted = false;

    private string _currentBehaviour = "Action";

    // Backup plan (below)
    private readonly Dictionary<FishBehaviour, string[]> _behaviourInputPair = new ()
    {
      {FishBehaviour.Green, ["Action"]},
      {FishBehaviour.Yellow, ["Up", "Down", "Left", "Right"]},
      {FishBehaviour.Red, ["Interact"]}
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

            if (Input.IsActionPressed(_currentBehaviour))
            {
                GD.Print("Mouse pressed detected");
                // _progressBar.Value += 0.16667; // 10 per second
                _progressBar.Value += 0.25;
            }
            else
            {
                _progressBar.Value -= 0.2; // 20 per second
            }

            // it should be if value becomes 0 (min) --> Lose
            // value becomes >= 100 (max) --> Win

            // try win only first
            if (_progressBar.Value >= _progressBar.MaxValue)
                WinMinigame();
        }
    }

    public void StartQTE(object sender, EventArgs e)
    {
        QTE.StartQuickTimeEvent(0.7f, "Action");  
    }

    public void StartMinigame(Control progressBar)
    {
        _progressBarParent = progressBar;
        _progressBar = progressBar.GetChild(0) as ProgressBar;
        _gameStarted = true;
        SignalBus.Instance.FishBehaviourChanged += HandleFishBehaviourChanged;
    }

    public void EndMinigame()
    {
        _gameStarted = false;
        // _progressBar.QueueFree();
        _progressBarParent.QueueFree();
        SignalBus.Instance.FishBehaviourChanged -= HandleFishBehaviourChanged;
    }

    public void HandleFishBehaviourChanged(string behaviour)
    {
        _currentBehaviour = behaviour;
    }

    private void WinMinigame()
    {
        EndMinigame();
        SignalBus.Instance.OnFishCaught(this, EventArgs.Empty);
    }

    
}