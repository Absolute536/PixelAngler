using Godot;
using System;
using System.Collections.Generic;

public partial class EntitySpawnPoint : Node2D
{
    // It's called a spawn "POINT", so we just spawn the fishes at that POINT
    // Or at most 1 tile around the POINT
    // No need for scanning an area or what not
    // https://www.reddit.com/r/godot/comments/vjge0n/could_anyone_share_some_code_for_finding_a/ (for circle)

    [Export] public int MaxEntityCount { set; get; }
    [Export] public PackedScene EntityScene { set; get; }

    private FishPoolGenerator _fishPoolGenerator = new ();

    // OK idea, once fish is hooked, change it to TopLevel (if needed actually, let's not first)

    public override void _Ready()
    {
        // Try spawnning on ready for now, should move this call to some event handler of a timer (in-game time) later
        SpawnEntity();
    }

    private void SpawnMax()
    {
        for (int i = 0; i < MaxEntityCount; i++)
        {
            string spawnLocation = GameInfo.Instance.GetWorldLocation(GlobalPosition);
            List<FishSpecies> spawnList = _fishPoolGenerator.GetRandomFishPool(spawnLocation);
            
            int randomChoice = GD.RandRange(0, spawnList.Count);
            FishSpecies chosenSpecies = spawnList[randomChoice];
        }
    }

    private Fish SpawnFish()
    {
        string spawnLocation = GameInfo.Instance.GetWorldLocation(GlobalPosition);
        List<FishSpecies> spawnList = _fishPoolGenerator.GetRandomFishPool(spawnLocation);
        
        int randomChoice = GD.RandRange(0, spawnList.Count);
        FishSpecies chosenSpecies = spawnList[randomChoice];

        Fish fish = new ();
        fish.SpeciesInformation = chosenSpecies;

        return fish;
    }

    private void SpawnEntity()
    {
        while (GetChildren().Count < MaxEntityCount) // oops, sprite 2d is alrady the child hehe
            AddChild(EntityScene.Instantiate());
    }

    // So now we need to dynamically spawn the fishes
    // Let's list down the conditions
    /*
     * On Game Start -> spawn a set according to the current time of day
     *
     * Day of Time changes, like dawn -> day -> dusk -> night, remove all spawned fishes and respawn a new set
     * - if a fish is hooked when day of time changes, mark it to despawn, but don't remove immediately
     * - only if the player failed to caught the fish, it will despawn afterwards
     * - so we need some flag to determine whether the fish should despawn or not.
     * 
     * All fishes for the spawn point are exhausted -> spawn a new set immediately
     * 
     * A fish is caught
     * -> put the spawn operation in a queue?
     * -> introduce some delay, like 20 -> 40 in-game minutes (can just set it as 20 ~ 40 seconds)
     * -> dequeue and spawn?
     * -> Or just use the one-shot scene tree timer for this.
     * -> but how to handle if the time changes after this?
     * -> probably just flush the queue, or rather reset the variable (like an int singleSpawns)
     * -> queue ---> spawn the fish immediately, but store in a queue, 
                     on one shot timer timeout, enquque and AddChild. Time change, flush queue immediately.
     * -> timer ---> increment an int, wrap spawn operation as timeout action and start timer, 
                     on timeout, execute it and add child straight away, decrement int. Time change, reset int to 0
     */

}