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

    public override void _Ready()
    {
        player = (Player)Player;
    }

    public override void EnterState(String previousState)
    {
        // Nothing cuz we need to check the animation on enter first
        // Hmm, what to do for walking then?
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
        // Mostly here
        // Let's just use the previous code and see if it works.

        // Get movement vector for every physics tick
        Vector2 movementVector = Input.GetVector("Left", "Right", "Up", "Down");
        Vector2 velocity = player.Velocity;

        // If there is movement input
        if (movementVector != Vector2.Zero)
        {
            // velocity.X = Mathf.Round(direction.X * Speed);
            // velocity.Y = Mathf.Round(direction.Y * Speed);

            // Set velocity x and y components to the movement vector * speed
            velocity.X = movementVector.X * MovementSpeed;
            velocity.Y = movementVector.Y * MovementSpeed;

            // Play corresponding animation (hmm.. should it be here?)
            if (movementVector == Vector2.Left)
                player.AnimationPlayer.Play("walk_left");
            else if (movementVector == Vector2.Right)
                player.AnimationPlayer.Play("walk_right");
            else if (movementVector == Vector2.Up)
                player.AnimationPlayer.Play("walk_back");
            else if (movementVector == Vector2.Down)
                player.AnimationPlayer.Play("walk_front");
        }
        else
        { 
            // Use MoveToward for the x & y component of velocity to smooth stopping movement (? look into this further)
            // Cuz it's generated automatically
            velocity.X = Mathf.MoveToward(player.Velocity.X, 0, MovementSpeed);
            velocity.Y = Mathf.MoveToward(player.Velocity.Y, 0, MovementSpeed);

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

        if (oldInputVector != movementVector)
		{
			oldInputVector = movementVector;
			if (movementVector != Vector2.Zero)
				player.GlobalPosition = player.GlobalPosition.Round();

			Console.WriteLine("After Jitter fix: " + player.Position);
		}
		Console.WriteLine("Position: " + player.Position);
        

    }
}