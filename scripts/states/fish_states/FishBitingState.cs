using Godot;
using System;
using SignalBusNS;

[GlobalClass]
public partial class FishBitingState : State
{
    [Export] Fish Fish;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void _ExitTree()
    {
        SignalBus.Instance.QTESucceeded -= HandleQTESuccess;
        SignalBus.Instance.QTEFailed -= HandleQTEFailed;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        SignalBus.Instance.QTESucceeded += HandleQTESuccess;
        SignalBus.Instance.QTEFailed += HandleQTEFailed;

        if (previousState == "FishNibbleState")
        {
            SignalBus.Instance.OnFishBite(this, EventArgs.Empty);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        SignalBus.Instance.QTESucceeded -= HandleQTESuccess;
        SignalBus.Instance.QTEFailed -= HandleQTEFailed;
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        
    }

    public override void ProcessUpdate(double delta)
    {
        
    }

    private void HandleQTESuccess(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
        {
            // QTE Success, instantiate the progress bar and pass the reference to MinigameManager
            PackedScene progressBarScene = GD.Load<PackedScene>("res://scenes/fishing_progress.tscn");
            FishingProgress fishingProgress = progressBarScene.Instantiate<FishingProgress>();
            fishingProgress.Position = new Vector2(-8, 8);

            // MarginContainer progressBarRoot = progressBarScene.Instantiate<MarginContainer>();
            // FishingProgressBar progressBar = progressBarRoot.GetChild(0) as FishingProgressBar; // fix later :P
            // GD.Print("Size: " + progressBar.Size); // size is 0? (cuz only controlled by parent?)
            // progressBar.Size = new Vector2(40, 3);
            // progressBarRoot.Position = new Vector2(-8, 8);

            Fish.AddChild(fishingProgress);
            MinigameManager.Instance.StartMinigame(fishingProgress);

            OnStateTransitioned("FishHookedState");
        }
            
    }

    private void HandleQTEFailed(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
            OnStateTransitioned("FishStartledState");
    }

}