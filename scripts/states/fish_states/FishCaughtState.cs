using Godot;
using System;

[GlobalClass]
public partial class FishCaughtState : State
{
    [Export] public Fish Fish;
    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        Fish.Velocity = Vector2.Zero;
        Fish.FishSprite.Material = null;
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
        // maybe can put the caught motion (fish follow bobber here instead)
    }

    public override void ProcessUpdate(double delta)
    {
        
    }
}