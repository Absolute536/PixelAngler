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
        // Comment out first cuz haven't made the animation yet
        // player.AnimationPlayer.Play("Idle");
        if (previousState == "PlayerWalkingState")
        {
            Player.AnimationPlayer.Stop();
            Player.Relocate(); // only call when relocate cuz the default is idle and on initialised, player is not loaded yet
        }
        // Probably can call the get tile location function here to get player's location upon idling

        }

    public override void ExitState()
    {
        // Nothing on exit yet
    }

    public override void HandleInput(InputEvent inputEvent)
    {
        // No one-off input
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
        else if (Input.IsActionPressed("Action")) // if I use JustPressed, then we can't trasition from walking to this while holding LMB, wait, maybe we can use presed in walking
        {
            GD.Print(Name + ": " + "Action");

            string currentSelectedItem = Player.SelectedItem;
            switch (currentSelectedItem)
            {
                case "Fishing Rod":
                    OnStateTransitioned("PlayerCastingState");
                    break;
                case "Bug Net":
                    OnStateTransitioned("PlayerNetState");
                    break;
                default:
                    GD.Print("Selected Item Unidentified.");
                    break;
            }

            // OnStateTransitioned("PlayerActionState");
        }

    }

}