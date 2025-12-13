using Godot;
using System;
using SignalBusNS;

[GlobalClass]
public partial class FishBitingState : State
{
    [Export] Fish Fish;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void _ExitTree()
    {
        SignalBus.Instance.QTESucceeded -= HandleQTESuccess;
        SignalBus.Instance.QTEFailed -= HandleQTEFailed;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);

        // Disable the collision shaped here so that on bite player would need to cast again
        Fish.LatchTarget.BobberCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

        // Play the sound cue here (for the fish bite - start of QTE)
        AudioManager.Instance.PlaySfx(this, SoundEffect.QteNotification, false);

        Texture2D biteAlertTexture = GD.Load<Texture2D>("res://assets/ui_design/fish_bite_alert_icon.png");
        Sprite2D biteAlertIcon = new Sprite2D
        {
          Texture = biteAlertTexture,
          Position = Fish.FishSprite.FlipH ? new Vector2(-Fish.SpeciesInformation.SpriteTexture.GetSize().X, -8) : new Vector2(0, -8)
        };
        Fish.AddChild(biteAlertIcon);
        GetTree().CreateTimer(0.6, false, true).Timeout += biteAlertIcon.QueueFree; // 0.7 just like the QTE window? (nah, make it -1)

        SignalBus.Instance.QTESucceeded += HandleQTESuccess;
        SignalBus.Instance.QTEFailed += HandleQTEFailed;

        if (previousState == "FishNibbleState")
        {
            SignalBus.Instance.OnFishBite(this, EventArgs.Empty);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        SignalBus.Instance.QTESucceeded -= HandleQTESuccess;
        SignalBus.Instance.QTEFailed -= HandleQTEFailed;
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

    private void HandleQTESuccess(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
        {
            // Maybe can include another sound cue for success (like ding!)
            AudioManager.Instance.PlaySfx(this, SoundEffect.QteSuccess, false);


            // GetTree().Paused = true;
            // GetTree().Paused = false;

            // TRY IT (YEAH IT WORKS!!!)
            // QTE Success, instantiate the progress bar and pass the reference to MinigameManager
            PackedScene progressBarScene = GD.Load<PackedScene>("res://scenes/fishing_progress.tscn");
            FishingProgress fishingProgress = progressBarScene.Instantiate<FishingProgress>();
            fishingProgress.Position = new Vector2(-8, 8);

            // MarginContainer progressBarRoot = progressBarScene.Instantiate<MarginContainer>();
            // FishingProgressBar progressBar = progressBarRoot.GetChild(0) as FishingProgressBar; // fix later :P
            // GD.Print("Size: " + progressBar.Size); // size is 0? (cuz only controlled by parent?)
            // progressBar.Size = new Vector2(40, 3);
            // progressBarRoot.Position = new Vector2(-8, 8);

            Fish.AddChild(fishingProgress);
            MinigameManager.Instance.CurrentSpeciesInGame = Fish.SpeciesInformation;
            MinigameManager.Instance.StartMinigame(fishingProgress);

            OnStateTransitioned("FishHookedState");
           
        }
            
    }

    private void HandleQTEFailed(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
            OnStateTransitioned("FishStartledState");
    }

}