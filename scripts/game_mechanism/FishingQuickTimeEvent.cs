using Godot;
using System;

[GlobalClass]
public partial class FishingQuickTimeEvent : Node
{
    [Export] public float Duration;

    public bool IsActive { get => _isActive; }
    private bool _isActive;
    private bool _isSuccessful = false;

    public delegate void QuickTimeEventEndedEventHander();
    // public event QuickTimeEventEndedEventHander SuccessfulQuickTimeEventEnded;
    // public event QuickTimeEventEndedEventHander FailureQuickTimeEventEnded;

    public void StartQTE()
    {
        SceneTreeTimer sceneTreeTimer = GetTree().CreateTimer(Duration, true, true);
        sceneTreeTimer.Timeout += EndQTE;
        _isActive = true;
        _isSuccessful = false;
    }

    public void EndQTE()
    {
        _isActive = false;

        if (_isSuccessful)
        {
            // raise success event
            GD.Print("Successful Reaction");
        }

        if (!_isSuccessful)
        {
            // raise failure event
            GD.Print("Unsuccessful Reaction");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isActive && Input.IsActionJustPressed("Action"))
        {
            _isSuccessful = true;
            EndQTE();
        }
    }
}