using Godot;
using System;

[GlobalClass]
public partial class FishNibbleState : State
{
    [Export] Fish Fish;

    private int _totalNibbleCount;
    private int _currentNibbleCount;
    private int _nibbleSpeed;

    private Bobber _bobber;

    public override void _Ready()
    {
        SetPhysicsProcess(false);
    }

    public override void EnterState(string previousState)
    {
        // set a random duration before starting the nibble (or we can have another WAITING/IDLE state?)

        // How to implement nibble?
        // Idea:
        // Use the fish's collision shape
        // Put collision shape to bobber as well
        // If fish collides with bobber, move backwards 
        // So it's like random velocity, move towards bobber --> with randomised velocity
        // Upon collision, move backwards random distance --> also with randomised distance
        // On collide, increment some counter
        // Instead of using a timer for the nibble, we randomise the nibble count, alongside the velocity and push back distance for each nibble
        // That way, it will be more natural(?)

        // On enter, should also retrieve a reference to the bobber
        // WAIT
        // the bobber always exist, just hidden
        

        // Fish.BodyEntered += OnFishBodyEntered;
        SceneTreeTimer waitTimer = GetTree().CreateTimer(2.0, true, true); // wait for 2 seconds for now
        waitTimer.Timeout += StartNibble;

        _currentNibbleCount = 0;
        InitialiseNibbleParameters();

        // For the other state ideas are as follows:
        // On enter each of those state, randomise a timer duration for the state duration
        // Also upon enter, check for the fish's stamina(?) NOPE ---> we probably don't neet yet, cuz we're planning on using a progress bar, but we'll see
        // For each of those states, we check for the "intended" input and raise events on correct input (like increase progress bar)
        // On full progress bar, raise event to switch fish to caught(?) state and trigger the animations?
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

    }

    private void StartNibble()
    {
        SetPhysicsProcess(true);
    }

    private void InitialiseNibbleParameters()
    {
        Random rand = new Random();
        _totalNibbleCount = rand.Next(2, 6); // Randomised for now, maybe we can set a nibble count for every fish? (same as nibble velocity)
        _nibbleSpeed = rand.Next(1, 4) * 50;
    }

    // Connected/Subscribed to the signal/event via the editor already
    private void OnFishBodyEntered(Node2D area)
    {

    }

}