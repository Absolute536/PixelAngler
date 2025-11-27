using Godot;
using System;
using SignalBusNS;
using System.Collections;

[GlobalClass]
public partial class FishWrestleState : State
{
    [Export] public Fish Fish;
    [Export] public Timer MovementTimer;
    private double _directionDuration;
    private Random _directionRandomiser = new ();

    private int clicksNeeded;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        // ChangeDirection();
        // MovementTimer.Timeout += ChangeDirection;
        // MovementTimer.Start();

        // SceneTreeTimer wrestleTimer = GetTree().CreateTimer(3.0, false, true);
        // wrestleTimer.Timeout += () =>
        // {
        //     if (IsCurrentlyActive)
        //         OnStateTransitioned("FishHookedState");
        // };
        clicksNeeded = _directionRandomiser.Next(3, 8);
        GD.Print("Clicks Needed: " + clicksNeeded);
        SignalBus.Instance.OnFishBehaviourChanged("Action");
        SignalBus.Instance.FishCaught += HandleFishCaught;
    }

    public override void ExitState()
    {
        base.ExitState();
        MovementTimer.Stop();
        // MovementTimer.Timeout -= ChangeDirection;

        SignalBus.Instance.FishCaught -= HandleFishCaught;
    }

    public override void HandleInput(InputEvent @event)
    {
        // no handling input?
        // quick test first
        if (@event.IsActionPressed("Action"))
        {
            clicksNeeded -= 1;
        }
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        // Fish.MoveAndSlide();
        if (clicksNeeded <= 0)
            OnStateTransitioned("FishHookedState");
    }

    public override void ProcessUpdate(double delta)
    {
        // nothing
    }

    private void ChangeDirection()
    {
        _directionDuration = _directionRandomiser.Next(1, 3) + _directionRandomiser.NextDouble();
        MovementTimer.WaitTime = _directionDuration;

        Vector2 movementDirection = Vector2.Zero;
        string action;

        int direction = _directionRandomiser.Next(4);
        switch (direction)
        {
            case 0:
                movementDirection = Vector2.Up;
                action = "Up";
                break;
            case 1:
                movementDirection = Vector2.Right;
                action = "Right";
                break;
            case 2:
                movementDirection = Vector2.Left;
                action = "Left";
                break;
            case 3:
            default:
                movementDirection = Vector2.Down;
                action = "Down";
                break;
        }

        // movementDirection = movementDirection.Rotated(Mathf.DegToRad(_directionRandomiser.Next(361))); // turn 0 ~ 360 degrees
        Fish.Velocity = movementDirection * 35; // fix movement speed as 35 for now

        
        // if (movementDirection.Y > 0.5)
        //     action = "Up";
        // else if (movementDirection.Y < -0.5)
        //     action = "Down";
        // else
        // {
        //     if (movementDirection.X >= 0)
        //         action = "Right";
        //     else
        //         action = "Left";
        // }

        SignalBus.Instance.OnFishBehaviourChanged(action);
    }

    private void HandleFishCaught(object sender, EventArgs e)
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