using System;
using Godot;
using SignalBusNS;

[GlobalClass]
public partial class PlayerFishingState : State
{
    [Export] public Bobber Bobber;

    [Export] public FishingQuickTimeEvent FishingQTE;

    public override void _Ready()
    {
        StateName = Name;
        SignalBus.Instance.ReverseBobberMotionEnded += HandleReverseBobberMotionEnded;


        
        
    }

    public override void EnterState(string previousState)
    {
        // On enter, we start the timer
        // Or maybe on enter we add the QTE node to the scene tree (hold this one first)

        // So, on enter, create scene tree timer
        // subscribe to timeout
        // you know what, let's make two nodes, one for controlling the QTE, another for controlling the fishing
        FishingQTE.StartQTE();

    }

    public override void ExitState()
    {

    }

    public override void HandleInput(InputEvent @event)
    {

    }

    public override void ProcessUpdate(double delta)
    {

    }

    public override void PhysicsProcessUpdate(double delta)
    {
        if (Input.IsActionJustPressed("Action") && !FishingQTE.IsActive)
        {
            Bobber.ReverseBobberMotion();
        }
    }

    private void HandleReverseBobberMotionEnded(object sender, EventArgs e)
    {
        OnStateTransitioned("PlayerIdleState");
    }

    
}