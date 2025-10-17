using System;
using Godot;

public interface StateAction
{
    public abstract void HandleInput(InputEvent inputEvent);

    public abstract void ProcessUpdate(double delta);

    public abstract void PhysicsProcessUpdate(double delta);

    public abstract void EnterState(string previousState);

    public abstract void ExitState();
}