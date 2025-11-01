using System;
using System.Threading.Tasks;
using Godot;
using SignalBusNS;

[GlobalClass]
public partial class PlayerFishingState : State
{
    [Export] public Bobber Bobber;

    [Export] public FishingQuickTimeEvent FishingQTE;

    private bool _IsFishing;

    // private Timer _waitTimer = new (); // actually, we need to add it to scene tree for it to work, so let's try scene tree timer instead
    public override void _Ready()
    {
        StateName = Name;

        SignalBus.Instance.ReverseBobberMotionEnded += HandleReverseBobberMotionEnded;

        // _waitTimer.OneShot = true; // Well, I can either do it here, or during initialisation
        // _waitTimer.Timeout += SpawnFish;
    }

    public override void EnterState(string previousState)
    {
        // On enter, we start the timer
        // Or maybe on enter we add the QTE node to the scene tree (hold this one first)

        // So, on enter, create scene tree timer
        // subscribe to timeout
        // you know what, let's make two nodes, one for controlling the QTE, another for controlling the fishing
        FishingQTE.StartQTE();
        _IsFishing = true;
        // Correction, starting the QTE should be performed by the spawnning thingy
        // So here should be starting some timer to trigger to fish to spawn
        Random rand = new Random();
        int duration = rand.Next(2, 4);
        
        SceneTreeTimer timer = GetTree().CreateTimer(duration, true, true);
        timer.Timeout += SpawnFish;
        GD.Print("Fish Spawnning Timer set to duration: " + duration);

        // _waitTimer.Start(rand.Next(2, 4) + rand.NextDouble()); // Random int 2 ~ 4 + Random double 0.0 ~ 1.0
    }

    public override void ExitState()
    {
        // Hmm, on exit OR on click (for reverse bobber)? on exit works, but it kinda has a "delay", cuz we had to wait unti the bobber is fully reeled back
        // However, on exit feels "right"? Cuz we set the flag on enter, so we should reverse on exit right?
        // I'll leave it like this for now (maybe not)
        // YEAH ~~, I think we'll do on click, or else the fish will still spawn when the bobber is reeling back if you time it well
        // _IsFishing = false;
    }

    public override void HandleInput(InputEvent @event)
    {

    }

    public override void ProcessUpdate(double delta)
    {

    }

    public override void PhysicsProcessUpdate(double delta)
    {
        if (Input.IsActionJustPressed("Action")) // How about we can reverse at anytime?
        {
            _IsFishing = false;
            Bobber.ReverseBobberMotion();
        }
    }

    private void HandleReverseBobberMotionEnded(object sender, EventArgs e)
    {
        OnStateTransitioned("PlayerIdleState");
    }

    private void SpawnFish()
    {
        if (_IsFishing)
        {
            GD.Print("Fish appears!");  

            // invoke signal bus? and pass the reference through there? I guess it works?
        }

    }
    
}