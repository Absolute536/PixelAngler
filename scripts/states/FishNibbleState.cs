using Godot;
using System;
using SignalBusNS;
using GamePlayer;

[GlobalClass]
public partial class FishNibbleState : State
{
    [Export] Fish Fish;
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
        GD.Print("Body entered detected");

        _currentNibbleCount += 1;
        _isReverse = true;
        if (area is Bobber)
            GD.Print("Bobber entered");

        SetPhysicsProcess(false); // try this first
    }

    private void InitialiseForwardNibble()
    {
        _nibbleSpeed = _random.Next(1, 4) * 50;
        
    }
    
    private void InitialiseReverseNibble()
    {
        
    }
}