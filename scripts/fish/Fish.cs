using System;
using Godot;
using SignalBusNS;

public partial class Fish : CharacterBody2D
{
    [Export] public Sprite2D FishSprite;

    [Export] public CollisionShape2D MovementCollisionShape;
    [Export] public CollisionShape2D DetectionRadiusShape;
    [Export] public Area2D InteractionRadius;
    [Export] public RayCast2D ObstacleDetectionRaycast;

    [Export] public FishSpecies SpeciesInformation; // make it exported for now, need to assign on instantiation by the spawn point later
    public float CurrentSizeMeasurement {get; set;}

    // For debug
    [Export] public Label DebugText;

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
        set => _isHooked = value;
    }

    private bool _isCaught = false;
    public bool IsCaught
    {
        get => _isCaught;
        set => _isCaught = value;
    }

    private bool _toDespawn = false;
    public bool ToDespawn
    {
        get => _toDespawn;
        set => _toDespawn = value;
    }


    private Random spawnPositionRandomiser = new Random();

    public override void _Ready()
    {
        Position = new Vector2(spawnPositionRandomiser.Next(4, 13), spawnPositionRandomiser.Next(5, 15));
        InitialiseFishParameters(); // try calling it here
    }

    public override void _PhysicsProcess(double delta)
    {
        // LookAt(LatchTarget.GlobalPosition); // rotate the +X transform so that it looks at the bobber
        // Transform2D current = GetTransform();
        // maybe can change to if LatchTarget is not null, then proceed with the operations
        AlignToMovementDirection();
        // GlobalPosition = GlobalPosition.Round();
        // Rounding works, but only when them movement speed is high enough, otherwise the position changed by MoveAndSlide will be rounded back

        // If Fish is hooked, fix the bobber's position at the fish's snout every frame
        // Since bobber's physics process will be halted once it reaches its destination
        // but instead of this, maybe we should just place this within the hooked state?
        if (_isHooked)
        {
            if (FishSprite.FlipH && LatchTarget is not null)
                LatchTarget.GlobalPosition = GlobalPosition + new Vector2(-16, 0); // if flipH, actual position is at tail, so need to offset by -16 on X-axis
            else
                LatchTarget.GlobalPosition = GlobalPosition; // 16 pixel offset upwards because fish's mouth will be at top of bobber without this
                // I see now. Because  the HandleQTESucceeded is broadcasted to every fish, so need to filter
        }

        // This is for if the fish is caught (minigame completed successfully)
        // So we need the fish to follow the bobber's position instead
        // Basically the opposite of isHooked block
        // I KNOW WE'RE POLLING THE STATE EVERY PHYSICS FRAME, BUT IT'LL HAVE TO DO FOR NOW, JUST TO MAKE THINGS FASTER
        if (_isCaught)
        {
            _enableAlignment = false;
            if (LatchTarget is not null)
            {
                if (FishSprite.FlipH)
                    GlobalPosition = LatchTarget.GlobalPosition + new Vector2(16, 0); // if FlipH, fish position at bobber's offset by 16, 16 to ensure mouth is at the bottom
                else
                    GlobalPosition = LatchTarget.GlobalPosition;
            }
        }
    }

    private void InitialiseFishParameters()
    {
        // Debug
        DebugText.Text = SpeciesInformation.SpeciesName;

        Texture2D spriteTexture = SpeciesInformation.SpriteTexture;
        FishSprite.Texture = spriteTexture;
        Vector2 textureSize = spriteTexture.GetSize();
        FishSprite.Offset = new Vector2(-textureSize.X, -textureSize.Y / 2);

        // Adjust the collision shapes
        CapsuleShape2D movementShape = MovementCollisionShape.Shape as CapsuleShape2D;
        CircleShape2D detectionShape = DetectionRadiusShape.Shape as CircleShape2D;

        movementShape.Radius = textureSize.Y;
        movementShape.Height = textureSize.X;
        detectionShape.Radius = textureSize.X - 16 + 48; // 16 wide -> 48, 24 wide -> 56, 32 wide -> 64 I guess it's still fine

    }     

    private void AlignToMovementDirection()
    {
        if (_enableAlignment)
        {
            if (Velocity.X < 0)
            {
                FishSprite.FlipH = true;
                InteractionRadius.Position = Vector2.Left * FishSprite.Texture.GetSize().X;
                ObstacleDetectionRaycast.Position = Vector2.Left * FishSprite.Texture.GetSize().X;
            }
            else if (Velocity.X >= 0)
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

    // Added a public utility method to force the alignment process regardless of EnableAlignment (this is a hacky fix, but whatever)
    public void ForceAlignmentToMovementDirection()
    {
        if (Velocity.X < 0)
        {
            FishSprite.FlipH = true;
            InteractionRadius.Position = Vector2.Left * FishSprite.Texture.GetSize().X;
            ObstacleDetectionRaycast.Position = Vector2.Left * FishSprite.Texture.GetSize().X;
        }
        else if (Velocity.X >= 0)
        {
            FishSprite.FlipH = false;
            InteractionRadius.Position = Vector2.Zero;
            ObstacleDetectionRaycast.Position = Vector2.Zero;
        }
    }
}

public enum FishBehaviour
{
    Green,
    Yellow,
    Red
}