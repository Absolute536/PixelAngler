using System;
using GamePlayer;
using Godot;

public partial class Fish : CharacterBody2D
{
    [Export] public Sprite2D FishSprite;
    private Bobber _bobber;
    public Bobber LatchTarget
    {
        set => _bobber = value;
        get => _bobber;
    }


    private Random _random = new Random();

    public override void _Ready()
    {
        Position = new Vector2(_random.Next(4, 13), _random.Next(5, 15));
    }

    public override void _PhysicsProcess(double delta)
    {
        // LookAt(LatchTarget.GlobalPosition); // rotate the +X transform so that it looks at the bobber
        // Transform2D current = GetTransform();
        // if (LatchTarget is not null)
        //     LookAt(LatchTarget.GlobalPosition);
        // maybe can change to if LatchTarget is not null, then proceed with the operations
    }


}