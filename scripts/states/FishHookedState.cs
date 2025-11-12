using Godot;
using System;

[GlobalClass]
public partial class FishHookedState : State
{
    [Export] public Fish Fish;

    public override void _Ready()
    {
        StateName = Name;
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
        Fish.LatchTarget.GlobalPosition = Fish.GlobalPosition + new Vector2(0, -16);

        Vector2 velocity = Vector2.Left * 10;
        Fish.Velocity = velocity;
        Fish.MoveAndSlide();
    }

}

