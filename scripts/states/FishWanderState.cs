using Godot;
using System;

[GlobalClass]
public partial class FishWanderState : State
{
    [Export] Fish Fish;

    private int _wanderSpeed;
    private float _wanderAngle;
    private Vector2 _wanderDirection;
    private Random _random = new Random();
    private double _wanderDuration;

    [Export] public Timer _movementTimer;

    // private bool _isColliding = false;

    public FishWanderState()
    {
         // randomise on constructor call
    }

    public override void _Ready()
    {
        StateName = Name;
        _movementTimer.Timeout += RandomiseWanderParameters;

        // Fish.BodyEntered += OnBodyEntered;
        // Fish.BodyExited += OnBodyExited;

        RandomiseWanderParameters(); // try calling it here instead
        _movementTimer.Start(_wanderDuration);

    }

    public override void EnterState(string previousState)
    {
        
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

        // Hold up, Player is a CharacterBody, so it works natually, but Fish is an Area2D
        // WTH, I can't get the collision to work properly?!!!

        Vector2 velocity = _wanderDirection.Rotated(_wanderAngle) * _wanderSpeed;
        Fish.Velocity = velocity;

        Fish.MoveAndSlide();
        
    }

    // private void OnBodyEntered(Node2D body)
    // {
    //     _isColliding = true;
    // }

    // private void OnBodyExited(Node2D body)
    // {
    //     _isColliding = false;
    // }

    private void RandomiseWanderParameters()
    {
        _wanderDuration = _random.Next(1, 3) + _random.NextDouble(); // (1 ~ 2) + (0.0 + 1.0) --> so max: 3, min: 1
        _movementTimer.WaitTime = _wanderDuration; // set it here, OR make it OneShot and start it? hmmmm.......

        _wanderAngle = Mathf.DegToRad(45 - _random.Next(91)); // same as last time, -45 to 45 on that direction
        _wanderSpeed = _random.Next(15, 31); // min: 15, max 50, let's try this out first

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


    }

}
