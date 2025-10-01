using Godot;
using System;
using GamePlayer;
using System.Linq;

[GlobalClass]
public partial class PlayerActionState : State
{
    [Export] public CharacterBody2D Player;

    private Player player;
    
    public override void _Ready()
    {
        player = (Player) Player;
    }

    public override void EnterState(string previousState)
    {
        // Comment out first cuz haven't made the animation yet
        // player.AnimationPlayer.Play("Idle");
        // if (previousState == "PlayerWalkingState")
        //     player.AnimationPlayer.Stop();

        // TEST
        // On entering the state, add the fishing line to player (let's see)

        // Ok, so now we instance the line, and assign it to fishing line (hmm, can this work?)
        Line2D line = new()
        {
            Width = 1,
            DefaultColor = Colors.White,
            Points = [new Vector2(0, 0)] // start at (0, 0)
        };
        fishingLine = line;

        player.AddChild(fishingLine);
    }

    public override void ExitState()
    {
        // Remove the fishing line on exiting the state (performance cost?)
        fishingLine.QueueFree();
    }

    public override void HandleInput(InputEvent inputEvent)
    {
        // Nothing here
    }

    public override void ProcessUpdate(double delta)
    {
        // Nothing per frame
    }

    // We'll put things here first just for experiment
    // Perhaps we should define separate behaviours for each itemAction (hmm???)

    private Line2D fishingLine;

    private int lineLength = 0;

    public override void PhysicsProcessUpdate(double delta)
    {
        // Let's list out the steps
        // We transition from idle to action on left mouse click
        // In this state, if the mouse is held, 

        lineLength++; // Increase length per physics tick 
        if (Input.IsActionPressed("ItemAction"))
        {
            // Add the point to the Line2D so that the line is extending
            fishingLine.AddPoint(new(lineLength, -lineLength));
        }

        // Upon releasing the left mouse click
        if (Input.IsActionJustReleased("ItemAction"))
        {
            lineLength = 0; // reset length
            OnTransitionedEventHandler("PlayerIdleState"); // if it's released, go back to idle

        }


        // For future reference: CanvasModulate Node for day/night cycle
        // Might also check out object pooling for the fishing line (or maybe not, cuz it's just one node)
        // But yeah for best practices, why not?!
    }
}