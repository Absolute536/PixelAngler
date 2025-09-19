using Godot;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
// using WorldInfo;

public partial class Player : CharacterBody2D
{
	[Export] AnimatedSprite2D _spriteAnimation;
	[Export] Label DebugText;

	public const float Speed = 50.0f;
	// public const float JumpVelocity = -400.0f;


	// Testing


	StringBuilder stringBuilder = new StringBuilder();
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		// if (!IsOnFloor())
		// {
		// 	velocity += GetGravity() * (float)delta;
		// }

		// Handle Jump.
		// if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		// {
		// 	velocity.Y = JumpVelocity;
		// }

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");

		stringBuilder.Clear();
		// stringBuilder.Append("Directional Vector: " + direction + "\n");
		// Console.WriteLine(direction);

		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
			// stringBuilder.Append("Velocity X: " + velocity.X + "\n");
			// stringBuilder.Append("Velocity Y: " + velocity.Y + "\n");
			// stringBuilder.Append("Position: " + Position + "\n");
			// stringBuilder.Append("Global: " + GlobalPosition);
			var input = new Vector2I((int)Position.X / 16, (int)Position.Y /16);
			// var test = GetTree().CurrentScene.GetNode("World").GetNode("WorldLayer").Call("LocalToMap", input);

			//TESTING!!!
			stringBuilder.Append("Location: " + GetTree().CurrentScene.GetNode("World").Call("WorldCoordToTileType", input));
			switch (direction)
			{
				case Vector2(1, 0):
					_spriteAnimation.Play("walk_right");
					break;
				case Vector2(-1, -0):
					_spriteAnimation.Play("walk_left");
					break;
				case Vector2(0, 1):
					_spriteAnimation.Play("walk_front");
					break;
				case Vector2(0, -1):
					_spriteAnimation.Play("walk_back");
					break;
			}
			// Console.WriteLine("Velocity X: " + velocity.X);
			// Console.WriteLine("Velocity Y: " + velocity.Y);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
			_spriteAnimation.Stop();
		}

		DebugText.Text = stringBuilder.ToString();

		Velocity = velocity;
		MoveAndSlide();
	}
}
