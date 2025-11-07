using Godot;
using System;
using SignalBusNS;
using GamePlayer;

[GlobalClass]
public partial class FishNibbleState : State
{
    [Export] Fish Fish;
    [Export] Area2D InteractionRaidus;
    private Random _random = new Random();

    private int _nibbleCountRequired;
    private int _currentNibbleCount;
    private int _nibbleSpeed;

    private float _forwardAngle;

    private int _reverseDistance;

    // idk if we need a private field _bobber here -> we already can access through fish

    private bool _isActive;
    private bool _isReverse;

    

    public override void _Ready()
    {
        StateName = Name;

        // wait this doesn't work cuz it's in the physics process of state machine
        // let's try remeding with a flag
        // SetPhysicsProcess(false);
        _isActive = false;
        _isReverse = false;

        // SignalBus.Instance.AnglingStarted += HandleAnglingStarted;
        // wait.... it actually won't work
        // because we are instancing the Fish scene during runtime dynamically
        // so.... when the fishing state invokes the event, nothing is subscribed yet
        InteractionRaidus.AreaEntered += OnAreaEntered;
    }

    public override void EnterState(string previousState) // Quick note here regarding the statemachine initial state hehe
    {
        // set a random duration before starting the nibble (or we can have another WAITING/IDLE state?)

        // How to implement nibble?
        // Idea:
        // Use the fish's collision shape
        // Put collision shape to bobber as well
        // If fish collides with bobber, move backwards 
        // So it's like random velocity, move towards bobber --> with randomised velocity
        // Upon collision, move backwards random distance --> also with randomised distance
        // On collide, increment some counter
        // Instead of using a timer for the nibble, we randomise the nibble count, alongside the velocity and push back distance for each nibble
        // That way, it will be more natural(?)


        // GD.Print(Fish.LatchTarget.GlobalPosition);
        // Fish.Position = new Vector2(-32, 20);

        // Fish.BodyEntered += OnFishBodyEntered;
        SceneTreeTimer waitTimer = GetTree().CreateTimer(2.0, true, true); // wait for 2 seconds for now
        waitTimer.Timeout += StartNibble;

        _currentNibbleCount = 0;
        InitialiseNibbleParameters();
        InitialiseForwardNibble(); // test
        // For the other state ideas are as follows:
        // On enter each of those states (the fish behaviour green, yellow, red), randomise a timer duration for the state duration
        // Also upon enter, check for the fish's stamina(?) NOPE ---> we probably don't neet yet, cuz we're planning on using a progress bar, but we'll see
        // For each of those states, we check for the "intended" input and raise events on correct input (like increase progress bar)
        // On full progress bar, raise event to switch fish to caught(?) state and trigger the animations?

        /*
         * REFRESH
         * Nibble behaviour
         * Enter State
         * wait 1 ~ 2.xx seconds before starting to move towards the bobber
         * get random nibble count required (how many times the fish needs to nibble)
         
         * Forward movement 
         * Get direction to bobber, also flip the sprite if needed (for -ve X)
         * Move in a straight line to bobber (I think)
         * Upon area entered for the interaction radius, initiate bounce back movement
         *
         * Bounce Back movment (Reverse)
         * From the forward direction, get the reverse direction by multiply the vector by (-1, -1) right?
         * Rotate the reverse direction by a random angle (doesn't have to be too big)
         * Get random bounce back distance, or rather duration(?) --> in case it reaches the water edge
         * then initiate the bounce back movement
         * when the distance is reached/ set duration is reached, initiate the forward movement again
         * repeat until the current nibble count == nibble count required
         * OR
         * Nibble movement
         * on area entered of the interaction radius, move back a small fixed distance in the opposite forward direction (also multiply by -1, -1)
         * then initiate forward movement again
         * and repeat until nibble count required is reached
         * this is probably easier and more like the "nibble" behaviour(?)
         
         * ALSO
         * need to have some sort of mechanism to ensure only one fish can bite at any time
         * more than one can detect and nibble, but once a fish nibble count required is reached and it makes contact another time, a QTE will start
         * then if success --> lead to bite, and the minigame
         * if success, then other fish will need to transition back to the wander state, if it's currently in nibble (can work like this?)
         * if failure, the failed fish go back to wander state, allowing other fish to "snatch" instead
         * OR if failure, the bait is consumed (BUT THIS KINDA GO AGAINST WHAT WE SAID IN THE GDD) / all go back to wander, and player need to cast again(?)
         * I think we try the first one first

         * AND ALSO
         * For a fish to detect the bobber AND transition to nibble state, the BAIT (or other equipment condition) must match its requirement
         * and again... this can be specified using a custom resource or something (SEE BELOW)
         
         * Additional Ideas
         * Every frame after the initial wait finishes, call LookAt() on the Fish (root) node, so that we orient the head towards the bobber
         * Possible, but need some tinkering with the flipping of the sprite, based on the rotation angle and movement direction
         
         * The movment speed, nibble count, etc (if any) can be unique to each fish species?
         * Use a fish stat custom Resource, and specify these parameters in there (will need to explore more first)
         * Hmm, this sounds good. that way may be able to recognise the species based on the behaviour (TBF, probably can recognise it visually through the outline already)
         * But this introduces some "dynamic?" behaviour (NAH), more like DIFFERENT behaviour for the fish based on its species
         *
         */
    }

    public override void ExitState()
    {

    }

    public override void HandleInput(InputEvent @event)
    {

    }

    public override void ProcessUpdate(double delta)
    {

    }

    public override void PhysicsProcessUpdate(double delta)
    {
        // set movement
        // Tolol
        // Cuz I didn't flip the _isActive flag upon area entered, so that's why the fish
        // continues to move until both origins (bobber and fish) overlaps

        // Use <= so it will nibble once more when current == required
        if (_isActive)
        {
            Vector2 velocity = Vector2.Zero;

            if (_isReverse)
            {
                // movement away from bobber
                
                // umm, get the bounce away from bobber
                // 
            }   
            else
            {
                // movement towards bobber
                // velocity = Fish.Position.DirectionTo(Vector2.Zero) * _nibbleSpeed;
                // Fish.Position = Fish.Position.MoveToward(velocity, 0.5f);
            }
        }

    }

    private void StartNibble()
    {
        // SetPhysicsProcess(true);
        _isActive = true;
        GD.Print("Nibble starts");
    }

    private void InitialiseNibbleParameters()
    {
        Random rand = new Random();
        _nibbleCountRequired = rand.Next(2, 6); // Randomised for now, maybe we can set a nibble count for every fish? (same as nibble velocity)
        _nibbleSpeed = rand.Next(1, 4) * 50;
    }

    // Connected/Subscribed to the signal/event via the editor already
    private void OnAreaEntered(Area2D area)
    {
        if (area is Bobber)
        {
            GD.Print("Bobber entered");
            _currentNibbleCount += 1;
            _isReverse = true;
        }
    }

    private void InitialiseForwardNibble()
    {
        _nibbleSpeed = _random.Next(1, 4) * 50;
        
    }
    
    private void InitialiseReverseNibble()
    {
        
    }
}