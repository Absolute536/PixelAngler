using Godot;
using SignalBusNS;
using System;

[GlobalClass]
public partial class FishWanderState : State
{
    [Export] public Fish Fish;
    [Export] public Timer MovementTimer;
    [Export] public Area2D DetectionRadius;
    [Export] public CollisionShape2D MovementCollisionShape;

    private double _wanderSpeed;
    private float _wanderAngle;
    private Vector2 _wanderDirection;
    private double _wanderDuration;
    private Vector2 _velocity = Vector2.Zero;
    
    private Random _random = new Random();

    

    // private bool _isColliding = false;

    public FishWanderState()
    {
         // randomise on constructor call
    }

    public override void _Ready()
    {
        StateName = Name;
        MovementTimer.Timeout += RandomiseWanderParameters;

        // Fish.BodyEntered += OnBodyEntered;
        // Fish.BodyExited += OnBodyExited;

        RandomiseWanderParameters(); // try calling it here instead
        MovementTimer.Start(_wanderDuration);

        DetectionRadius.AreaEntered += HandleBobberEntered;

    }

    public override void EnterState(string previousState)
    {
        // ok, so here if come from nibble state, disable the detection for some duration, then re-enable it back
        if (previousState == "FishNibbleState" || previousState == "FishHookedState") // or other game state?
        {
            DetectionRadius.Monitoring = false;
            SceneTreeTimer disableTimer = GetTree().CreateTimer(3.0f, true, true); // try 3 seconds first
            disableTimer.Timeout += () => { DetectionRadius.Monitoring = true; };
        }
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
        // wandering behaviour
        // randomly select a direction and speed (+ movement angle I guess, so that it won't be just 4 directions only)
        // also need a timer to set the duration of moving via the randomised parameters
        // maybe also need to track direction, namely if left to right (or vice versa), flip the sprite horizontally (vertical flip is for rotation)
        // I suppose we can skip the rotation

        _velocity = _wanderDirection.Rotated(_wanderAngle) * (float) _wanderSpeed;
        Fish.Velocity = _velocity;
        
        Fish.MoveAndSlide();
    }

    private void RandomiseWanderParameters()
    {
        _wanderDuration = _random.Next(1, 2) + _random.NextDouble(); // (1 ~ 2) + (0.0 + 1.0) --> so max: 3, min: 1
        MovementTimer.WaitTime = _wanderDuration; // set it here, OR make it OneShot and start it? hmmmm.......

        _wanderAngle = Mathf.DegToRad(45 - _random.Next(91)); // same as last time, -45 to 45 on that direction
        _wanderSpeed = _random.Next(15, 51); // min: 15, max 50, let's try this out first

        // well, this I can probably use an array of constant and store it, but whatever, it'll do for now
        int randomDirectionDraw = _random.Next(4);

        if (randomDirectionDraw == 0)
            _wanderDirection = Vector2.Up;
        else if (randomDirectionDraw == 1)
            _wanderDirection = Vector2.Right;
        else if (randomDirectionDraw == 2)
            _wanderDirection = Vector2.Left;            
        else
            _wanderDirection = Vector2.Down;

        // MovementRaycast.TargetPosition = _wanderDirection.Rotated(_wanderAngle) * 32; // try 2 tile first
        // MovementRaycast.ForceRaycastUpdate();

        // yeah it kinda works, but lacking some polish
        // most importantly, flip horizontally when changing direction


        // use manual raycast instead of collision
        // raycast one tile ahead
        // if collided, change direction & update raycast direction as well
        // then physics frame condition, just check raycast iscolliding

        // just idea
        // rng when spawnning -> determine rarity
        // rng when hooking -> if bait type fits a fish of the rarity (?)
        // when biting -> multiple can nibble, but only one bite? (kinda difficult), OR when area entered, wait random sec until nibble, and only one can lock one
        // rng when catching -> all the conditions from the narrowed down pool? (or we just spawn rarity and do everything here?)

        // rectify idea
        // all fish spawn with the same sprite (dark colour) and shape
        // the rng is determined only when the fish is hooked (along with the conditions)
        // that way, the surprise factor remains
        // OR
        // we just straight away spawn the entire fish sprite, but with the dark shader
        // the rng is determined by the spawn point during spawn cycle
        // and we spawn periodically, or rather reset the spawned pool of fish in the water body every X in-game hour
        // especially during significant time of day change (morning -> afternoon, sth like that)
        // and note that we need some sort of spawn animation to make it blend naturally
        // Also if we go with this, then need to account for different sizes, cuz bigger fish = bigger sprite, so need bigger collision shape

    }

    private void HandleBobberEntered(Area2D area)
    {
        // Only respond if area is bobber
        // AND also some other conditions (later)
        if (area is Bobber)
        {
            Fish.LatchTarget = area as Bobber;
            GD.Print(GetParent().GetParent().Name + ":  Bobber Detected");

            // We transition to nibble state straight away, and have the nibble state wait 1.x seconds upon entering the state
            OnStateTransitioned("FishNibbleState");

        }
    }
}
