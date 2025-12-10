using Godot;
using System;
using SignalBusNS;
using System.Collections;

[GlobalClass]
public partial class FishWrestleState : State
{
    [Export] public Fish Fish;
    [Export] public Timer MovementTimer;

    private Vector2 _wrestleDirection;

    private const int MinClickCount = 8;
    private const int MaxClickCount = 20;

    private const float WrestleDirectionDuration = 0.5f; // make it a constant, then scale with aggressiveness
    private Random _directionRandomiser = new ();

    private int clicksNeeded;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        
        int lowerBound = (int) (MinClickCount * Fish.SpeciesInformation.Aggressiveness) + 3; // Min: 3 clicks (for min)
        int upperBound = (int) (MaxClickCount * Fish.SpeciesInformation.Aggressiveness) + 5; // Min: 5 clicks (for max)
        clicksNeeded = _directionRandomiser.Next(lowerBound, upperBound + 1); // random for now, can derive from FishInfo later
        GD.Print("Clicks Needed: " + clicksNeeded);

        if (_directionRandomiser.Next(2) == 0)
            _wrestleDirection = Vector2.Left;
        else
            _wrestleDirection = Vector2.Right;
        
        MovementTimer.Timeout += ChangeWrestleDirection;
        MovementTimer.WaitTime = WrestleDirectionDuration * (1 - Fish.SpeciesInformation.Aggressiveness);
        MovementTimer.Start();

        // SignalBus.Instance.OnFishBehaviourChanged(FishBehaviour.Yellow, clicksNeeded);
        MinigameManager.Instance.CurrentBehaviour = FishBehaviour.Yellow;
        MinigameManager.Instance.ClicksNeeded = clicksNeeded;
        
        SignalBus.Instance.FishWrestleCompleted += HandleFishWrestleCompleted;

        SignalBus.Instance.FishCaught += HandleFishCaught;
        SignalBus.Instance.FishLost += HandleFishLost;
    }

    public override void ExitState()
    {
        base.ExitState();
        MovementTimer.Timeout -= ChangeWrestleDirection;
        SignalBus.Instance.FishCaught -= HandleFishCaught;
        SignalBus.Instance.FishLost -= HandleFishLost;
        SignalBus.Instance.FishWrestleCompleted -= HandleFishWrestleCompleted;
    }

    public override void HandleInput(InputEvent @event)
    {
        // no handling input?
        // quick test first
        // if (@event.IsActionPressed("Action"))
        // {
        //     clicksNeeded -= 1;
        // }
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        Fish.Velocity = _wrestleDirection * Fish.SpeciesInformation.MovementSpeed; // 100% speed first?
        Fish.MoveAndSlide();
    }

    public override void ProcessUpdate(double delta)
    {
        // nothing
    }

    private void ChangeWrestleDirection()
    {
        if (_wrestleDirection == Vector2.Left)
            _wrestleDirection = Vector2.Right;
        else
            _wrestleDirection = Vector2.Left;
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
    private void HandleFishLost(object sender, EventArgs e) // remember to place this in all in-game states as well
    {
        if (IsCurrentlyActive)
        {
            Fish.IsHooked = false;
            Fish.LatchTarget.IsLatchedOn = false;
            OnStateTransitioned("FishStartledState");
        }
    }

    private void HandleFishWrestleCompleted(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
            OnStateTransitioned("FishHookedState");
    }
}