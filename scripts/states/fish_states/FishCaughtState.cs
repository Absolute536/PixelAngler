using GamePlayer;
using Godot;
using SignalBusNS;
using System;
using System.Text;

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

        SignalBus.Instance.OnCatchProgressUpdated(Fish.SpeciesInformation.FishId, Fish.CurrentSizeMeasurement); // only update progress update if the fish is caught
        SignalBus.Instance.FishObtained += HandleFishObtained;
        // also when the fish is caught, other fish can detect bobber when we're reeling in the fish
        // so, we probably should enable the monitoring or the IsLatchedOn here? (NEGATIVE)

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
        // 28 pixel above player, and X is GetSize().X of fish's texture / 2 to centre it
        Fish.GlobalPosition = player.GlobalPosition + new Vector2(Fish.SpeciesInformation.SpriteTexture.GetSize().X / 2, -28);

        RichTextLabel caughtNotif = GetFishCaughtNotif();
        
        Fish.AddChild(caughtNotif);
        // origin is at the top-left, so need to shift to the left by X / 2 to centre it, also 4 pixel above the fish
        caughtNotif.Position = new Vector2(-caughtNotif.Size.X / 2, -24); // Somehow only 24 pixel above fish works???
        caughtNotif.Visible = true;

        GetTree().CreateTimer(3.0f, false, true).Timeout += () =>
        {
            SignalBus.Instance.FishObtained -= HandleFishObtained;
            Fish.QueueFree(); // should also free the notif (Yep.)
        };
    }

    private RichTextLabel GetFishCaughtNotif()
    {
        // totol, just instantiate it, cuz we've already configured it in the editor (face palm)
        // update 9:53 pm, 10/12/2025, remove pulse effect, cuz of readability
        // update 10pm, again, just use a goldish colour for readability
        
        PackedScene caughtNotifScene = GD.Load<PackedScene>("res://scenes/fish_caught_notification.tscn");
        RichTextLabel caughtNotif = caughtNotifScene.Instantiate<RichTextLabel>();

        StringBuilder notifContent = new ("[pulse freq=1.50 color=d5ae55][wave amp=35.0 freq=5.0 connected=1]");

        // if (Fish.SpeciesInformation.Rarity == FishRarity.Common)
        //     caughtNotif.AddThemeColorOverride("default_color", new Color(0.25f, 0.52f, 0.75f));
        // else if (Fish.SpeciesInformation.Rarity == FishRarity.Uncommon)
        //     caughtNotif.AddThemeColorOverride("default_color", new Color(0.16f, 0.64f, 0.39f));
        // else
        //     caughtNotif.AddThemeColorOverride("default_color", new Color(0.74f, 0.20f, 0.23f));

        notifContent.Append(" " + Fish.SpeciesInformation.SpeciesName);
        notifContent.Append($" ({Fish.CurrentSizeMeasurement:F2}m) ");
        notifContent.Append("[/wave][/pulse]");

        caughtNotif.Text = notifContent.ToString();

        return caughtNotif;
    }
}