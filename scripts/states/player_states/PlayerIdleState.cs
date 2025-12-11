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
        if (inputEvent.IsActionPressed("Action", true)) // try allowing echo, so that after a delay when holding LMB in walking, it will cast?
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

    }

}