using Godot;
using GamePlayer;
using SignalBusNS;
using System;


[GlobalClass]
public partial class PlayerAnglingState : State
{   
    [Export] public Bobber Bobber;
    [Export] public Player Player;

    public override void _Ready()
    {
        StateName = Name;
        
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        SignalBus.Instance.ReverseBobberMotionEnded += HandleReverseBobberMotionEnded;
        SignalBus.Instance.QTESucceeded += HandleQTESuccess;
    }

    public override void ExitState()
    {
        base.ExitState();
        SignalBus.Instance.ReverseBobberMotionEnded -= HandleReverseBobberMotionEnded;
        SignalBus.Instance.QTESucceeded += HandleQTESuccess;
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Action") && !QuickTimeEvent.Instance.IsActive) // just a quick and dirty test (HaHaHa....)
        {
            Bobber.ReverseBobberMotion();
            SignalBus.Instance.OnAnglingCancelled(this, EventArgs.Empty);
        }
    }

    public override void PhysicsProcessUpdate(double delta)
    {

    }

    public override void ProcessUpdate(double delta)
    {

    }

    private void HandleReverseBobberMotionEnded(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
            OnStateTransitioned("PlayerIdleState");
    }

    private void HandleQTESuccess(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
            OnStateTransitioned("PlayerFishingState");
    }
}