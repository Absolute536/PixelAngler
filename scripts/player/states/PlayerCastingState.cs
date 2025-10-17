using System;
using GamePlayer;
using GameWorld;
using Godot;
using SignalBusNS;

[GlobalClass]
public partial class PlayerCastingState : State
{
    private Player player;

    // Ok, instead of using the player action manager, we trasition into the respective state for each action based on SelectedItem
    public override void _Ready()
    {
        StateName = Name;
        player = GetTree().GetFirstNodeInGroup("Player") as Player;
    }

    public override void EnterState(string previousState)
    {
        SignalBus.Instance.OnActionStart(PlayerActionType.CastAction, this, EventArgs.Empty);
        // Can't just invoke start, we need something to return the location information for state transition
    }

    public override void ExitState()
    {

    }

    public override void HandleInput(InputEvent @event)
    {

    }

    public override void ProcessUpdate(double delta)
    {

    }

    public override void PhysicsProcessUpdate(double delta)
    {
        if (Input.IsActionJustReleased("Action"))
        {
            TileType markerTile = SignalBus.Instance.OnCastingEnded(this, EventArgs.Empty);
            // ok maybe the event args can contain reference of nodes that were instantiated (like the line and bobber)
            // and then hide them or queue free them here based on markerTile?
            if (markerTile == TileType.Water)
            {
                // if on water do sth
                OnStateTransitioned("PlayerFishingState");
                // Go back to idle state for now
            }
            else
                OnStateTransitioned("PlayerIdleState");
        }
    }
}