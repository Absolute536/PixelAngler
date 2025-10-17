using Godot;
using System;
using GamePlayer;
using System.Linq;


[GlobalClass]
public partial class PlayerActionState : State
{
    [Export] public CharacterBody2D Player;
    [Export] public PlayerActionManager PlayerActionManager;

    private Player player;

    private Sprite2D castMarker;

    // public PlayerActionState()
    // {
    //      // StateName property is the Name of the node
    // }

    // NOTE: Clean up this part cuz we're assuming the ItemAction is only for fishing, there could be other things as well?
    // Or maybe we just make it like this and the bug catching part is like a clickable area that pops up?
    public override void _Ready()
    {
        // ----> initialise the property in Ready() instead of constructor
        // This is because _Ready() triggers after the constructor (or _init())
        // So putting the initialisation in constructor might cause it to get "" empty string (I think) or null value?
        // Because the node is not loaded yet, and the Name property of the Node can't be accessed yet
        // Order is: initial value upon definition (default value), then constructor, then exported value assignment
        // And since StateMachine accesses the property in Ready(), it is guaranteed that it is safe
        // Because the Ready() callback for a node only triggers after the Ready() of all its children nodes
        // So we won't get Dictionary has the same key "" exception because the State Nodes hasn't initialised their name yet
        StateName = Name;
        player = (Player)Player;
        // castMarker = new Sprite2D();

        // https://docs.godotengine.org/en/stable/tutorials/io/runtime_file_loading_and_saving.html
        // Image markerImg = Image.LoadFromFile("res://icon.svg");
        // castMarker.Texture = ImageTexture.CreateFromImage(markerImg);
        // castMarker.Visible = false;
        // castMarker.Name = "CastMarker";
        // castMarker.Scale = new Vector2(0.5f, 0.5f);
        // player.CallDeferred(MethodName.AddChild, castMarker);

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
        // Line2D line = new()
        // {
        //     Width = 1,
        //     DefaultColor = Colors.White,
        //     Points = [new Vector2(0, 0)] // start at (0, 0)
        // };
        // fishingLine = line;

        // player.AddChild(fishingLine);
        // castMarker.Visible = true;

        // PackedScene marker = GD.Load<PackedScene>("res://scenes/cast_marker.tscn");
        // player.AddChild(marker.Instantiate());
        // GetNode<Sprite2D>("../../FishingAction/CastMarker").Visible = true;

        // Immediately Start the specified action upon entering the state
        PlayerActionManager.StartAction(player.SelectedItem);
    }

    public override void ExitState()
    {
        // Remove the fishing line on exiting the state (performance cost?)
        // fishingLine.QueueFree();

    }

    // The Input callbacks will be triggered per InputEvent(?), NOT per frame
    // Since we want to trigger it only once on certain input, put it here instead of the processes callbacks
    // wait, I might be able to use the input singleton here. Just need to mark it as consumed from the previous state?
    public override void HandleInput(InputEvent @event)
    {

    }

    public override void ProcessUpdate(double delta)
    {
        // Nothing per frame
    }

    public override void PhysicsProcessUpdate(double delta)
    {
        // Check for Action Released event, if it does, end the action (for now) and go back to idle state
        if (Input.IsActionJustReleased("Action"))
        {
            PlayerActionManager.EndAction(player.SelectedItem);
            OnStateTransitioned("PlayerIdleState");
        }

        // For future reference: CanvasModulate Node for day/night cycle
        // Might also check out object pooling for the fishing line (or maybe not, cuz it's just one node)
        // But yeah for best practices, why not?!
    }


    // fish idle duration - aggresiveness?
}