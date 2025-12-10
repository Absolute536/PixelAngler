using GamePlayer;
using Godot;
using SignalBusNS;
using System;

[GlobalClass]
public partial class FishCaughtState : State
{
    [Export] public Fish Fish;
    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);

        SignalBus.Instance.OnCatchProgressUpdated(Fish.SpeciesInformation.FishId); // only update progress update if the fish is caught
        SignalBus.Instance.FishObtained += HandleFishObtained;
        // also when the fish is caught, other fish can detect bobber when we're reeling in the fish
        // so, we probably should enable the monitoring or the IsLatchedOn here?

        Fish.Velocity = Vector2.Zero;
        Fish.FishSprite.Material = null;
        // GetTree().CreateTimer(3.0, false, true).Timeout += () => {Fish.QueueFree();}; // test queue free after 3 seconds
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
        // maybe can put the caught motion (fish follow bobber here instead)
    }

    public override void ProcessUpdate(double delta)
    {
        
    }

    private void HandleFishObtained(object sender, EventArgs e)
    {
        Player player = GetTree().GetFirstNodeInGroup("Player") as Player;
        Fish.IsCaught = false; // to disable the latching
        // 48 pixel above player, and X is GetSize().X of fish's texture / 2 to centre it
        Fish.GlobalPosition = player.GlobalPosition + new Vector2(Fish.SpeciesInformation.SpriteTexture.GetSize().X / 2, -32);

        GetTree().CreateTimer(3.0f, false, true).Timeout += () =>
        {
            SignalBus.Instance.FishObtained -= HandleFishObtained;
            Fish.QueueFree();
        };
    }
}