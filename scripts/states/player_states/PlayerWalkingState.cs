using Godot;
using System;
using GamePlayer;
using System.Collections.Generic;
using System.IO;

[GlobalClass]
public partial class PlayerWalkingState : State
{
    [Export] public Player Player;
    [Export] public float MovementSpeed;
    private Vector2 oldInputVector = Vector2.Zero; // Initialise to (0, 0) cuz player is not moving at first (idle)

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(String previousState)
    {
        base.EnterState(previousState);
        // Nothing cuz we need to check the animation on enter first
        // Hmm, what to do for walking then?

        AudioManager.Instance.PlayActionAudio(PlayerActionAudio.Walking);
    }

    public override void ExitState()
    {
        AudioManager.Instance.StopActionAudio(PlayerActionAudio.Walking);
        
        Player.AnimationPlayer.Stop();
        base.ExitState();
    }

    public override void HandleInput(InputEvent inputEvent)
    {
        
    }

    public override void ProcessUpdate(double delta)
    {
        // Nothing per frame
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        // Get movement vector for every physics tick
        Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
        Vector2 velocity = Player.Velocity;

        // If there is movement input
        if (direction != Vector2.Zero)
        {
            // velocity.X = Mathf.Round(direction.X * Speed);
            // velocity.Y = Mathf.Round(direction.Y * Speed);

            // Set velocity x and y components to the movement vector * speed
            velocity.X = direction.X * MovementSpeed;
            velocity.Y = direction.Y * MovementSpeed;

            Player.FacingDirection = direction; // hmm.. assign every frame huh... (dang it! I've got no time left)
            PlayWalkingAnimation(direction);
        }
        // Stop and transition to IDLE when no direction input (update 10/12/2025: only when no directional input)
        else
        {
            // Use MoveToward for the x & y component of velocity to smooth stopping movement (? look into this further)
            // Cuz it's generated automatically
            // yeah we need these, or else there will be pixel snapping upon stopping the movement
            // Actually there still is, but the effects are less noticable

            // MoveToward moves the "from" towards "to" by the "delta" specified
            // So MoveToward(10, 60, 10) returns 20
            // MoveToward(10, 30, 50) returns 30 - it won't exceed the "to" value

            // velocity.X = Mathf.MoveToward(player.Velocity.X, 0, MovementSpeed);
            // velocity.Y = Mathf.MoveToward(player.Velocity.Y, 0, MovementSpeed);

            velocity = velocity.MoveToward(Vector2.Zero, MovementSpeed);

            // Transition to idle state
            OnStateTransitioned("PlayerIdleState");
        }
        // GD.Print("Will print if execute after transitioned to idle");

        // Set player's actual Velocity property to the configured one
        Player.Velocity = velocity;
        // Call the player's MoveAndSlide to actually move the node
        Player.MoveAndSlide();

        // 08/12/2025 try rounding camera
        // Player.PlayerCamera.GlobalPosition = Player.GlobalPosition.Round();

        // GD.Print("Player Position: " + player.Position);

        // Hmm... Is this needed? Yeah I think we need this
        if (HasInputDirectionChanged(direction))
        {
            oldInputVector = direction;
            if (direction != Vector2.Zero)
                Player.GlobalPosition = Player.GlobalPosition.Round();
            GD.Print("Apply Jitter Fix upon Input change"); // Solution from Reddit 
        }

        GD.Print("Player Position: " + Player.GlobalPosition);

        // Maybe can call the get location here to retrieve player's location when walking
        // But then, fishing is only possible when idling, so probably need to be called once on enter in idle
        // and it should be enough
    }

    private void PlayWalkingAnimation(Vector2 direction)
    {
        if (direction == Vector2.Up)
            Player.AnimationPlayer.Play("Up");
        else if (direction == Vector2.Down)
            Player.AnimationPlayer.Play("Down");
        else if (direction.X > 0)
            Player.AnimationPlayer.Play("Right");
        else
            Player.AnimationPlayer.Play("Left");

    }

    private bool HasInputDirectionChanged(Vector2 newDirection)
    {
        return oldInputVector != newDirection;
    }

}