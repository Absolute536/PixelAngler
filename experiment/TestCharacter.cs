using Godot;
using System;

public partial class TestCharacter : CharacterBody2D
{
	public const float Speed = 30f;
	public const float JumpVelocity = -400.0f;

	[Export] Node2D Bobber;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		velocity = GlobalPosition.DirectionTo(Bobber.GlobalPosition + new Vector2(0, 16)) * Speed;

		Velocity = velocity;
		MoveAndSlide();
	}
}
