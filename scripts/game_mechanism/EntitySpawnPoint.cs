using Godot;
using System;

public partial class EntitySpawnPoint : Node2D
{
    // It's called a spawn "POINT", so we just spawn the fishes at that POINT
    // Or at most 1 tile around the POINT
    // No need for scanning an area or what not
    // https://www.reddit.com/r/godot/comments/vjge0n/could_anyone_share_some_code_for_finding_a/ (for circle)

    [Export] public int MaxEntityCount { set; get; }
    [Export] public PackedScene EntityScene { set; get; }

    // OK idea, once fish is hooked, change it to TopLevel (if needed actually, let's not first)

    public override void _Ready()
    {
        // Try spawnning on ready for now, should move this call to some event handler of a timer (in-game time) later
        SpawnEntity();
    }

    private void SpawnEntity()
    {
        while (GetChildren().Count - 1 < MaxEntityCount) // oops, sprite 2d is alrady the child hehe
            AddChild(EntityScene.Instantiate());
    }

}