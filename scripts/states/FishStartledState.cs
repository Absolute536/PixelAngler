using Godot;
using System;

[GlobalClass]
public partial class FishStartledState : State
{
    [Export] Fish Fish;
    [Export] Area2D DetectionRadius;

    private double _duration;
    private int _movementSpeed;
    private Random _random = new ();

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        DetectionRadius.Monitoring = false;
        _movementSpeed = _random.Next(30, 51); // 30 ~ 50?
        _duration = 1 + _random.NextDouble(); // 1.xx to 2.00

        SceneTreeTimer startleTimer = GetTree().CreateTimer(_duration, true, true);
        startleTimer.Timeout += EndStartleState;
    }

    public override void ExitState()
    {
        
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        Fish.Velocity = Fish.Velocity.Normalized()  * -1 * _movementSpeed; // move in opposite direction from nibble with the specified movementspeed
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