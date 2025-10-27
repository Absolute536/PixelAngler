using Godot;
using SignalBusNS;

[GlobalClass]
public partial class PlayerFishingState : State
{
    [Export] public Bobber Bobber;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        // On entering fishing state
        // cast the bobber + fishing line
        // well, not really, we need to cast it on release
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
        if (Input.IsActionJustPressed("Action"))
        {
            if (Bobber.InWater)
                Bobber.ReverseBobberMotion();

            OnStateTransitioned("PlayerIdleState");
        }
    }
}