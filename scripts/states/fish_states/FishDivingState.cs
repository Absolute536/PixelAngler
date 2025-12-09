using Godot;
using SignalBusNS;
using System;

[GlobalClass]
public partial class FishDivingState : State
{
    [Export] public Fish Fish;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        Fish.Velocity = Vector2.Zero; // not moving
        // change sprite to the diving thingy or animation

        MinigameManager.Instance.CurrentBehaviour = FishBehaviour.Red;

        SceneTreeTimer divingTimer = GetTree().CreateTimer(3.0, false, true);
        divingTimer.Timeout += () =>
        {
            if (IsCurrentlyActive)
                OnStateTransitioned("FishHookedState");
        };

        SignalBus.Instance.FishCaught += HandleFishCaught;
        SignalBus.Instance.FishLost += HandleFishLost;
    }

    public override void ExitState()
    {
        base.ExitState();
        SignalBus.Instance.FishCaught -= HandleFishCaught;
        SignalBus.Instance.FishLost -= HandleFishLost;
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

    private void HandleFishLost(object sender, EventArgs e) // remember to place this in all in-game states as well
    {
        if (IsCurrentlyActive)
        {
            Fish.IsHooked = false;
            Fish.LatchTarget.IsLatchedOn = false;
            OnStateTransitioned("FishStartledState");
        }
    }
}