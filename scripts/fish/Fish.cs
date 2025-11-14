using System;
using Godot;
using SignalBusNS;

public partial class Fish : CharacterBody2D
{
    [Export] public Sprite2D FishSprite;
    [Export] public Area2D InteractionRadius;
    [Export] public RayCast2D ObstacleDetectionRaycast;

    private Bobber _bobber;
    public Bobber LatchTarget
    {
        set => _bobber = value;
        get => _bobber;
    }

    private Vector2 _movementDirection;
    public Vector2 MovementDirection
    {
        set => _movementDirection = value;
        get => _movementDirection;
    }

    private bool _enableAlignment = true;
    public bool EnableAlignment
    {
        set => _enableAlignment = value;
        get => _enableAlignment;
    }

    private bool _isHooked = false;
    public bool IsHooked
    {
        get => _isHooked;
        private set => _isHooked = value;
    }


    private Random _random = new Random();

    public override void _Ready()
    {
        Position = new Vector2(_random.Next(4, 13), _random.Next(5, 15));
        SignalBus.Instance.QTESucceeded += HandleQTESucceeded;
    }

    public override void _PhysicsProcess(double delta)
    {
        // LookAt(LatchTarget.GlobalPosition); // rotate the +X transform so that it looks at the bobber
        // Transform2D current = GetTransform();
        // maybe can change to if LatchTarget is not null, then proceed with the operations
        AlignToMovementDirection();

        if (_isHooked)
        {
            if (FishSprite.FlipH && LatchTarget is not null)
                LatchTarget.GlobalPosition = GlobalPosition + new Vector2(-16, -16); // if flipH, actual position is at tail, so need to offset by -16 on X-axis
            else
                LatchTarget.GlobalPosition = GlobalPosition + new Vector2(0, -16); // 16 pixel offset upwards because fish's mouth will be at top of bobber without this
        }
    }
            

    private void AlignToMovementDirection()
    {
        if (_enableAlignment)
        {
            if (Velocity.X < 0)
            {
                FishSprite.FlipH = true;
                InteractionRadius.Position = Vector2.Left * 16;
                ObstacleDetectionRaycast.Position = Vector2.Left * 16;
            }
            else if (Velocity.X > 0)
            {
                FishSprite.FlipH = false;
                InteractionRadius.Position = Vector2.Zero;
                ObstacleDetectionRaycast.Position = Vector2.Zero;
            }
        }
        // BUGFIX
        // Instead of repositioning the collision shape, which will cause the fish to "jolt"
        // We just reposition the interaction radius instead, it works.... somewhat
        // maybe this can be a method in the fish's physics process?
        // ok I know why
        // because the fish's origin is still at the "tail"
        // and only the interaction radius is reposition, so the fish will move until the origin reaches the designated point instead
        // soooo.... does that mean when the sprite is flipped, the end position will need to += (16, 0) ?
        // yes. you're right
        // Extracted from FishNibbleState (12/11/2025)
    }

    private void HandleQTESucceeded(object sender, EventArgs e)
    {
        IsHooked = true;
    }


}