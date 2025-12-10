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
    }

    public override void ExitState()
    {
        base.ExitState();
        SignalBus.Instance.FishCaught -= HandleFishCaught;
        SignalBus.Instance.FishLost -= HandleFishLost;
        SignalBus.Instance.ReverseBobberMotionEnded += HandleReverseBobberMotionEnded;

        Player.PlayerCamera.TopLevel = false;
        Player.PlayerCamera.Position = Vector2.Zero;
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
        // if (Input.IsActionJustPressed("Action")) // How about we can reverse at anytime?
        // {
            // _IsFishing = false;
            // Bobber.ReverseBobberMotion();
            // SignalBus.Instance.OnAnglingCancelled(this, EventArgs.Empty); // Quick note, if not casted on water and fish detected, they will still home onto bobber [Solved 13/11/2025]
            // _instancedFish.TopLevel = true;
        // }
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
		return to + (from - to) * Mathf.Exp(-25.0f * delta); // speed = 25.0f
	}
}