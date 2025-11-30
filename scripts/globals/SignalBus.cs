namespace SignalBusNS;

using System;
using GameWorld;
using Godot;

// https://forum.godotengine.org/t/optimal-way-of-handling-custom-signals-in-godot-c-mono/122454/2
// EventBus implementation
// Smart and clean, but seems like you would need to create many concrete classes for each "event"
// The properties of the event classes will act as input parameters to the delegate invocation I suppose
// And since the subscription and publishing methods are based on the Type <T>, that is specify upon invocation, it is still safe
// Subscribe<T>(Action<T> callback) 
// --> try and get the list of delegate (or Action<T>s) from the dictionary
// --> if fail, assign empty list to "value", add new entry with the type, T, as the key, and "value" as the value-pair
// --> note that the keyword "out" is used, so "value" will contain the value associated with the key if success, if failure, it will be the defaul value of the Type of "value", which in this case, for List<T> is null
// --> then, still with the reference to "value", Add the input argument callback, which is an Action<T> --> a method with one input parameter, but no return value
// --> in this case, it is a method that matches the delegate (or Action) signature, which takes in an argument of type T, that will be the concrete event class that you specify when calling the method
// Publish<T>(T event) and Unsubscribe<T>(Action<T> callback) methods work largely the same, but with different internal logic
// Do I want to use this, hmm.....
// Looks clean, but I think I'll stick with the current SignalBus first, else I'll get nothing done (>A< !!!)

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
     * especially to prevent movement when the bobber is still in motion, but already transition to walking state
     * Such as waiting until the motion ends, then we trigger the state transitions (especially from idle to casting, then fishing, etc.)
     *
     * Also we restrict the caller, such that only the Bobber can raise these events
     */

    public event EventHandler ForwardBobberMotionEnded;
    public event EventHandler ReverseBobberMotionEnded;

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

    // Probably won't use this, but keep it for now
    public event EventHandler AnglingStarted;
    public void OnAnglingStarted(object sender, EventArgs e)
    {
        if (sender is PlayerFishingState)
            AnglingStarted?.Invoke(sender, e);
        else
            GD.PushError("Cannot raise angling started event if caller is not of type \"PlayerFishingState\"");
    }

    public event EventHandler FishBite;
    public void OnFishBite(object sender, EventArgs e)
    {
        FishBite?.Invoke(sender, e);
    }

    public event EventHandler AnglingCancelled;
    public void OnAnglingCancelled(object sender, EventArgs e)
    {
        if (sender is PlayerAnglingState)
            AnglingCancelled?.Invoke(sender, e);
        else
            GD.PushError("Cannot raise angling cancelled event if caller is not of type \"PlayerAnglingState\"");
    }
    public event EventHandler QTESucceeded;
    public event EventHandler QTEFailed;

    public void OnQTESucceeded(object sender, EventArgs e)
    {
        QTESucceeded?.Invoke(sender, e);
    }
    
    public void OnQTEFailed (object sender, EventArgs e)
    {
        QTEFailed?.Invoke(sender, e);
    }

    public delegate void FishBehaviourChangedEventHandler(FishBehaviour behaviour, int actionRepeat);
    public event FishBehaviourChangedEventHandler FishBehaviourChanged;

    public void OnFishBehaviourChanged(FishBehaviour behaviour, int actionRepeat)
    {
        FishBehaviourChanged?.Invoke(behaviour, actionRepeat);
    }

    public event EventHandler FishWrestleCompleted;
    public void OnFishWrestleCompleted(object sender, EventArgs e)
    {
        FishWrestleCompleted?.Invoke(sender, e);
    }

    public event EventHandler FishCaught;
    public void OnFishCaught(object sender, EventArgs e)
    {
        FishCaught?.Invoke(sender, e);
    }

    public delegate void InGameTimeChangedEventHandler(float scaledTotalMinutes, int day, int hours, int minutes);
    public event InGameTimeChangedEventHandler InGameTimeChanged;

    public void OnInGameTimeChanged(float scaledTotalMinutes, int day, int hours, int minutes)
    {
        InGameTimeChanged?.Invoke(scaledTotalMinutes, day, hours, minutes);
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