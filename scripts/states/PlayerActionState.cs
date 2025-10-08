using Godot;
using System;
using GamePlayer;
using System.Linq;


[GlobalClass]
public partial class PlayerActionState : State
{
    [Export] public CharacterBody2D Player;
    [Export] public ActionManager PlayerActionManager;

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
        GetNode<Sprite2D>("../../FishingAction/CastMarker").Visible = true;
    }

    public override void ExitState()
    {
        // Remove the fishing line on exiting the state (performance cost?)
        // fishingLine.QueueFree();
        
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

        // CREATE AND EXTEND CAST MARKER HERE (Fishing Rod as SelectedItem)

        if (Input.IsActionPressed("Action"))
        {
            // if LMB and Fishing Rod is selected
            // we forward the selected item to the action manager
            // action manager will call the respective functions to perform the operations.
            if (player.SelectedItem == "Fishing Rod")
            {
                
            }
            
        }

        // Upon releasing the left mouse click
        if (Input.IsActionJustReleased("Action"))
        {
            // player.GetChild(-1).QueueFree();
            GetNode<Sprite2D>("../../FishingAction/CastMarker").Visible = false;
            OnTransitionedEventHandler("PlayerIdleState"); // if it's released, go back to idle

            GD.Print("Back to idle");

        }


        // For future reference: CanvasModulate Node for day/night cycle
        // Might also check out object pooling for the fishing line (or maybe not, cuz it's just one node)
        // But yeah for best practices, why not?!
    }

    // fish idle duration - aggresiveness?
    private void StartFishingItemAction()
    {
        lineLength++; // Increase length per physics tick 
                      // Add the point to the Line2D so that the line is extending
        if (player.FacingDirection == Vector2.Up)
        {
            fishingLine.AddPoint(new(0, -lineLength));
            castMarker.Position += new Vector2(0, -lineLength / 4);
        }
        else if (player.FacingDirection == Vector2.Down)
        {
            fishingLine.AddPoint(new(0, lineLength));
            castMarker.Position += new Vector2(0, lineLength / 4);
        }
        else if (player.FacingDirection == Vector2.Right)
        {
            fishingLine.AddPoint(new(lineLength, 0));
            castMarker.Position += new Vector2(lineLength / 4, 0);
        }
        else
        {
            fishingLine.AddPoint(new(-lineLength, 0));
            castMarker.Position += new Vector2(-lineLength / 4, 0);
        }
    }

    private void EndFishingItemAction()
    {
        lineLength = 0; // reset length
        castMarker.Position = new Vector2(0, 0);
        castMarker.Visible = false;
    }
}