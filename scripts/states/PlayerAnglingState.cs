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
        SignalBus.Instance.ReverseBobberMotionEnded += HandleReverseBobberMotionEnded;
    }
    
    public override void EnterState(string previousState)
    {

    }

    public override void ExitState()
    {
        
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Action") && !QuickTimeEvent.Instance.IsActive) // just a quick and dirty test
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
        OnStateTransitioned("PlayerIdleState");
    }
}