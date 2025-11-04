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
                velocity = Fish.Position.DirectionTo(Vector2.Zero) * _nibbleSpeed;
                Fish.Position = Fish.Position.MoveToward(velocity, 0.5f);
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

        // wait, need to initialise a distance first and check the position
        // random distance based on facing direction as well,
        // left: +x
        // right: -x
        // up: -y
        // down: +y
        // so get distance, and check the area with the dist as the radius, and bobber as centre
        // if area contains grass, reduce the distance
        // repeat until area is only water OR if distance < some threshold, just flip to the other direction and repeat the process
        // you know what, instead of the entire area, let's just check the area encompassed by the angle
        // so..... we need angle first (dang it again)




        // angle?
        // _forwardAngle = Fish.GlobalPosition.AngleToPoint(Fish.LatchTarget.GlobalPosition);
        // calculate base on player's facing direction
        // left: 90 deg + 45 + rand(0 ~ 90)
        // right: rand(-45 ~ 45)
        // up: -45 - rand(0 ~ 90)
        // down: 45 + rand(0 ~ 90)
        // wait, this is all relative FROM bobber, not the fish (dang it!!!)
        // just use it first I guess
        // if (Fish.Player.FacingDirection == Vector2.Up)
        //     _forwardAngle = Mathf.DegToRad(-_random.Next(45, 136)); // - (45 ~ 135)
        // else if (Fish.Player.FacingDirection == Vector2.Down)
        //     _forwardAngle = Mathf.DegToRad(45 + _random.Next(91));
        // else if (Fish.Player.FacingDirection == Vector2.Left)
        //     _forwardAngle = Mathf.DegToRad(135 + _random.Next(91));
        // else
        //     _forwardAngle = Mathf.DegToRad(45 - _random.Next(91)); // 45 - (0 ~ 90) --> so 45 to -45

        // AngleToPoint is (To - From).angle
        // _forwardAngle is the angle from bobber to fish
        // so we use a rotation matrix?

        // Hmm, if we use Rotated(), then the angle wouldn't be originated from the x-axis
        // let try a different way

        // *******************************************************************************
        // _forwardAngle = Mathf.DegToRad(45 - _random.Next(91)); // 45 - (0 ~ 90)

        // int dist = _random.Next(1, 3) * 16;
        // float x = Fish.LatchTarget.GlobalPosition.X;
        // float y = Fish.LatchTarget.GlobalPosition.Y;
        // ******************************************************************************

        // Vector2 potentialPosition = new Vector2(x * Mathf.Cos(_forwardAngle) - y * Mathf.Sin(_forwardAngle), x * Mathf.Sin(_forwardAngle) + y * Mathf.Cos(_forwardAngle));

        // ***********************************************
        // Vector2 potentialPosition = Fish.LatchTarget.GlobalPosition + (Fish.Player.FacingDirection.Rotated(_forwardAngle) * dist);
        // GD.Print("Bobber: " + Fish.LatchTarget.GlobalPosition);
        // GD.Print("Potential: " + potentialPosition);
        // GD.Print("Forward Angle: " + _forwardAngle);
        // GD.Print("Forward Angle (Deg): " + Mathf.RadToDeg(_forwardAngle));
        // *************************************************

        // GD.Print("To verify: " + Fish.LatchTarget.GlobalPosition.AngleToPoint(potentialPosition));
        // GD.Print("To verify (Deg): " + Mathf.RadToDeg(Fish.LatchTarget.GlobalPosition.AngleToPoint(potentialPosition) - Fish.LatchTarget.GlobalPosition.Angle()));

        // Hold that thought
        // instead of using the bobber's vector (global position), we use the player's facing direction instead
        // then +45 ~ -45 for each direction?
        // so it's bobber position + potential (direction) * distance?
        GD.Print("\n");
        // GD.Print("Verify: " + potentialPosition.AngleToPoint(Fish.LatchTarget.GlobalPosition));
        // Yes. It works.
        // But there's an idea. Should I make the sprite rotate as well, and make it so that it can spawn in any angle around bobber?
        // OK. It can work, just use look at
        // So.... we follow this
        // spawn in at location (as usual)
        // rotated such that the snout/mouth/head/whatever points towards the bobber position (using LookAt())
        // then proceed with the nibble on timeout.
        

        
    }
    
    private void InitialiseReverseNibble()
    {
        
    }
}