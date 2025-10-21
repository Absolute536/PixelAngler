using System;
using System.Threading.Tasks;
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

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
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
            // More accurately,
            // Stop the cast marker on release
            // Start the bobber motion & fishing line
            Tuple<TileType, Vector2> castingInfo = CastMarker.StopCastMarking();

            // Bobber.StartBobberMotion(castingInfo.Item2);
            // Now the problem is if the bobber won't land on water, it won't be visible at all
            // And what we want is to make it visible and hide it back when it lands
            // A simple solution is to use a loop to wait before resetting the bobber status (it can work, but might be a bit dangerous)
            // loop doesn't work, it will freeze
            // use a flag instead

            // ok maybe the event args can contain reference of nodes that were instantiated (like the line and bobber)
            // and then hide them or queue free them here based on markerTile?
            if (castingInfo.Item1 == TileType.Water)
            {
                // if on water do sth
                // OnStateTransitioned("PlayerFishingState");
                Bobber.StartBobberMotion(castingInfo.Item2, true);
                OnStateTransitioned("PlayerIdleState");
                // Go back to idle state for now
            }
            else
            {
                // reset the bobber and fishing line to initial state
                // in-between can pop-up message or some other operations etc.
                // transition back to idle state

                // wait until bobber stops before resetting and transition back to idle state
                // kinda works, because even in Stardew valley, you cast on non-water tile, you'll still be animation locked until the casting finishes
                // but yeah, having a loop is kinda dangerous
                // K... now it froze???
                // Ok, I see the problem. Cuz HasStopped() is called on the frame (at least within a few frame I suppose) click is released, we don't know when it stops
                // and Has stoppped is called, before the bobber ends(???)

                Bobber.StartBobberMotion(castingInfo.Item2, false);
                OnStateTransitioned("PlayerIdleState");
            }
        }
    }

}