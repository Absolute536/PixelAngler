using System;
using Godot;
using GamePlayer;
using System.Collections.Generic;

[GlobalClass]
public partial class PlayerIdleState : State
{
    // Property for Idle state here
    [Export] public Player Player;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        
        // Comment out first cuz haven't made the animation yet
        // player.AnimationPlayer.Play("Idle");
        // Probably can call the get tile location function here to get player's location upon idling

    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void HandleInput(InputEvent inputEvent)
    {
        // No one-off input
        // just testing (hahaha......)
        if (inputEvent.IsActionPressed("ShowCatalogue"))
        {
            FishCatalogueUi catalogue = GetNode<FishCatalogueUi>("/root/Main/HUD/FishCatalogue");
            // catalogue.Visible = true;
            // catalogue.FocusMode = Control.FocusModeEnum.All;
            // catalogue.GrabFocus();
            

            // just quick testing (nice)
            // FishRepository.Instance.FishCatchProgress[0] += 1; this works
            catalogue.OpenCatalogue(); // add 1 before open
            OnStateTransitioned("PlayerUiState");
        }
        else if (inputEvent.IsActionPressed("ShowPause"))
        {
            PauseMenu pauseMenu = GetNode<PauseMenu>("/root/Main/HUD/PauseMenu");
            pauseMenu.ShowPauseMenu();
            OnStateTransitioned("PlayerUiState");
            // for this no need to go to ui state, cuz everything is paused?
            // WAIT, then maybe opening catalogue can be like this too?
            // nevermind, we still need the ui state to ensure idle doesn't consume the event
            
        }
        else if (inputEvent.IsActionPressed("Action", true)) // try allowing echo, so that after a delay when holding LMB in walking, it will cast?
        {
            OnStateTransitioned("PlayerCastingState");
        }
    }

    public override void ProcessUpdate(double delta)
    {
        // Nothing per frame
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        // Check input every physics tick
        Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");

        // If has movement input AND Action is not pressed, transition to walking state
        if (direction != Vector2.Zero && !Input.IsActionPressed("Action"))
            OnStateTransitioned("PlayerWalkingState");
        // If no movement input and Action is pressed (continuously as well), transition to action state
        // So that it's gonna be like -> clicked -> action -> start casting or something
        // else if (Input.IsActionPressed("Action")) // if I use JustPressed, then we can't trasition from walking to this while holding LMB, wait, maybe we can use presed in walking
        // {
        //     GD.Print(Name + ": " + "Action");
        //     OnStateTransitioned("PlayerCastingState");
        //     // string currentSelectedItem = Player.SelectedItem;
        //     // switch (currentSelectedItem)
        //     // {
        //     //     case "Fishing Rod":
        //     //         OnStateTransitioned("PlayerCastingState");
        //     //         break;
        //     //     case "Bug Net":
        //     //         OnStateTransitioned("PlayerNetState");
        //     //         break;
        //     //     default:
        //     //         GD.Print("Selected Item Unidentified.");
        //     //         break;
        //     // }

        //     // OnStateTransitioned("PlayerActionState");
        // }

    }

}