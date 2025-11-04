using System;
using GamePlayer;
using Godot;

public partial class Fish : Area2D
{
    private Bobber _bobber;
    public Bobber LatchTarget
    {
        set => _bobber = value;
        get => _bobber;
    }

    private Player _player;
    public Player Player
    {
        set => _player = value;
        get => _player;
    }

    private Random _random = new Random();

    public Fish()
    {
        Visible = false;
    }

    public override void _Ready()
    {
        // GD.Print("RayCast Result: " + LatchTarget.CheckFishSpawnLocation(Position)); // true if is colliding -> not valid location

        // Ohhh, I know, because we only check a single direction, not reverse
        // Ok, instead of using player's facing direction, how about we randomly choose up, down, left and right?
        // and if we go through all 4, just abort it (need error handling afterwards)
        // I mean it will only happen if the bobber is in a extremely small pool I suppose (hmm, there might be more delays though)
        // And looping like this is never good, because the app will freeze, while it is looping
        // hmm, we it freeze after we change direction?
        // oh yeah, should also adjust the actual spawn to be 1 or 2 tiles inwards compared to the raycasted position

        // Ok, so the problem is the collision information is not updated yet when CheckFishSpawnLocation() is called
        // The intersection is only calculated every physics frame, and the information will be held until the next physics frame
        // So....., this means if first check returns true, the execution will be suspended within the loop, and nothing else can be executed, because it is single-threaded?
        // OHHH, and also position won't be updated until we exit the loop (does that matter?)
        // I suppose my theory is right
        // but more accurately, 
        // OK. So the initial target position of the raycast is downwards (0, 50)
        // So the collision info is held and calling IsColliding() returns true, EVEN AFTER we reassign a new target position,
        // because at the physics frame the method is called, the raycast information STILL uses the initial (0, 50)
        // SOOOOO, the only way is to use ForceRayCastUpdate() to immediately update the raycast information without having to wait for the next physics frame
        // alright, so it works now, and a spawn position can be searched using the while loop

        // ok, so we probably need a terminate condition now to account for small patches of water (or we could just not placed that sort of environment)
        // probably would be better if we allow reel back as well (but the while loop forbids it)

        Vector2 initialSpawnPosition = InitialiseSpawnPosition();
        if (initialSpawnPosition != Vector2.Zero)
            Position = initialSpawnPosition;
        
        Visible = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        LookAt(LatchTarget.GlobalPosition); // rotate the +X transform so that it looks at the bobber
        Transform2D current = GetTransform();

        // maybe we can rotate it back a little bit, making it not as "Extreme"
        // refer back to the godot doc for adjusting transform matrix
        // idea, if transform.X.X < 0, flip the sprite instead (vertically?)
    }

    // OK, so it works, just lacking adaptability and a fail condition
    private Vector2 InitialiseSpawnPosition()
    {
        Vector2 spawnPosition;

        do
        {
            int distance = _random.Next(2, 5) * 16; // 1 to 4 tiles

            float angle = Mathf.DegToRad(45 - _random.Next(90)); // -45 to 45

            spawnPosition = Player.FacingDirection.Rotated(angle) * distance; // spawn position at [angle] from bobber and at [distance] tiles away

            GD.Print("InitialiseSpawnPosition Call");
            GD.Print("Distance (tiles): " + distance / 16);
            GD.Print("Angle (Degree): " + Mathf.RadToDeg(angle));
            GD.Print("Spawn Position: " + spawnPosition + "\n");

        } while (LatchTarget.IsSpawnPositionInvalid(spawnPosition));

        return spawnPosition;
    }

    private void CheckSpawnningDirection()
    {
        bool valid = true;
        for (int tileOffset = 1; tileOffset < 5 && valid; tileOffset++)
        {
            Vector2 potentialPosition = LatchTarget.GlobalPosition + (Player.FacingDirection * 16 * tileOffset);
            valid = GameInfo.Instance.GetTileType(potentialPosition) == GameWorld.TileType.Water;
            // So it needs to be water on all offsets from the bobber
        }
    }
}