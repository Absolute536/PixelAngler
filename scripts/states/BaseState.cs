using System;
using Godot;

[GlobalClass]
public abstract partial class BaseState : Node
{
    private string _stateName;
    public string StateName
    {
        get => _stateName;
        set => _stateName = StateName.ToUpper(); // Convert all state names to upper case characters
    }
    public delegate void Transitioned(string nextState);
    public event Transitioned TransitionedEventHandler;
    public abstract void HandleInput(InputEvent inputEvent);

    public abstract void ProcessUpdate(double delta);

    public abstract void PhysicsProcessUpdate(double delta);

    public abstract void EnterState(string previousState);

    public abstract void ExitState();

    protected virtual void InvokeTransitionedEventHandler(string nextState)
    {
        TransitionedEventHandler?.Invoke(nextState);
    }
}