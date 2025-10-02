using System;
using Godot;

public interface State
{
    public delegate void Transitioned(string nextState);
    public event Transitioned TransitionedEventHandler;

    public string StateName { get; set; }

    public abstract void HandleInput(InputEvent inputEvent);

    public abstract void ProcessUpdate(double delta);

    public abstract void PhysicsProcessUpdate(double delta);

    public abstract void EnterState(string previousState);

    public abstract void ExitState();

    // protected virtual void OnTransitionedEventHandler(string nextState)
    // {
    //     TransitionedEventHandler?.Invoke(nextState);
    // }
}