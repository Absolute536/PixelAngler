using Godot;
using System;
using SignalBusNS;
using GamePlayer;

[GlobalClass]
public partial class FishNibbleState : State
{
    [Export] Fish Fish;
    [Export] Area2D InteractionRaidus;
    [Export] CollisionShape2D MovementCollisionShape;
    private Random _random = new Random();

    private int _nibbleCountRequired;
    private int _currentNibbleCount;
    private int _nibbleSpeed;

    private Vector2 _targetVelocity = Vector2.Zero;

    // idk if we need a private field _bobber here -> we already can access through fish

    private bool _nibbleActive;
    private bool _isReverse;

    // try and see if these works
    private double _reverseDuration = 0;
    private const double ReverseLimit = 0.5;
    

    public override void _Ready()
    {
        StateName = Name;

        // wait this doesn't work cuz it's in the physics process of state machine
        // let's try remeding with a flag
        // SetPhysicsProcess(false);
        _nibbleActive = false;
        _isReverse = false;

        // SignalBus.Instance.AnglingStarted += HandleAnglingStarted;
        // wait.... it actually won't work
        // because we are instancing the Fish scene during runtime dynamically
        // so.... when the fishing state invokes the event, nothing is subscribed yet
        InteractionRaidus.AreaEntered += OnAreaEntered;
    }

    public override void EnterState(string previousState) // Quick note here regarding the statemachine initial state hehe
    {
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
         * Get random bounce back distance, or rather duration(?) --> in case it reaches the water edge, and let characterbody2d handle the collision
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

         * Now it's like enter nibble state, and just stop
         * So should be enter nibble state, LookAt the bobber, wait random duration, then start the movement
         */

        InitialiseNibbleParameters();
        SceneTreeTimer waitTimer = GetTree().CreateTimer(1 + _random.NextDouble(), false, true); // second false is processAlways, so that if game paused, timer will pause as well (explore this on other timers as well)
        waitTimer.Timeout += () => { _nibbleActive = true; }; // make it active on Timeout
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
        

        // Hol up, maybe we don't really need these?
        // you know, maybe don't do rotation, it jumbles up the pixels
        // how about move to the bobber's Y, then nibble through modifying X?

        // OKAY!!!
        // I see the problem
        // It's because when I flip the sprite when changing direction, all the other collision shape (ESPECIALLY the interaction one) still remain at their original location
        // SO, when we flip the sprite, should also move the collision accordingly
        // ALLRIGHT!
        // It works now
        // polish further by randomising the speed as well
        // and should also play a sound cue for each nibble

        if (_nibbleActive && _currentNibbleCount <= _nibbleCountRequired)
        {
            // use global position on both so we get the normalised direction correctly
            Vector2 directionToTarget = Fish.GlobalPosition.DirectionTo(Fish.LatchTarget.GlobalPosition + new Vector2(0, 16));

            _targetVelocity = directionToTarget * _nibbleSpeed;

            if (_isReverse)
            {
                // https://www.reddit.com/r/godot/comments/1c2yjuj/using_a_timer_instead_of_physics_process/ (might be useful)
                // The premise for this is we accumulate delta value to a variable until it reaches a threshold (ReverseLimit)
                // Since delta the time elapsed since last (physics) frame, we can use this to "create" a timer(?) --> kinda like what we did back in Bobber's movement
                // so while the threshold is not reached (in reverse movement, haven't reach the reverse limit yet), move in the opposite direction from forward movement
                // once the threshold is reached, flip back the flag and reset the duration, and also introduce some delay before stating movement again
                _reverseDuration += delta;
                if (_reverseDuration <= ReverseLimit) // hmm, idk about the equality
                {
                    _targetVelocity *= new Vector2(-1, -1);
                }
                else
                {
                    _isReverse = false; // if limit is reached, flip the reverse flag, so that we move forward again
                    _reverseDuration = 0; // and reset _reverseDuration
                    // and try putting in a wait?
                    StartNibbleDelay(0.5);

                }
            }

            // velocity = velocity.MoveToward(targetVelocity, (float)delta * 500);
            // else
            // {
            //     // movement towards bobber
            //     // velocity = Fish.Position.DirectionTo(Vector2.Zero) * _nibbleSpeed;
            //     // Fish.Position = Fish.Position.MoveToward(velocity, 0.5f);

            //     // just try it first I guess
            //     velocity = directionToTarget * _nibbleSpeed;
            //     velocity = velocity.MoveToward(Vector2.Zero, _nibbleSpeed);

            // }

            // We flip the position of the interaction area as well, to make it consistent (at the snout of the fish)
            AlignFishToMovement();

            Fish.Velocity = _targetVelocity;
            Fish.MoveAndSlide();

            // OK, improvements.
            // Sprite flipping, so that rotation looks correct (maybe no rotation, who knows)
            // Nah. Rotation is important, so that it looks correct
            // and also the nibble speed and count, or rather the parameters that control the nibble behaviour should be randomised on each movement
            // so that it feels more natural
            // AND also need some mechanism to notify the fish and transition back to wander state if they failed to bite (basically sth to transition back to wander state)
        }

    }

    private void InitialiseNibbleParameters()
    {
        // For the parameters, we use fixed values for now
        // May be replaced with unique values based on the FishStat(?) resource in the future
        _nibbleCountRequired = 2;
        _currentNibbleCount = 0;
        _nibbleSpeed = 30;

        _nibbleActive = false; // Idk if we should put this here first?
        // Or we move the other thing out?
        // Cuz they will be uninitialised when the PhysicsProcess() runs, before this function is called
        // CORRECTION, nope. Because a state's EnterState() is called first, before the state machine switches to that state
        // So the values will be initialised before PhysicsProcessUpdate() runs
    }

    // Connected/Subscribed to the signal/event via the editor already
    private void OnAreaEntered(Area2D area)
    {
        if (area is Bobber)
        {
            GD.Print("Bobber entered");
            _currentNibbleCount += 1;
            _isReverse = true;

            if (_currentNibbleCount > _nibbleCountRequired)
                OnStateTransitioned("FishWanderState");
        }
    }

    private void AlignFishToMovement()
    {
        if (!_isReverse)
        {
            // If moving left, adjust the sprite and movement collision shape to face right
            if (_targetVelocity.X < 0)
            {
                Fish.FishSprite.FlipH = true;
                Fish.FishSprite.Offset = new Vector2(8, 0);
                MovementCollisionShape.Position = new Vector2(8, 0);
            }
            // If move right, adjust back to default (as set up in scene editor) 
            else
            {
                Fish.FishSprite.FlipH = false;
                Fish.FishSprite.Offset = new Vector2(-8, 0);
                MovementCollisionShape.Position = new Vector2(-8, 0);
            }
        }
    }

    private void StartNibbleDelay(double duration)
    {
        _nibbleActive = false;
        SceneTreeTimer nibbleDelayTimer = GetTree().CreateTimer(duration, true, true);
        nibbleDelayTimer.Timeout += () => { _nibbleActive = true; };
    }
}