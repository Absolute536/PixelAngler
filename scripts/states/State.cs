using Godot;
using System;

[GlobalClass]
public abstract partial class State : Node, StateAction
{
    public delegate void Transitioned(string nextState);
    public event Transitioned TransitionedEventHandler;

    public string StateName { get; set; }

    protected virtual void OnTransitionedEventHandler(string nextState)
    {
        TransitionedEventHandler?.Invoke(nextState);
    }

    public abstract void EnterState(string previousState);

    public abstract void ExitState();

    public abstract void HandleInput(InputEvent @event);

    public abstract void ProcessUpdate(double delta);

    public abstract void PhysicsProcessUpdate(double delta);
    
}