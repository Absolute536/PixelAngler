using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class StateMachine : Node
{
    [Export] public State initialState;
    private State currentState;
    private Dictionary<string, State> availableStates = new();

    public override void _Ready()
    {
        // Initialise currentState
        foreach (State state in GetChildren().Cast<State>())
        {
            availableStates.Add(state.StateName, state);
            state.StateTransitioned += HandleStateTransition;
            // We're subscribing to the state's transition event
            // Probably overkill cuz the only observer is the state machine, but we'll see
        }

        // Wait until owner node is ready (do we need this?)
        // while (!Owner.IsNodeReady())
        //     Console.WriteLine("Owner is not ready, waiting...");

        if (initialState != null)
        {
            initialState.EnterState(""); // previous state is an empty string since it's the very first state (might change it later?)
            currentState = initialState;
            GD.Print("State Machine: Initial State " + initialState.StateName);
        }
        else
        {
            currentState = availableStates["PlayerIdleState"]; // Set PlayerIdleState as the default as a fallback (about that ~)
            currentState.EnterState("");
        }
    }

    public override void _Process(double delta)
    {
        currentState.ProcessUpdate(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        currentState.PhysicsProcessUpdate(delta);
        // GD.Print("Current State: " + currentState.StateName);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // base._UnhandledInput(@event);
        // GD.Print("Unhandled input detected in State Machine");
        currentState.HandleInput(@event);
    }

    private void HandleStateTransition(string nextState)
    {
        // Basically, we check if next state is valid
        // If True, fetch it from dictionary, call its EnterState(), call the currentState's ExitState() and assign new state to currentState
        if (availableStates.Keys.Contains(nextState))
        {
            State targetState = availableStates[nextState];

            currentState.ExitState();
            GD.Print("State Machine: Exit " + currentState.StateName);

            targetState.EnterState(currentState.StateName); // Currently we use the name of the node (string) to specify the previous state and next state
            GD.Print("State Machine: Enter " + targetState.StateName);
            currentState = targetState;
        }
        else
            GD.PushError("Specified next state not found on state transition");
        
    }

}