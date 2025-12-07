using Godot;
using System;

[GlobalClass]
public partial class FishDespawnState : State
{
    [Export] public Fish Fish;

    private Color _spriteColor;
    private readonly Color TransparentColor = new Color(1, 1, 1, 0);

    private float _ratio = 0.0f;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        _spriteColor = Fish.FishSprite.Modulate;
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
        Fish.FishSprite.Modulate = _spriteColor.Lerp(TransparentColor, _ratio);
        Fish.Velocity = Fish.Velocity.Normalized() * Fish.SpeciesInformation.MovementSpeed * 0.8f; // don't make it too slow
        Fish.MoveAndSlide();

        _ratio += (float) delta;

        if (Fish.FishSprite.Modulate.IsEqualApprox(TransparentColor))
            Fish.QueueFree();
    }

    public override void ProcessUpdate(double delta)
    {
        
    }
}