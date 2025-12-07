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

    private bool _toSpawnFullCapacity = false;
    private int _currentActiveFishCount = 0;

    // OK idea, once fish is hooked, change it to TopLevel (if needed actually, let's not first)

    public override void _Ready()
    {
        // Try spawnning on ready for now, should move this call to some event handler of a timer (in-game time) later
        SpawnMax();
        SignalBus.Instance.FishCaught += HandleFishCaught;
        SignalBus.Instance.TimeOfDayChanged += HandleTimeOfDayChanged;
    }

    public override void _PhysicsProcess(double delta)
    {
        // it's either we check per physics frame, or we use signal to check after every catch (yeah.)
        // just subscribe to the fish caught event

        // just testing here
        // if (GetChildren().Count == 0) // cuz we check 0, so that's why it won't spawn if one is hooked
        // {
        //     SpawnMax();
           
        //     // _toSpawnFullCapacity = false;
        // }
    }

    private Fish SpawnFish()
    {
        string spawnLocation = GameInfo.Instance.GetWorldLocation(GlobalPosition);
        List<FishSpecies> spawnList = _fishPoolGenerator.GetRandomFishPool(spawnLocation);

        GD.Print("Spawn location: " + spawnLocation);
        GD.Print("Spawn list size: " + spawnList.Count);
        // 17:37, 07/12/2025 -> I see what's happening now
        // Cuz the randomiser selected a rare rarity, but there are no available "rare" fish that satisfy such condition (AHHHH~~~)
        // So, we use a loop to keep rolling I guess (is this dangerous? probably fine)
        while (spawnList.Count == 0)
            spawnList = _fishPoolGenerator.GetRandomFishPool(spawnLocation);
        
        int randomChoice = GD.RandRange(0, spawnList.Count - 1); // The 'to' is inclusive, so need to - 1
        FishSpecies chosenSpecies = spawnList[randomChoice];

        // Fish fish = new ();// wait, need to instantiate the scene first probably
        Fish fish = EntityScene.Instantiate<Fish>();
        fish.SpeciesInformation = chosenSpecies;

        return fish;
    }

    private void SpawnMax()
    {
        // Oh yeah, no need to wait for the fish to be freed, cuz we're using a loop
        // if (GetChildren().Count == 0)
        // {
            
        // }
        for (int i = 0; i < MaxEntityCount; i++)
            AddChild(SpawnFish());
    
        _currentActiveFishCount = MaxEntityCount;
        
    }

    // hmm, this will be broadcasted to all spawn points, but I suppose it's better than checking per physics frame
    private void HandleFishCaught(object sender, EventArgs e)
    {
        if (GetChildren().Count == 0)
        {
            
        }
    }

    private void HandleTimeOfDayChanged(object sender, EventArgs e)
    {
        // Despawn all
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
        _currentActiveFishCount = 0;
        SpawnMax();
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

}