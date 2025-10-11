namespace SignalBus;

using System;
using System.Collections.Generic;
using GameWorld;
using Godot;

public partial class SignalBus : Node
{
    public static SignalBus Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public delegate TileType PositionChangedEventHandler(object sender, PositionEventArgs e);
    public event PositionChangedEventHandler PositionChanged;

    public delegate string LocationChangedEventHandler(object sender, PositionEventArgs e);
    public event LocationChangedEventHandler LocationChanged;

    // Other nodes will use this to invoke the desired event with the specified EventName
    public TileType InvokePositionChangedEvent(object sender, PositionEventArgs e)
    {
        // You know what, error checking later
        return PositionChanged.Invoke(sender, e);
    }

    public string InvokeLocationChangedEvent(object sender, PositionEventArgs e)
    {
        return LocationChanged.Invoke(sender, e);
    }
}

public class PositionEventArgs : EventArgs
{
    public Vector2 Position { get; set; }
}

public enum EventName
{
    LocationChangedEvent,
    PositionChangedEvent
}