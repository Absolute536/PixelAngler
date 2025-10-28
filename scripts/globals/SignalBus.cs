namespace SignalBusNS;

using System;
using GameWorld;
using Godot;

public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    // public delegate TileType PositionChangedEventHandler(object sender, PositionEventArgs e);
    // public event PositionChangedEventHandler PositionChanged;

    // public delegate string LocationChangedEventHandler(object sender, PositionEventArgs e);
    // public event LocationChangedEventHandler LocationChanged;

    // // Other nodes will use this to invoke the desired event with the specified EventName
    // public TileType InvokePositionChangedEvent(object sender, PositionEventArgs e)
    // {
    //     // You know what, error checking later
    //     return PositionChanged.Invoke(sender, e);
    // }

    // public string InvokeLocationChangedEvent(object sender, PositionEventArgs e)
    // {
    //     return LocationChanged.Invoke(sender, e);
    // }

    /*
     * NOW
     * We placed the bobber motion event handler in the Signal Bus, instead of within the class itself,
     * because we might want to notify some other observers/subscribers outside of the player's scene
     * examples (for now):
     * OnForwardBobberMotionEnded -> trigger RNG to generate a fish object/scene, and initiate the fishing minigame
     * ReverseBobberMotionEnded -> cancel the fishing minigame
     * Additionally (actually the main reason), with this we can have more control over the state transitions
     * Such as waiting until the motion ends, then we trigger the state transitions (especially from idle to casting, then fishing, etc.)
     *
     * Also we restrict the caller, such that only the Bobber can raise these events
     */

    public delegate void BobberMotionEndedEventHandler(object sender, EventArgs e);
    public event BobberMotionEndedEventHandler ForwardBobberMotionEnded;
    public event BobberMotionEndedEventHandler ReverseBobberMotionEnded;

    public void OnForwardBobberMotionEnded(object sender, EventArgs e)
    {
        if (sender is Bobber)
            ForwardBobberMotionEnded?.Invoke(sender, e);
        else
            GD.PushError("Cannot raise bobber motion event if caller is not of type \"Bobber\"");
    }

        public void OnReverseBobberMotionEnded(object sender, EventArgs e)
    {
        if (sender is Bobber)
            ReverseBobberMotionEnded?.Invoke(sender, e);
        else
            GD.PushError("Cannot raise bobber motion event if caller is not of type \"Bobber\"");
    }




}

public enum PlayerActionType
{
    CastAction,
    NetAction,
    FishingAction
}

public class PositionalEventArgs : EventArgs
{
    public Vector2 CallerPosition { get; set; }
}

public class CastingEventArgs : EventArgs
{
    public TileType LandedOnTile { get; set; }
}