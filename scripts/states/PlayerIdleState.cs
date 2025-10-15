using System;
using Godot;
using GamePlayer;
using System.Collections.Generic;

[GlobalClass]
public partial class PlayerIdleState : State
{
    // Property for Idle state here
    [Export] public CharacterBody2D Player;

    private Player player;

    // public PlayerIdleState()
    // {
        
    // }

    public override void _Ready()
    {
        StateName = Name;
        player = (Player)Player;
    }

    public override void EnterState(string previousState)
    {
        // Comment out first cuz haven't made the animation yet
        // player.AnimationPlayer.Play("Idle");
        if (previousState == "PlayerWalkingState")
        {
            player.AnimationPlayer.Stop();
            player.Relocate(); // only call when relocate cuz the default is idle and on initialised, player is not loaded yet
        }

        // Probably can call the get tile location function here to get player's location upon idling

        }

    public override void ExitState()
    {
        // Nothing on exit yet
    }

    public override void HandleInput(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("Left") || inputEvent.IsActionPressed("Right") || inputEvent.IsActionPressed("Up") || inputEvent.IsActionPressed("Down"))
        {
            OnStateTransitioned("PlayerWalkingState");
        }
        else if (inputEvent.IsActionPressed("Action"))
        {
            OnStateTransitioned("PlayerActionState");
        }

    }

    public override void ProcessUpdate(double delta)
    {
        // Nothing per frame
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        /*
         * If no directional input and no "Action" input
         * - transition to walking state
         * - we check for the LMB because we don't want the state transition to boomerang between IDLE and WALKING
         * - since if you hold wasd, the first condition will be true -> transition to WALKING
         * - then in WALKING "Action" is still pressed -> transition to IDLE
         *
         * If ONLY "Action" is pressed, transition to ACTION state
         */

        // Check input every physics tick
        // Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");

        // // If has movement input, transition to walking state
        // if (direction != Vector2.Zero && !Input.IsActionPressed("Action"))
        //     OnStateTransitioned("PlayerWalkingState");
        // // If not movement input and ItemAction is clicked, transition to fishing state(?)
        // else if (Input.IsActionPressed("Action"))
        // {
        //     GD.Print(Name + ": " + "Action");
        //     OnStateTransitioned("PlayerActionState"); // ~ like this? or a fishing state?
        // }

    }

}