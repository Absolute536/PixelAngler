using Godot;
using System;
using GamePlayer;

[GlobalClass]
public partial class PlayerWalkingState : BaseState
{
    [Export] public CharacterBody2D Player;

    private Player player;

    public PlayerWalkingState()
    {
        StateName = "WALKING";
    }

    public override void _Ready()
    {
        player = (Player) Player;
    }

     public override void EnterState(String previousState)
    {
        // Nothing cuz we need to check the animation on enter first
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
        Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
        Vector2 velocity = player.Velocity;

        // If there is directional input
        if (direction != Vector2.Zero)
        {
            // velocity.X = Mathf.Round(direction.X * Speed);
            // velocity.Y = Mathf.Round(direction.Y * Speed);
            velocity.X = direction.X * player.Speed;
            velocity.Y = direction.Y * player.Speed;

            if (direction == Vector2.Left)
                player.AnimationPlayer.Play("walk_left");
            else if (direction == Vector2.Right)
                player.AnimationPlayer.Play("walk_right");
            else if (direction == Vector2.Up)
                player.AnimationPlayer.Play("walk_back");
            else if (direction == Vector2.Down)
                player.AnimationPlayer.Play("walk_front");

        }
        else
        {
            velocity.X = Mathf.MoveToward(player.Velocity.X, 0, player.Speed);
            velocity.Y = Mathf.MoveToward(player.Velocity.Y, 0, player.Speed);
            InvokeTransitionedEventHandler("IDLE");
            // velocity.X = 0;
            // velocity.Y = 0;
        }

        player.Velocity = velocity;
        player.MoveAndSlide();

        // if (oldInputVector != direction)
		// {
		// 	oldInputVector = direction;
		// 	if (direction != Vector2.Zero)
		// 		GlobalPosition = GlobalPosition.Round();

		// 	Console.WriteLine("After Jitter fix: " + Position);
		// }
		// Console.WriteLine("Position: " + Position);
        

    }
}