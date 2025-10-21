using System;
using GamePlayer;
using GameWorld;
using Godot;
using SignalBusNS;

[GlobalClass]
public partial class PlayerCastingState : State
{
    [Export] public Player Player;
    [Export] public CastMarker CastMarker;
    [Export] public Bobber Bobber;

    // Ok, instead of using the player action manager, we trasition into the respective state for each action based on SelectedItem
    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        SignalBus.Instance.OnActionStart(PlayerActionType.CastAction, this, EventArgs.Empty);
        CastMarker.StartCastMarking();
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
            // Ok, so on Action release, we need to cast the bobber and fishing line
            // but if they don't land on water (marker not on water previously)
            // hide them back and reset to initial state

            // start bobber motion
            // start casting animation

            // More accurately,
            // Stop the cast marker on release (and start the bobber & fishing line motion)
            // The bobber motion is invoked in StopCastMarking

            Tuple<TileType, Vector2> castingInfo = CastMarker.StopCastMarking();
            Bobber.StartBobberMotion(castingInfo.Item2);
            // ok maybe the event args can contain reference of nodes that were instantiated (like the line and bobber)
            // and then hide them or queue free them here based on markerTile?
            if (castingInfo.Item1 == TileType.Water)
            {
                // if on water do sth
                // OnStateTransitioned("PlayerFishingState");
                OnStateTransitioned("PlayerIdleState");
                // Go back to idle state for now
            }
            else
            {
                // reset the bobber and fishing line to initial state
                // in-between can pop-up message or some other operations etc.
                // transition back to idle state
                Bobber.ResetBobberStatus();
                OnStateTransitioned("PlayerIdleState");
            }
        }
    }

}