using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class State : Node, StateAction
{
    public delegate void TransitionedEventHandler(string nextState);
    public event TransitionedEventHandler StateTransitioned;

    protected bool IsCurrentlyActive = false;

    public string StateName { get; set; }

    protected virtual void OnStateTransitioned(string nextState)
    {
        StateTransitioned?.Invoke(nextState);
    }

    public virtual void EnterState(string previousState)
    {
        IsCurrentlyActive = true;
    }

    public virtual void ExitState()
    {
        IsCurrentlyActive = false;
    }

    public abstract void HandleInput(InputEvent @event);

    public abstract void ProcessUpdate(double delta);

    public abstract void PhysicsProcessUpdate(double delta);
    
}