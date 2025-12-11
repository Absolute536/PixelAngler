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

    // need is currently active?
    public override void HandleInput(InputEvent inputEvent)
    {
        // Ok, I'm gonna do some band-aid here
        // It's not gonna be pretty, but whatever, we're short on time
        // Should've revamped the control scheme to using Unhandled_input earlier
        // But hey, I tried and abandoned it, and now it's biting me in the FUCKING ASSSSS
        // Dang it. This is ugly af, but whatever, I need to get it done.
        // FishCatalogueUi catalogue = GetNode<FishCatalogueUi>("/root/Main/HUD/FishCatalogue");
        // PauseMenu pauseMenu = GetNode<PauseMenu>("/root/Main/HUD/PauseMenu");

        // // Basically if either one is shown, don't go back to idle yet
        // if (inputEvent.IsActionPressed("ShowCatalogue"))
        // {
        //     // if catalogue is shown, can show pause menu
        //     // if pause is shown, can't show catalogue(?)
        //     if (!pauseMenu.Visible)
        //         OnStateTransitioned("PlayerIdleState");
        // }
        // else if (inputEvent.IsActionPressed("ShowPause"))
        // {
        //     if (!catalogue.Visible)
        //         OnStateTransitioned("PlayerIdleState");
        // }

        // NEVERMIND Just let both pause the game when open I guess
        if (inputEvent.IsActionPressed("ShowCatalogue") || inputEvent.IsActionPressed("ShowPause"))
        {
            if (IsCurrentlyActive)
                OnStateTransitioned("PlayerIdleState");
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