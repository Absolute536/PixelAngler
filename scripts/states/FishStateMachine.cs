using Godot;
using System;
using SignalBusNS;

[GlobalClass]
public partial class FishStateMachine : StateMachine
{
    public override void _Ready()
    {
        base._Ready(); // need to so that the base StateMachine's _Ready() is executed, and every property is initialised

        // So if angling cancelled (reel back before bite), failed QTE and a fish has bitten (QTE success), go back to wander state
        // I think it's more like if a fish has reached the required nibble count, it bites, then the other go back to wander (regardless of QTE success/failure)
        // then need to cast again? (or not? let the other fish have the opportunitiy to bite too) ---> oooh, also make the fish go in opposite direction of bobber
        SignalBus.Instance.AnglingCancelled += RevertNibbleState;
        SignalBus.Instance.QTEFailed += RevertNibbleState;
        SignalBus.Instance.FishBite += RevertNibbleState; // probably can't cuz the biting one will transition as well
    }

    public void RevertNibbleState(object sender, EventArgs e)
    {
        if (currentState is FishNibbleState)
            HandleStateTransition("FishStartledState");

        GD.Print("Print if angling cancelled");
    }
}