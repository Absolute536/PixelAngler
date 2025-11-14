using Godot;
using System;
using SignalBusNS;

public partial class MinigameManager : Node
{
    public static MinigameManager Instance { get; private set; }
    private QuickTimeEvent QTE;

    public override void _Ready()
    {
        Instance = this;
        QTE = QuickTimeEvent.Instance; // get the static instance of quick time event (autoload)
        SignalBus.Instance.FishBite += StartQTE;
    }

    public void StartQTE(object sender, EventArgs e)
    {
        QTE.StartQuickTimeEvent(0.7f, "Action");
    }


    
}