namespace SignalBusNS;

using System;
using System.Collections.Generic;
using GamePlayer;
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

    // PlayerActionEventHandler delegate signature
    public delegate void PlayerActionEventHandler(object sender, EventArgs e);

    // Events of PlayerActionEventHandler
    public event PlayerActionEventHandler CastActionStart;
    public event PlayerActionEventHandler CastActionEnd;
    public event PlayerActionEventHandler NetActionStart;
    public event PlayerActionEventHandler NetActionEnd;
    public event PlayerActionEventHandler FishingActionStart;
    public event PlayerActionEventHandler FishingActionEnd;

    // Public "wrapper" methods for external class to invoke the events with the naming convention: "On<EventName>"
    // Methods for invoking events related to Player action -> only if sender is a PlayerActionManger (hmm... then, it is coupled? kinda inflexible). 
    public void OnCastActionStart(object sender, EventArgs e)
    {
        if (sender is PlayerActionManager)
            CastActionStart?.Invoke(sender, e);
        else
            GD.PushError("Cannot invoke player action related events if sender is not of type \"PlayerActionManager\"");
    }

    public void OnCastActionEnd(object sender, EventArgs e)
    {
        if (sender is PlayerActionManager)
            CastActionEnd?.Invoke(sender, e);
        else
           GD.PushError("Cannot invoke player action related events if sender is not of type \"PlayerActionManager\"");
    }

    public void OnNetActionStart(object sender, EventArgs e)
    {
        if (sender is PlayerActionManager)
            NetActionStart?.Invoke(sender, e);
        else
            GD.PushError("Cannot invoke player action related events if sender is not of type \"PlayerActionManager\"");
    }

    public void OnNetActionEnd(object sender, EventArgs e)
    {
        if (sender is PlayerActionManager)
            NetActionEnd?.Invoke(sender, e);
        else
            GD.PushError("Cannot invoke player action related events if sender is not of type \"PlayerActionManager\"");
    }

    public void OnFishingActionStart(object sender, EventArgs e)
    {
        FishingActionStart?.Invoke(sender, e);
    }

    public void OnFishingActionEnd(object sender, EventArgs e)
    {
        FishingActionEnd?.Invoke(sender, e);
    }

}

// public class PositionEventArgs : EventArgs
// {
//     public Vector2 Position { get; set; }
// }

// public enum EventName
// {
//     LocationChangedEvent,
//     PositionChangedEvent
// }