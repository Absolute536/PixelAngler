using System;
using Godot;
using GamePlayer;

[GlobalClass]
public partial class PlayerIdleState : State
{
    // Property for Idle state here
    [Export] public CharacterBody2D Player;

    private Player player;
    
    public override void _Ready()
    {
        player = (Player)Player;
    }

    public override void EnterState(string previousState)
    {
        // Comment out first cuz haven't made the animation yet
        // player.AnimationPlayer.Play("Idle");
        if (previousState == "PlayerWalkingState")
            player.AnimationPlayer.Stop();
    }

    public override void ExitState()
    {
        // Nothing on exit yet
    }

    public override void HandleInput(InputEvent inputEvent)
    {
        // Nothing here
    }

    public override void ProcessUpdate(double delta)
    {
        // Nothing per frame
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        // Check input every physics tick
        Vector2 movementVector = Input.GetVector("Left", "Right", "Up", "Down");

        // If has movement input, transition to walking state
        if (movementVector != Vector2.Zero)
            OnTransitionedEventHandler("PlayerWalkingState");
        // If not movement input and ItemAction is clicked, transition to fishing state(?)
        else if (Input.IsActionJustPressed("ItemAction"))
        {
            GD.Print(Name + ": " + "ItemAction");
            OnTransitionedEventHandler("PlayerActionState"); // ~ like this? or a fishing state?
        }
    }

}