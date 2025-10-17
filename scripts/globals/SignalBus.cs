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

    // PlayerActionEventHandler delegate signature
    public delegate void PlayerActionEventHandler(object sender, EventArgs e);

    // Events of PlayerActionEventHandler
    public event PlayerActionEventHandler CastingStarted;
    public event PlayerActionEventHandler CastActionEnd;
    public event PlayerActionEventHandler NetActionStart;
    public event PlayerActionEventHandler NetActionEnd;
    public event PlayerActionEventHandler FishingActionStart;
    public event PlayerActionEventHandler FishingActionEnd;

    public delegate void BobberMovementEventHandler(object sender, PositionalEventArgs posArgs);

    public delegate TileType TileTypeEventHandler(object sender, EventArgs e);
    private TileTypeEventHandler CastingEnded;

    public void SubscribeCastingEnded(object sender, TileTypeEventHandler method)
    {
        if (CastingEnded is not null)
        {
            GD.PushError("MarkerLanded can only have one subscriber");
            return;
        }

        CastingEnded = method;
        GD.Print("Callback successfully added to MarkerLanded by " + sender.GetType());
    }

    public TileType OnCastingEnded(object sender, EventArgs e)
    {
        // if (sender is PlayerCastingState)
        TileType landedTile = CastingEnded.Invoke(sender, e);
        return landedTile;

    }


    public event BobberMovementEventHandler BobberMovement;

    // Public "wrapper" methods for external class to invoke the events with the naming convention: "On<EventName>"
    // Methods for invoking events related to Player action -> only if sender is a PlayerActionManger (hmm... then, it is coupled? kinda inflexible). 
    // public void OnCastActionStart(object sender, EventArgs e)
    // {
    //     if (sender is PlayerActionManager)
    //         CastActionStart?.Invoke(sender, e);
    //     else
    //         GD.PushError("Cannot invoke player action related events if sender is not of type \"PlayerActionManager\"");
    // }

    // public void OnCastActionEnd(object sender, EventArgs e)
    // {
    //     if (sender is PlayerActionManager)
    //         CastActionEnd?.Invoke(sender, e);
    //     else
    //        GD.PushError("Cannot invoke player action related events if sender is not of type \"PlayerActionManager\"");
    // }

    // public void OnNetActionStart(object sender, EventArgs e)
    // {
    //     if (sender is PlayerActionManager)
    //         NetActionStart?.Invoke(sender, e);
    //     else
    //         GD.PushError("Cannot invoke player action related events if sender is not of type \"PlayerActionManager\"");
    // }

    // public void OnNetActionEnd(object sender, EventArgs e)
    // {
    //     if (sender is PlayerActionManager)
    //         NetActionEnd?.Invoke(sender, e);
    //     else
    //         GD.PushError("Cannot invoke player action related events if sender is not of type \"PlayerActionManager\"");
    // }

    // public void OnFishingActionStart(object sender, EventArgs e)
    // {
    //     FishingActionStart?.Invoke(sender, e);
    // }

    // public void OnFishingActionEnd(object sender, EventArgs e)
    // {
    //     FishingActionEnd?.Invoke(sender, e);
    // }

    // Well actually... perhaps we can consider using an "ItemType" enum instead of actionType for specifying which action to invoke (to align with the manager)
    public void OnActionStart(PlayerActionType actionType, object sender, EventArgs e)
    {
        // if (sender is not PlayerActionManager)
        // {
        //     GD.PushError("Cannot invoke player action related events if sender is not of type \"PlayerActionManager\"");
        //     return;
        // }

        switch (actionType)
        {
            case PlayerActionType.CastAction:
                CastingStarted?.Invoke(sender, e);
                break;
            case PlayerActionType.NetAction:
                NetActionStart?.Invoke(sender, e);
                break;
            case PlayerActionType.FishingAction: // not sure if only action manager can invoke fishing yet
                FishingActionStart?.Invoke(sender, e);
                break;
            default:
                throw new ArgumentException("PlayerActionType argument provided is invalid");
        }

    }

    public void OnActionEnd(PlayerActionType actionType, object sender, EventArgs e)
    {
        if (sender is not PlayerActionManager)
        {
            GD.PushError("Cannot invoke player action related events if sender is not of type \"PlayerActionManager\"");
            return;
        }

        switch (actionType)
        {
            case PlayerActionType.CastAction:
                CastActionEnd?.Invoke(sender, e);
                break;
            case PlayerActionType.NetAction:
                NetActionEnd?.Invoke(sender, e);
                break;
            case PlayerActionType.FishingAction: // not sure if only action manager can invoke fishing yet
                FishingActionEnd?.Invoke(sender, e);
                break;
            default:
                throw new ArgumentException("PlayerActionType argument provided is invalid");
        }

    }
    
    public void OnBobberMovement(object sender, PositionalEventArgs posArgs)
    {
        BobberMovement?.Invoke(sender, posArgs);
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