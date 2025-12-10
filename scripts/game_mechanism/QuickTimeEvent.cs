using Godot;
using System;
using SignalBusNS;

public partial class QuickTimeEvent : Node
{
    public static QuickTimeEvent Instance {get; private set;}

    private float _duration;
    private string _inputActionName;

    public bool IsActive {get => _isActive;}
    private bool _isActive;
    private bool _isSuccessful;

    public override void _Ready()
    {
        Instance = this;
        _isActive = false;
        _isSuccessful = false;
    }

    public void StartQuickTimeEvent(float duration, string actionName)
    {
        _duration = duration;
        _inputActionName = actionName;
        _isActive = true;
        _isSuccessful = false;

        SceneTreeTimer reactionTimer = GetTree().CreateTimer(_duration, false, true);
        reactionTimer.Timeout += EndQuickTimeEvent;
    }

    public void EndQuickTimeEvent()
    {
        if (_isActive)
        {
            if  (_isSuccessful)
            {
                // raise successful event
                SignalBus.Instance.OnQTESucceeded(this, EventArgs.Empty);
            }
            else
            {
                // raise failure event
                SignalBus.Instance.OnQTEFailed(this, EventArgs.Empty);
            }
        }

        _isActive = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (_isActive && @event.IsActionPressed(_inputActionName))
        {
            _isSuccessful = true;
            EndQuickTimeEvent();
            GetViewport().SetInputAsHandled(); // does this work?
        }
    }
}