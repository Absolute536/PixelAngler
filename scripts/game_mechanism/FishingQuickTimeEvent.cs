using Godot;
using SignalBusNS;
using System;

[GlobalClass]
public partial class FishingQuickTimeEvent : Node
{
    [Export] public float Duration = 0.7f;

    public bool IsActive { get => _isActive; }
    private bool _isActive;
    private bool _isSuccessful = false;
    private string _actionName = "Action";

    public delegate void QuickTimeEventEndedEventHander();
    // public event QuickTimeEventEndedEventHander SuccessfulQuickTimeEventEnded;
    // public event QuickTimeEventEndedEventHander FailureQuickTimeEventEnded;

    public override void _Ready()
    {
        // SignalBus.Instance.FishBite += StartQTE;
    }

    public void StartQTE(string inputActionName)
    {
        _actionName = inputActionName;
        _isActive = true;
        _isSuccessful = false;

        SceneTreeTimer sceneTreeTimer = GetTree().CreateTimer(Duration, true, true);
        sceneTreeTimer.Timeout += EndQTE;
        
    }

    public void EndQTE()
    {
        if (_isActive)
        {
            if (_isSuccessful)
            {
                // raise success event
                SignalBus.Instance.OnQTESucceeded(this, EventArgs.Empty);
                GD.Print("Successful Reaction");
            }
            else
            {
                // raise failure event
                SignalBus.Instance.OnQTEFailed(this, EventArgs.Empty);
                GD.Print("Unsuccessful Reaction");
            }
        } 
        
        _isActive = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        // if (_isActive && Input.IsActionJustPressed("Action"))
        // {
        //     _isSuccessful = true;
        //     EndQTE();
        // }
    }

    // Actually can use _unhandled input for this I think, because it's only one-time per QTE
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(_actionName) && _isActive)
        {
            _isSuccessful = true;
            GD.Print("Success input in minigame");
            EndQTE();
        }
    }
}