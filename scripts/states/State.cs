using System;
using Godot;

[GlobalClass]
public abstract partial class State : Node
{
    public delegate void Transitioned(string nextState);
    public event Transitioned TransitionedEventHandler;
    public abstract void HandleInput(InputEvent inputEvent);

    public abstract void ProcessUpdate(double delta);

    public abstract void PhysicsProcessUpdate(double delta);

    public abstract void EnterState(string previousState);

    public abstract void ExitState();

    protected virtual void OnTransitionedEventHandler(string nextState)
    {
        TransitionedEventHandler?.Invoke(nextState);
    }
}