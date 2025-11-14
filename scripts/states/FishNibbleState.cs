using Godot;
using System;
using SignalBusNS;
using GamePlayer;

[GlobalClass]
public partial class FishNibbleState : State
{
    [Export] Fish Fish;
    [Export] Area2D InteractionRadius;
    [Export] CollisionShape2D MovementCollisionShape;
    [Export] FishSpecies SpeciesInformation; // probably should be in the root "Fish", but testing for now (YES, it works)
    private Random _random = new Random();

    private int _nibbleCountRequired;
    private int _currentNibbleCount;
    private int _nibbleSpeed;

    private Vector2 _targetVelocity = Vector2.Zero;

    // idk if we need a private field _bobber here -> we already can access through fish

    private bool _nibbleActive;
    private bool _isReverse;
    public bool IsReverse { get => _isReverse; }

    // try and see if these works
    private double _reverseDuration = 0;
    private const double ReverseLimit = 0.25;
    

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
        InteractionRadius.AreaEntered += OnAreaEntered;
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
         
        // Testing disable alignment process in nibble
        // Fish.EnableAlignment = false;
        // How about just setting the velocity to point to bobber before disabling it?
        // it can't
        // but if we only disable it when the fish starts moving (through waitTimer), then it works YEAHHHH
        // So set the velocity on entering nibble state, allowing the sprite to flip WHILE the fish is temporary motionless
        // Then disable it on waitTimer timeout
        Fish.Velocity = GetMovementDirectionTowardTarget();

        SceneTreeTimer waitTimer = GetTree().CreateTimer(_random.Next(1, 3) + _random.NextDouble(), false, true); // second false is processAlways, so that if game paused, timer will pause as well (explore this on other timers as well)
        waitTimer.Timeout += () => 
        { 
            _nibbleActive = true; 
            Fish.EnableAlignment = false; 
        };
    }

    public override void ExitState()
    {
        _nibbleActive = false;
        _isReverse = false;

        Fish.EnableAlignment = true; // Re-enable the sprite alignment process in Fish
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

        if (_nibbleActive && _currentNibbleCount < _nibbleCountRequired)
        {
            // use global position on both so we get the normalised direction correctly
            Vector2 directionToTarget = GetMovementDirectionTowardTarget();
            GD.Print(directionToTarget);

            /*
             * To resolve the sprite flipping issue, it had to be done
             * The issue is caused by the AlignmentProcess in Fish, as it will flip the FishSprite and offset the interaction area according to the movement direction
             *
             * However, since the pathfinding(?) is based on the fish's actual position,
             * when the sprite is flipped, the origin is at the fish's tail
             *
             * So, the account for this, in GetMovementDirection(), I had added an offset of 16 pixels to the right of the bobber's position
             * so that the fish's snout and its interaction area will collide with the bobber (IF the sprite is flipped).
             * Actually the movement still works without this extra step, but the fish will converge towards the left side if we use that method
             * Because when moving towards the bobber from the right, the fish sprite is flipped, and the origin is at the tail
             * So it's quite likely for the fish to "graze" past the bobber, flip immediately and register as nibble,
             * and then proceed with the remaining nibble.
             * It works (sort of), but just looks bad.
             * Actually issue will arise if the bobber's left side is blocked, then the collision will never happen.
             *
             * SO
             * The problem is if the horizontal displacement between the bobber and the fish's position (in nibble state) is too small,
             * The sprite will continuously be flipped, and so does the interaction area, as it tries to move towards the bobber
             * Because in the Alignment process within Fish, if the velocity.X is negative, we flip, else, we don't
             * AND when the horizontal displacement is too small, and say the fish starts facing right (default),
             * The direction towards bobber will have negative X, and so does the velocity.
             * Velocity.X negative, flip the sprite, but this causes direction towards bobber becomes positive X in the next physics frame
             * Positive X, flip back, direction X negative again, and so on....
             * So this continues indefinitely, and the fish's interaction area can't collide with the bobber as well
             *
             * Although hacky, but I guess I have no other choices but to use the check shown below
             * For every movement, we check the direction.X, and the horizontal displacement from the target
             * If the displacement is below a certain threshold, we disable the sprite alignment process
             * Else, we keep the alignment enabled
             * AND this is only done for the forward movement, not reverse
             * P/S: I placed the reverse movement toggle in the OnAreaEntered before, but now we're gonna put it here instead to make things consistent

             * FUCKKKKKKKK
             * Too clunky, need another approach
             */
            _targetVelocity = directionToTarget * _nibbleSpeed;

            // if (!_isReverse)
            // {
            //     if (directionToTarget.Abs().X < 0.25)
            //     {
            //         Fish.EnableAlignment = false;
            //     }
            //     else
            //         Fish.EnableAlignment = true;
            // }
            // else
            //     Fish.EnableAlignment = false;


            if (_isReverse)
            {
                // https://www.reddit.com/r/godot/comments/1c2yjuj/using_a_timer_instead_of_physics_process/ (might be useful)
                // The premise for this is we accumulate delta value to a variable until it reaches a threshold (ReverseLimit)
                // Since delta the time elapsed since last (physics) frame, we can use this to "create" a timer(?) --> kinda like what we did back in Bobber's movement
                // So while the threshold is not reached (in reverse movement, haven't reach the reverse limit yet), move in the opposite direction from forward movement
                // Once the threshold is reached, flip back the flag and reset the duration, and also introduce some delay before stating movement again
                _reverseDuration += delta;
                if (_reverseDuration <= ReverseLimit)
                {
                    // If bobber is on the left, set _targetVelocity to reverse to the right
                    // Conversely, reverse to left instead
                    // Basically, instead of reversing in the opposite direction, we just do left and right
                    // I think this if more like the "nibble" behaviour
                    // Just putting it here in case you (me) wanna switch back: _targetVelocity *= new Vector2(-1, -1);
                    // Additional problem: this does not account for the lack of space if bobber is at water's edge
                    // So is the original one better?

                    // _targetVelocity = directionToTarget.X < 0 ? Vector2.Right * _nibbleSpeed : Vector2.Left * _nibbleSpeed;
                    _targetVelocity *= new Vector2(-1, -1);

                    // Hmm, then again, if we wanna do this, then perhaps it's better to move to Bobber's X, then go with the nibble
                }
                else
                {
                    _isReverse = false; // if limit is reached, flip the reverse flag, so that we move forward again
                    // Fish.EnableAlignment = true; // flip the alignment flag again, so the sprite may align according to movement
                    _reverseDuration = 0; // and reset _reverseDuration
                    // and try putting in a wait?
                    StartNibbleDelay(_random.NextDouble()); // Yeah, so we randomise the delay, not the nibble speed --> but nibble speed can be dependent on the fish itself?

                }
            }

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
        _nibbleCountRequired = 3;
        _currentNibbleCount = 0;
        _nibbleSpeed = SpeciesInformation.MovementSpeed;

        _nibbleActive = false; 
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
                SignalBus.Instance.OnFishBite(this, EventArgs.Empty);

            GD.Print("Fish Nibble!");
        }
    }

    private void StartNibbleDelay(double duration)
    {
        _nibbleActive = false;
        SceneTreeTimer nibbleDelayTimer = GetTree().CreateTimer(duration, true, true);
        nibbleDelayTimer.Timeout += () => { _nibbleActive = true; };
    }

    private Vector2 GetMovementDirectionTowardTarget()
    {
        Vector2 fishPosition = Fish.GlobalPosition;
        Vector2 bobberPosition = Fish.LatchTarget.GlobalPosition;

        // Add a 16 pixel offset to the left (of the target) if the fish is flipped (facing left)
        // Because the actual position will be at the tail, and we want the snout/head to contact with the bobber
        if (Fish.FishSprite.FlipH)
            return fishPosition.DirectionTo(bobberPosition + new Vector2(16, 16));
            // equivalent to (fishPosition + new Vector2(16, 0)).DirectionTo(bobberPosition + new Vector2(0, 16));

        // Problem: the fish sprite will flip left and right continuously if it's somewhat right below the bobber
        // Because if it's right below, it's not gonna be exactly a STRAIGHT down, so there will be some angle
        // If fish faces right, and it's slightly towards the right, direction to gives x as -ve because need to go left, fish flip
        // Once fish flips, direction to gives +ve x instead because need to go right, fish flip again
        // And this repeats indefinitely
        // This only happens if fish is above OR below the bobber, and almost straight up or down


        // If we don't account for the Sprite's flip, it works
        // Just that the calculation will consider the actual position of the Fish node instead, so if fish flips, the actual position is at the tail
        // The fish will home into the bobber centred around the tail, so it will graze past the bobber once (area entered once, but no effect?), then only nibble less than 1 required count
        // Kinda works, but looks bad
        // It actually nibbles the required count as well, but the first one happens when it grazes past, and immediately flips the sprite, so area entered
        // And also it will cause most of the nibble to happen on the left side of bobber because of this
        // Yeah I think we need the check and also need to account for the direction
        return fishPosition.DirectionTo(bobberPosition + new Vector2(0, 16));
    }

    // Next, add a signal to broadcast to fish in nibblestate to go back to wander if bobber is removed
    // Wee added it in the FishStateMachine weee~~~

    private void HandleQTESucceeded(object sender, EventArgs e)
    {
        Fish.LatchTarget.IsLatchedOn = true;
        OnStateTransitioned("FishHookedState");
        
        GD.Print("QTE succeeded");
    }
}