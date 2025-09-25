namespace GamePlayer;

using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using WorldInfo;
// using WorldInfo;

public class PositionArgs : EventArgs
{
	public Vector2I WorldCoordinate { get; }
	public TileType Type { set; get; }
	public PositionArgs(Vector2I worldCoordinate, TileType type)
	{
		WorldCoordinate = worldCoordinate;
		Type = type;
	}
}

public partial class Player : CharacterBody2D
{
	[Export] AnimatedSprite2D _spriteAnimation;
	[Export] Label DebugText;

	public const float Speed = 50.0f;
	// public const float JumpVelocity = -400.0f;

	private Vector2 oldPosition;
	// Testing
	public override void _Ready()
	{
		oldPosition = Position;
	}

	public delegate TileType PositionChangedEventHandler(Vector2I worldCoordinate);
	public PositionChangedEventHandler PositionChange;

	private void Relocate()
	{
		DebugText.Text = PositionChange(new Vector2I((int)Position.X / 16, (int)Position.Y / 16)).ToString();
	}



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

			/*
			 * The root node of the player is at the same Position as the World node in the main scene
			 * So, the Position of the player and its Global Position is the same (idk if it's really like that)
			 * So the Position of the playe can be rather naturally translated to the coordinates in the world tile map layer
			 * A division of 16 pixel is needed because each tile is 16 by 16 pixels in the tile map layer
			 * Player sprite is also 16 pixels wide
			 * 1 pixel displacement in the world tile map = 16 pixel displacement in the global scene
			 * Opposite: 1 displacement in global scene = 1 / 16 displacement in world tile map coordinate
			 * So need to divide the Position by 16 to map the global coordinate to the internal coordinate of the tile map layer
			 * Then we can use the internal coordinate to obtain tile information, etc...
			 */

			// var input = new Vector2I((int)Position.X / 16, (int)Position.Y / 16);
			// var test = GetTree().CurrentScene.GetNode("World").GetNode("WorldLayer").Call("LocalToMap", input);

			//TESTING!!!
			// stringBuilder.Append("Location: " + GetTree().CurrentScene.GetNode("World").Call("WorldCoordToTileType", input));

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

		// DebugText.Text = stringBuilder.ToString();
		if (!oldPosition.IsEqualApprox(Position))
		{
			Relocate();
			oldPosition = Position;
			Console.WriteLine("Position Changed.");
		}
		Velocity = velocity;
		MoveAndSlide();

		CastLine();
	}

	List<Vector2> polyPoints = [new Vector2(0, 0)];
	Line2D l = new();

	private void CastLine()
	{

		


		if (Input.IsActionJustPressed("ItemAction"))
		{
			l.Name = "FishingLine";
			l.Width = 1.0f;
			l.DefaultColor = Colors.White;
			l.Points = polyPoints.ToArray();
			AddChild(l);
			Console.WriteLine("FIshing Line created on pressed");

		}

		int count = 1;
		if (Input.IsActionPressed("ItemAction"))
		{
			// Line2D line = new Line2D();
			// Vector2[] points = [new Vector2(0, 0), new Vector2(10.0f, -10.0f), new Vector2(20.0f, -20.0f), new Vector2(30.0f, -10.0f), new Vector2(40.0f, -5.0f), new Vector2(-10.0f, 10.0f)];
			// line.Points = points;
			// line.DefaultColor = Colors.White;
			// line.Width = 1.0f;
			// line.JointMode = Line2D.LineJointMode.Round;
			// AddChild(line);

			polyPoints.Add(new Vector2(count, -count));
			Line2D fishingLine = GetNode<Line2D>("FishingLine");
			fishingLine.Points = polyPoints.ToArray();
			count++;
			Console.WriteLine("Fishing Line extending on hold");

		}

		if (Input.IsActionJustReleased("ItemAction"))
		{
			// var fish = GD.Load<PackedScene>("res://scenes/Fish.tscn");
			// CharacterBody2D fishInstance = fish.Instantiate() as CharacterBody2D;
			// fishInstance.Position = l.Points[l.Points.Length - 1];
			// AddChild(fishInstance);
		}

	}
}
