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
        // MovementTimer.Timeout += ChangeBehaviour;
        MovementTimer.Start(1.0); // every 3s?
        // SignalBus.Instance.OnFishBehaviourChanged(FishBehaviour.Green, 0);
        MinigameManager.Instance.CurrentBehaviour = FishBehaviour.Green;

        SignalBus.Instance.FishCaught += HandleFishCaught;
        // PackedScene progressBarScene = GD.Load<PackedScene>("res://scenes/fishing_progress.tscn");
        // MarginContainer progressBarRoot = progressBarScene.Instantiate<MarginContainer>();
        // FishingProgressBar progressBar = progressBarRoot.GetChild(0) as FishingProgressBar; // fix later :P
        // // GD.Print("Size: " + progressBar.Size); // size is 0? (cuz only controlled by parent?)
        // // progressBar.Size = new Vector2(40, 3);
        // progressBarRoot.Position = new Vector2(-8, 8);

        // Fish.AddChild(progressBarRoot);
        // MinigameManager.Instance.StartMinigame(progressBarRoot);

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
    }

    public override void ExitState()
    {
        base.ExitState();
        // set it like this for now, may be different later (need to wait until animation finished)
        MovementTimer.Timeout -= ChangeMovementAngle;
        MovementTimer.Stop();
        SignalBus.Instance.FishCaught -= HandleFishCaught;
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
        _movementAngle = Mathf.DegToRad(90 - _movementRandomiser.Next(181));
    }

    private void ChangeBehaviour()
    {
        // SignalBus.Instance.OnFishBehaviourChanged("Interact");
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

}

