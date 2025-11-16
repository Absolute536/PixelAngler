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
        SignalBus.Instance.QTESucceeded += SuccessQTE;
    }

    public override void EnterState(string previousState)
    {
        if (previousState == "FishNibbleState")
        {
            SignalBus.Instance.OnFishBite(this, EventArgs.Empty);
        }
    }

    public override void ExitState()
    {
        
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

    private void SuccessQTE(object sender, EventArgs e)
    {
        GD.Print("QTE success");
    }
}