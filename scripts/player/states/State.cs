using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public abstract partial class State : Node, StateAction
{
    public delegate void TransitionedEventHandler(string nextState);
    public event TransitionedEventHandler StateTransitioned;

    public string StateName { get; set; }

    protected virtual void OnStateTransitioned(string nextState)
    {
        StateTransitioned?.Invoke(nextState);
    }

    public abstract void EnterState(string previousState);

    public abstract void ExitState();

    public abstract void HandleInput(InputEvent @event);

    public abstract void ProcessUpdate(double delta);

    public abstract void PhysicsProcessUpdate(double delta);
    
}