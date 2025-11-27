using Godot;
using System;
using SignalBusNS;

[GlobalClass]
public partial class FishAttractedState : State
{
    [Export] Fish Fish;
    
    private int _movementSpeed = 8; // set as 15 so that it moves really slowly?
    private Vector2 _movementDirection;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void _ExitTree()
    {
        SignalBus.Instance.AnglingCancelled -= RevertToDefaultState;
        SignalBus.Instance.FishBite -= RevertToDefaultState;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);

        SignalBus.Instance.AnglingCancelled += RevertToDefaultState;
        SignalBus.Instance.FishBite += RevertToDefaultState;

        // Similar to startle state, we set some timer here
        _movementDirection = Fish.GlobalPosition.DirectionTo(Fish.LatchTarget.GlobalPosition + new Vector2(0, 16)); // adjust later

        // Force alignment to flip the fish if necessary while EnableAlignment is false
        Fish.Velocity = _movementDirection;
        Fish.ForceAlignmentToMovementDirection();

        // only move the Y component, move away if < 2 tiles
        // yeah, make it like this FOR NOW
        _movementDirection.X = 0;
        if (Fish.GlobalPosition.DistanceTo(Fish.LatchTarget.GlobalPosition + new Vector2(0, 16)) < 32) // less than 2 tiles?
            _movementDirection.Y *= -1;
        
     
        // screw it, so if close (< 2 tiles), move away
        // else, move towards
        // THAT'S ALL!
        // should we account for the flipH too?

        Fish.EnableAlignment = false; // actually can try ditching force alignment and use a timer as before with the isActive flag
        // let's not first

        SceneTreeTimer attractedTimer = GetTree().CreateTimer(2.0f, false, true);
        attractedTimer.Timeout += EndAttractedState;
    }

    public override void ExitState()
    {
        base.ExitState();
        SignalBus.Instance.AnglingCancelled -= RevertToDefaultState;
        SignalBus.Instance.FishBite -= RevertToDefaultState;
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        Fish.Velocity = _movementDirection * _movementSpeed;
        Fish.MoveAndSlide();
    }

    public override void ProcessUpdate(double delta)
    {
        
    }

    private void EndAttractedState()
    {
        if (IsCurrentlyActive)
            OnStateTransitioned("FishNibbleState");
    }

    private void RevertToDefaultState(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
            OnStateTransitioned("FishWanderState");
    }
}