using Godot;
using System;
using SignalBusNS;

[GlobalClass]
public partial class FishStateMachine : StateMachine
{
    public override void _Ready()
    {
        base._Ready();
        SignalBus.Instance.AnglingCancelled += HandleAnglingCancelled;
    }

    public void HandleAnglingCancelled(object sender, EventArgs e)
    {
        if (currentState is FishNibbleState)
            HandleStateTransition("FishWanderState");

        GD.Print("Print if angling cancelled");
    }
}