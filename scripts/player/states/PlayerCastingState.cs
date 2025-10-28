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
    [Export] public FishingLine FishingLine;

    private bool _landedInWater = true;

    public override void _Ready()
    {
        StateName = Name;
        SignalBus.Instance.ForwardBobberMotionEnded += HandleForwardBobberMotionEnded;
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
            // Stop cast marking on action release
            // Get the casting info in the form of a Tuple (TileType of the landing tile, Position of the landing tile)
            Tuple<TileType, Vector2> castingInfo = CastMarker.StopCastMarking();

            _landedInWater = castingInfo.Item1 == TileType.Water;
            if (_landedInWater)
            {
                // if on water do sth
                // OnStateTransitioned("PlayerFishingState");
                Bobber.StartBobberMotion(castingInfo.Item2, _landedInWater);
                // cast fishing line, passing the landing position
                // actually, we can't put it here if we want the line to "animate" with the bobber motion

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

                Bobber.StartBobberMotion(castingInfo.Item2, _landedInWater);
            }
        }
    }

    public void HandleForwardBobberMotionEnded(object sender, EventArgs e)
    {
        if (_landedInWater)
            OnStateTransitioned("PlayerFishingState");
        else
            OnStateTransitioned("PlayerIdleState");
    }
}