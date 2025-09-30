using System;
using Godot;
using GamePlayer;

[GlobalClass]
public partial class PlayerIdleState : BaseState
{
    // Property for Idle state here
    [Export] public CharacterBody2D Player;

    private Player player;

    public PlayerIdleState()
    {
        StateName = "IDLE";
    }

    public override void _Ready()
    {
        player = (Player) Player;
    }

    public override void EnterState(string previousState)
    {
        // Comment out first cuz haven't made the animation yet
        // player.AnimationPlayer.Play("Idle");
        if (previousState == "WALKING")
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
        Vector2 movementVector = Input.GetVector("Left", "Right", "Up", "Down");
        if (movementVector != Vector2.Zero)
            InvokeTransitionedEventHandler("WALKING");
    }

}