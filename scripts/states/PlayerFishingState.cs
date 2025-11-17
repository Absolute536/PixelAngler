using System;
using GamePlayer;
using Godot;
using SignalBusNS;

[GlobalClass]
public partial class PlayerFishingState : State
{
    [Export] public Bobber Bobber;
    [Export] public Player Player;
    
    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
    }

    public override void ExitState()
    {
        // Hmm, on exit OR on click (for reverse bobber)? on exit works, but it kinda has a "delay", cuz we had to wait unti the bobber is fully reeled back
        // However, on exit feels "right"? Cuz we set the flag on enter, so we should reverse on exit right?
        // I'll leave it like this for now (maybe not)
        // YEAH ~~, I think we'll do on click, or else the fish will still spawn when the bobber is reeling back if you time it well
        // _IsFishing = false;
        base.ExitState();
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed("Action")) // just a quick and dirty test
        {
            GD.Print("Reel in");
        }
    }

    public override void ProcessUpdate(double delta)
    {

    }

    public override void PhysicsProcessUpdate(double delta)
    {
        // if (Input.IsActionJustPressed("Action")) // How about we can reverse at anytime?
        // {
            // _IsFishing = false;
            // Bobber.ReverseBobberMotion();
            // SignalBus.Instance.OnAnglingCancelled(this, EventArgs.Empty); // Quick note, if not casted on water and fish detected, they will still home onto bobber [Solved 13/11/2025]
            // _instancedFish.TopLevel = true;
        // }
    }


    // Kay~ so currently we're just instancing the fish scene directly, but later
    // we probably would want a spawner or some other class to handle all the rng and shits for spawnning the fish
    // then it'll return us a reference to the fish object from the scene here
    
}