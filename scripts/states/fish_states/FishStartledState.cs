using Godot;
using System;

[GlobalClass]
public partial class FishStartledState : State
{
    [Export] Fish Fish;
    [Export] Area2D DetectionRadius;
    [Export] RayCast2D ObstacleDetectionRaycast;

    private double _duration;
    private int _movementSpeed;
    private Random _random = new ();

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        
        DetectionRadius.Monitoring = false;
        _movementSpeed = _random.Next(30, 51); // 30 ~ 50?
        _duration = _random.Next(1, 3) + _random.NextDouble(); // 1.xx to 2.00

        SceneTreeTimer startleTimer = GetTree().CreateTimer(_duration, false, true);
        startleTimer.Timeout += EndStartleState;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        // GD.Print(Fish.Velocity);
        // Fish.Velocity = Fish.Velocity.Normalized()  * -1 * _movementSpeed; // move in opposite direction from nibble with the specified movementspeed

        // because at the last physics frame when the fish is nibbling, its velocity will be 0, as DirectionTo(Bobber's position) will be 0, 0
        // instead, let's try using the raycast's target position instead as use its opposite
        Fish.Velocity = ObstacleDetectionRaycast.TargetPosition.Normalized() * -1 * _movementSpeed; // yes, it works

        Fish.MoveAndSlide();
    }

    public override void ProcessUpdate(double delta)
    {
        
    }

    public void EndStartleState()
    {
        DetectionRadius.Monitoring = true;
        OnStateTransitioned("FishWanderState");
    }
}