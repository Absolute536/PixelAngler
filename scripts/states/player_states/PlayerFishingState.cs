using System;
using GamePlayer;
using Godot;
using SignalBusNS;

[GlobalClass]
public partial class PlayerFishingState : State
{
    [Export] public Bobber Bobber;
    [Export] public Player Player;
    
    private bool _isFishingSuccess;

    public override void _Ready()
    {
        StateName = Name;
    }

    public override void EnterState(string previousState)
    {
        base.EnterState(previousState);
        SignalBus.Instance.FishCaught += HandleFishCaught;
        SignalBus.Instance.FishLost += HandleFishLost;
        SignalBus.Instance.ReverseBobberMotionEnded += HandleReverseBobberMotionEnded;

        _isFishingSuccess = false;
        Player.PlayerCamera.TopLevel = true;
        Player.PlayerCamera.GlobalPosition = Player.GlobalPosition;

        UiDisplayManager.Instance.CanSelectBait = false;
    }

    public override void ExitState()
    {
        base.ExitState();
        SignalBus.Instance.FishCaught -= HandleFishCaught;
        SignalBus.Instance.FishLost -= HandleFishLost;
        SignalBus.Instance.ReverseBobberMotionEnded += HandleReverseBobberMotionEnded;

        Player.PlayerCamera.TopLevel = false;
        Player.PlayerCamera.Position = Vector2.Zero;

        UiDisplayManager.Instance.CanSelectBait = true;
    }

    public override void HandleInput(InputEvent @event)
    {
        
    }

    public override void ProcessUpdate(double delta)
    {
        // testing lerp to bobber per frame
        // YOOOOOO, it works let's fucking gooooo
        // Ok, this is a bit of a hack, cuz we've sort of compromised encapsulation, but it should make the camera stop moving if in wrestle state
        // Yup, that works hehe
        if (MinigameManager.Instance.CurrentBehaviour != FishBehaviour.Yellow)
            Player.PlayerCamera.GlobalPosition = LerpSmooth(Player.PlayerCamera.GlobalPosition, Bobber.GlobalPosition, (float) delta);
    }

    public override void PhysicsProcessUpdate(double delta)
    {

    }

    private void HandleFishCaught(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
        {
            _isFishingSuccess = true;
            Bobber.ReverseBobberMotion();
        }
                 
    }

    private void HandleFishLost(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
        {
            _isFishingSuccess = false;
            Bobber.ReverseBobberMotion();
        }
    }

    private void HandleReverseBobberMotionEnded(object sender, EventArgs e)
    {
        if (IsCurrentlyActive)
        {
            if (_isFishingSuccess)
                OnStateTransitioned("PlayerShowOffState");
            else
                OnStateTransitioned("PlayerIdleState");
        }

        // this one should go to like show off state if fish is caught, and idle state if fish is lost?
    }

    // Kay~ so currently we're just instancing the fish scene directly, but later
    // we probably would want a spawner or some other class to handle all the rng and shits for spawnning the fish
    // then it'll return us a reference to the fish object from the scene here
    private Vector2 LerpSmooth(Vector2 from, Vector2 to, float delta)
	{
        // https://docs.godotengine.org/en/stable/tutorials/math/interpolation.html
        // More on here: https://easings.net/#easeInSine
        // Thanks random commenters
        delta = (1 - Mathf.Cos(Mathf.Pi * delta)) / 2;
		return to + (from - to) * Mathf.Exp(-75.0f * delta); // -speed * delta
	}
}