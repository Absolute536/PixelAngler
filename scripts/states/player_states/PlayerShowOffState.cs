using GamePlayer;
using Godot;
using SignalBusNS;
using System;

[GlobalClass]
public partial class PlayerShowOffState : State
{
    [Export] public Player Player;
    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        // play the show off animation
        Player.AnimationPlayer.Animation = "ShowOff";

        AudioManager.Instance.PlaySfx(this, SoundEffect.ShowOff, true);

        // position the fish
        // using signal to obtain a reference
        // OR we do the same thing for the fish within the CaughtState (but this is kinda bad, cuz it's separated)
        
        // Raise FishObtained event so that the fish will be moved above the player (along with the notification) 
        SignalBus.Instance.OnFishObtained(this, EventArgs.Empty);

        // instantiate the caught alert
        // you know, maybe the alert should be instantiated in fish (*** HERE ***)
        // PackedScene caughtAlertScene = GD.Load<PackedScene>("res://scenes/fish_caught_alert.tscn");
        // FishCaughtAlert caughtAlert = caughtAlertScene.Instantiate<FishCaughtAlert>();
        // caughtAlert.Position = Player.FacingDirection == Vector2.Left ? new Vector2(48, -48) : new Vector2(-48, -48);
        // Player.AddChild(caughtAlert);

        // // then exit after some delay AND? on click?
        GetTree().CreateTimer(3.0f, false, true).Timeout += () =>
        {
            OnStateTransitioned("PlayerIdleState");
        };
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        
    }

    public override void ProcessUpdate(double delta)
    {
        
    }

    private void HandleFishObtained(object sender, Fish caughtFish)
    {
        
    }

}