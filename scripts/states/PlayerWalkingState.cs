using Godot;
using System;
using GamePlayer;

[GlobalClass]
public partial class PlayerWalkingState : State
{
    [Export] public CharacterBody2D Player;
    [Export] public float MovementSpeed;

    private Player player;
    private Vector2 oldInputVector = Vector2.Zero; // Set old input vector to zeros cuz no input at start???

    // public PlayerWalkingState()
    // {

    // }

    public override void _Ready()
    {
        StateName = Name;
        player = (Player)Player;
    }

    public override void EnterState(String previousState)
    {
        // Nothing cuz we need to check the animation on enter first
        // Hmm, what to do for walking then?
        player.AudioPlayer.Play();
    }

    public override void ExitState()
    {
        // Nothing on exit yet
        player.AudioPlayer.Stop();
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
        // Mostly here
        // Let's just use the previous code and see if it works.

        // Get movement vector for every physics tick
        Vector2 movementVector = Input.GetVector("Left", "Right", "Up", "Down");
        Vector2 velocity = player.Velocity;

        // If there is movement input
        if (movementVector != Vector2.Zero && !Input.IsMouseButtonPressed(MouseButton.Left))
        {
            // velocity.X = Mathf.Round(direction.X * Speed);
            // velocity.Y = Mathf.Round(direction.Y * Speed);

            // Set velocity x and y components to the movement vector * speed
            velocity.X = movementVector.X * MovementSpeed;
            velocity.Y = movementVector.Y * MovementSpeed;

            player.FacingDirection = movementVector;
            PlayWalkingAnimation(movementVector);
        }
        else if (movementVector == Vector2.Zero || Input.IsActionJustPressed("ItemAction"))
        {
            // Use MoveToward for the x & y component of velocity to smooth stopping movement (? look into this further)
            // Cuz it's generated automatically
            // yeah we need these, or else there will be pixel snapping upon stopping the movement
            // Actually there still is, but the effects are less noticable

            // MoveToward moves the "from" towards "to" by the "delta" specified
            // So MoveToward(10, 60, 10) returns 20
            // MoveToward(10, 30, 50) returns 50 - it won't exceed the "to" value

            // velocity.X = Mathf.MoveToward(player.Velocity.X, 0, MovementSpeed);
            // velocity.Y = Mathf.MoveToward(player.Velocity.Y, 0, MovementSpeed);

            velocity = velocity.MoveToward(Vector2.Zero, MovementSpeed);

            // Transition to idle state
            OnTransitionedEventHandler("PlayerIdleState");
            // GD.Print("Transition to idle");
            // velocity.X = 0;
            // velocity.Y = 0;
        }
        // GD.Print("Will print if execute after transitioned to idle");

        // Set player's actual Velocity property to the configured one
        player.Velocity = velocity;
        // Call the player's MoveAndSlide to actually move the node
        player.MoveAndSlide();
        GD.Print("Player Position: " + player.Position);

        // Hmm... Is this needed? Yeah I think we need this

        if (oldInputVector != movementVector)
        {
            oldInputVector = movementVector;
            if (movementVector != Vector2.Zero)
                player.GlobalPosition = player.GlobalPosition.Round();

            Console.WriteLine("After Jitter fix: " + player.Position);
        }
        Console.WriteLine("Position: " + player.Position);

        // Maybe can call the get location here to retrieve player's location when walking
        // But then, fishing is only possible when idling, so probably need to be called once on enter in idle
        // and it should be enough
    }

    private void PlayWalkingAnimation(Vector2 direction)
    {
        if (direction == Vector2.Up)
            player.AnimationPlayer.Play("Up");
        else if (direction == Vector2.Down)
            player.AnimationPlayer.Play("Down");
        else if (direction.X > 0)
            player.AnimationPlayer.Play("Right");
        else
            player.AnimationPlayer.Play("Left");
    }

}