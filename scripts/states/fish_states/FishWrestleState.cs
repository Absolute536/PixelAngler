using Godot;
using System;
using SignalBusNS;
using System.Collections;

[GlobalClass]
public partial class FishWrestleState : State
{
    [Export] public Fish Fish;
    [Export] public Timer MovementTimer;
    private double _directionDuration;
    private Random _directionRandomiser = new ();

    private int clicksNeeded;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        
        clicksNeeded = _directionRandomiser.Next(3, 8); // random for now, can derive from FishInfo later
        GD.Print("Clicks Needed: " + clicksNeeded);

        // SignalBus.Instance.OnFishBehaviourChanged(FishBehaviour.Yellow, clicksNeeded);
        MinigameManager.Instance.CurrentBehaviour = FishBehaviour.Yellow;
        MinigameManager.Instance.ClicksNeeded = clicksNeeded;
        
        SignalBus.Instance.FishWrestleCompleted += HandleFishWrestleCompleted;
        // MinigameManager.Instance.ActionRepeat = clicksNeeded;

        SignalBus.Instance.FishCaught += HandleFishCaught;
    }

    public override void ExitState()
    {
        base.ExitState();
        SignalBus.Instance.FishCaught -= HandleFishCaught;
        SignalBus.Instance.FishWrestleCompleted -= HandleFishWrestleCompleted;
    }

    public override void HandleInput(InputEvent @event)
    {
        // no handling input?
        // quick test first
        // if (@event.IsActionPressed("Action"))
        // {
        //     clicksNeeded -= 1;
        // }
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        // Fish.MoveAndSlide();
        // if (clicksNeeded <= 0)
        //     OnStateTransitioned("FishHookedState");
        // signal from minigame to change state
    }

    public override void ProcessUpdate(double delta)
    {
        // nothing
    }

    private void HandleFishCaught(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
        {
            Fish.IsHooked = false;
            Fish.LatchTarget.IsLatchedOn = false;
            Fish.IsCaught = true;
            OnStateTransitioned("FishCaughtState");
        }
    }
    private void HandleFishWrestleCompleted(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
            OnStateTransitioned("FishHookedState");
    }
}