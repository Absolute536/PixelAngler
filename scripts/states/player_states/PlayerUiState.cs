using Godot;
using GamePlayer;

[GlobalClass]
public partial class PlayerUiState : State
{
    [Export] public Player Player;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void HandleInput(InputEvent inputEvent)
    {
        // No one-off input
        // just testing
        if (inputEvent.IsActionPressed("ShowCatalogue") || inputEvent.IsActionPressed("ShowPause"))
        {
            if (IsCurrentlyActive)
                OnStateTransitioned("PlayerIdleState"); // should probably create like a player state machinie, and use signal from the catalogue to communicate
        }
    }

    public override void ProcessUpdate(double delta)
    {
        // Nothing per frame
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        

    }
}