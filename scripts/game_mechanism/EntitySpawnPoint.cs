using Godot;
using SignalBusNS;
using System;
using System.Collections.Generic;
using System.Linq;

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
        SpawnAtFullCapacity();
        SignalBus.Instance.FishCaught += HandleFishCaught;
        SignalBus.Instance.TimeOfDayChanged += HandleTimeOfDayChanged;
    }

    public override void _PhysicsProcess(double delta)
    {
        // it's either we check per physics frame, or we use signal to check after every catch (yeah.)
        // just subscribe to the fish caught event
    }

    private Fish GetRandomFish()
    {
        string spawnLocation = GameInfo.Instance.GetWorldLocation(GlobalPosition);
        List<FishSpecies> spawnList = _fishPoolGenerator.GetRandomFishPool(spawnLocation);

        GD.Print("Spawn location: " + spawnLocation);
        GD.Print("Spawn list size: " + spawnList.Count);
        // 17:37, 07/12/2025 -> I see what's happening now
        // Cuz the randomiser selected a rare rarity, but there are no available "rare" fish that satisfy such condition (AHHHH~~~)
        // So, we use a loop to keep rolling I guess (is this dangerous? probably fine)
        // 11/12/2025, 6:00am: also kinda a duplication, so probably should use the loop to first initialise?
        while (spawnList.Count == 0)
            spawnList = _fishPoolGenerator.GetRandomFishPool(spawnLocation);
        
        int randomChoice = GD.RandRange(0, spawnList.Count - 1); // The 'to' is inclusive, so need to - 1
        FishSpecies chosenSpecies = spawnList[randomChoice];

        // Fish fish = new ();// wait, need to instantiate the scene first probably
        Fish fish = EntityScene.Instantiate<Fish>();
        fish.SpeciesInformation = chosenSpecies;
        fish.CurrentSizeMeasurement = GetRandomSizeMeasurement(chosenSpecies);

        return fish;
    }

    private void SpawnAtFullCapacity()
    {
        // Oh yeah, no need to wait for the fish to be freed, cuz we're using a loop
        for (int i = 0; i < MaxEntityCount; i++)
            AddChild(GetRandomFish()); 
    }

    // hmm, this will be broadcasted to all spawn points, but I suppose it's better than checking per physics frame
    private void HandleFishCaught(object sender, EventArgs e)
    {
        int stillActive = GetChildren().Count;
        // should be 1 cuz if the last fish is caught, there's some delay before it's queued freed, and also if it's hooked there'll still be one left
        // if all fish are caught
        if (stillActive == 1) 
            SpawnAtFullCapacity();
        else
        {
            // 15 to 25 seconds (in-game minutes), probably need to scale properly based on game speed
            SceneTreeTimer spawnTimer = GetTree().CreateTimer(GD.RandRange(15, 25), false, true);
            spawnTimer.Timeout += () =>
            {
                if (GetChildren().Count < MaxEntityCount)
                    AddChild(GetRandomFish());
                // so after the delay, if not at max capacity, spawn the fish
                // if time of day changes afterwards, it'll be despawned
                // if time of day changes before, it would be at max capacity, so the addchild wouldn't be executed.

                // NICE! It works fine now
                // just there's some problem with the disabling of bobber's area, cuz some fish can still detect it (I think it's when yanking out the fish if others are in range)
            };
        }  
    }

    private void HandleTimeOfDayChanged(object sender, EventArgs e)
    {
        // Mark all active fish for despawn
        foreach(Fish spawnedFish in GetChildren().Cast<Fish>())
        {
            spawnedFish.ToDespawn = true;
            // just queue free for now
            // spawnedFish.QueueFree();
        }

        // just spawn max here doesn't work
        // just the call needs to go through
        // maybe just flip a flag or something
        // _toSpawnFullCapacity = true;
        // _currentActiveFishCount = 0;
        SpawnAtFullCapacity();
    }

    private float GetRandomSizeMeasurement(FishSpecies species)
    {
        int size = GD.RandRange((int) (species.MinSizeMeasurement * 100), (int) (species.MaxSizeMeasurement * 100));

        return (float) size / 100; // note: float division, so that it's not integer division and becomes 0
    }

    // So now we need to dynamically spawn the fishes
    // Let's list down the conditions
    /*
     * On Game Start -> spawn a set according to the current time of day
     *
     * Time of day changes, like dawn -> day -> dusk -> night, remove all spawned fishes and respawn a new set
     * - if a fish is hooked when day of time changes, mark it to despawn, but don't remove immediately
     * - only if the player failed to caught the fish, it will despawn afterwards
     * - so we need some flag to determine whether the fish should despawn or not.
     * 
     * All fishes for the spawn point are exhausted -> spawn a new set immediately
     * 
     * A fish is caught
     * -> put the spawn operation in a queue?
     * -> introduce some delay, like 15 -> 25 in-game minutes (can just set it as 15 ~ 25 seconds)
     * -> dequeue and spawn?
     * -> Or just use the one-shot scene tree timer for this.
     * -> but how to handle if the time changes after this?
     * -> probably just flush the queue, or rather reset the variable (like an int singleSpawns)
     * -> queue ---> spawn the fish immediately, but store in a queue, 
                     on one shot timer timeout, enquque and AddChild. Time change, flush queue immediately. (hmm, kinda bad?)
     * -> timer ---> increment an int, wrap spawn operation as timeout action and start timer, 
                     on timeout, execute it and add child straight away, decrement int. Time change, reset int to 0
     */

    // Ok, we redefine everything
    // If no fish left, respawn max
    // If n fish is caught, if not at max capacity yet, respawn one by one until n gradually
    // Ok, time of day changes works 
    // here

}