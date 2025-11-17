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
            Fish.IsHooked = true;
            Fish.LatchTarget.IsLatchedOn = true;
            OnStateTransitioned("FishHookedState");
        }
        
    }

    private void HandleQTEFailed(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
            OnStateTransitioned("FishWanderState");
    }

}