using GamePlayer;
using Godot;
using SignalBusNS;
using System;

[GlobalClass]
public partial class FishHookedState : State
{
    [Export] public Fish Fish;
    [Export] public Timer MovementTimer;

    private Player _player;
    private Random _movementRandomiser = new ();
    private Vector2 _movementDirection;
    private float _movementAngle;

    public override void _Ready()
    {
        StateName = Name;
    }
    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);

        Fish.EnableAlignment = true;
        Fish.IsHooked = true;
        Fish.LatchTarget.IsLatchedOn = true;

        _player = GetTree().GetFirstNodeInGroup("Player") as Player; // ohhh, oooor we make player information as an autoload? (hmm~ or is it a hassle?)

        _movementDirection = Fish.GlobalPosition.DirectionTo(_player.GlobalPosition);
        _movementDirection *= -1;
        ChangeMovementAngle();
        // make the direction constant on enter state?

        MovementTimer.Timeout += ChangeMovementAngle;
        MovementTimer.Start(1.0); // every 3s?
        // SignalBus.Instance.OnFishBehaviourChanged(FishBehaviour.Green, 0);
        MinigameManager.Instance.CurrentBehaviour = FishBehaviour.Green;

        SignalBus.Instance.FishCaught += HandleFishCaught;
        SignalBus.Instance.FishLost += HandleFishLost;

        /*
         * ImageTexture
         * Get the Image used in the ImageTexture
         * GetSize() -> Vector2I of (width, height)
         * To centre the progress bar under fish, position of progressbar root should be at
         * - Image's width / 2
         * 
         */

        // Hooked -> Green
        // Dive -> Red
        // Wrestle -> Yellow
        // Separate states
        // Always start with green

        // this duration depends on the Aggressiveness of the fish
        SceneTreeTimer greenTimer = GetTree().CreateTimer(3.0, false, true);
        greenTimer.Timeout += () =>
        {
            if (IsCurrentlyActive)
            {
                Random random = new ();
                if (random.Next(2) == 1) // simple 0 or 1 for now (or maybe keep it, just modify the duration of each based on fish aggressiveness)
                    OnStateTransitioned("FishWrestleState"); // go to wrestle state for now, randomise later
                else
                    OnStateTransitioned("FishDivingState");
            }
                
        };

        // aggressiveness
        // a value between 0 ~ 1
        // for those with duration, we want > aggro = < duration (so 1 - aggroValue)
        // for those with clicks (YELLOW), we want to generate a random value based on the aggro, with > aggro = > clicks required
    }

    public override void ExitState()
    {
        base.ExitState();
        // set it like this for now, may be different later (need to wait until animation finished)
        MovementTimer.Timeout -= ChangeMovementAngle;
        MovementTimer.Stop();
        SignalBus.Instance.FishCaught -= HandleFishCaught;
        SignalBus.Instance.FishLost -= HandleFishLost;
    }

    public override void HandleInput(InputEvent @event)
    {

    }

    public override void ProcessUpdate(double delta)
    {
        // No per frame update
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        // Fish.LatchTarget.GlobalPosition = Fish.GlobalPosition + new Vector2(0, -16);

        // just use global position regardless of the flip for now, it would matter that much actually
        // Vector2 movementDirection = Fish.GlobalPosition.DirectionTo(_player.GlobalPosition); 
        // movementDirection *= -1; // reverse it

        // float movementAngle = Mathf.DegToRad(90 - _movementRandomiser.Next(181)); // -90 to 90 degree

        Fish.Velocity = _movementDirection.Rotated(_movementAngle) * 50; // movement speed fixed as 40 first;
        Fish.MoveAndSlide();
    }

    private void ChangeMovementAngle()
    {
        _movementAngle = Mathf.DegToRad(90 - _movementRandomiser.Next(181)); // -90 ~ 90 degree
        // ok, so for this one, we want to prevent it from going out of the camera's view
        // Or... we can just make the camera follow the fish (ugh)
        
        // I guess we need some sort of boundary, facing left or right, (x = ...., a vertical line)
        // then for up or down, (y = ....,, a horizontal line)
        // so like, can move away if within bound (camera.Position + screen width / 2 AND - screen width / 2), same for height
        // so, move in any direction, if reached boundary, move in opposite direction

    }

    private void HandleFishCaught(object sender, EventArgs e) // since this is duplicated across the three state, maybe can extract into an interface?
    {
        if (IsCurrentlyActive)
        {
            Fish.IsHooked = false;
            Fish.LatchTarget.IsLatchedOn = false;
            Fish.IsCaught = true;
            OnStateTransitioned("FishCaughtState");
        }
    }

    private void HandleFishLost(object sender, EventArgs e) // remember to place this in all in-game states as well
    {
        if (IsCurrentlyActive)
        {
            Fish.IsHooked = false;
            Fish.LatchTarget.IsLatchedOn = false;
            OnStateTransitioned("FishStartledState");
        }
    }

}

