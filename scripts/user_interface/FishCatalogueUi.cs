using Godot;
using System;

public partial class FishCatalogueUi : Control
{
    [Export] GridContainer FishSelectionButtonsContainer;
    [Export] TextureRect FishSpriteDisplay;
    [Export] Label SpeciesNameLabel;
    [Export] Label NumbersCaughtLabel; // this one need more work (haven't implemented yet) along with the locked unlocked tracker (it's be 0 for now)
    [Export] Label LocationLabel;
    [Export] Label SpawnTimeLabel;
    [Export] Label BaitLabel;
    [Export] Label DescriptionLabel;
    [Export] Label HintLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        // CallDeferred(Control.MethodName.GrabFocus);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (@event.IsActionPressed("OpenCatalogue"))
        {
            if (Visible)
            {
                Visible = false;
                ReleaseFocus();
            }
			    // Visible = false;
            // FocusMode = FocusModeEnum.None;
        }
    }

    private void HandleCatalogueOpened(object sender, EventArgs e)
    {
        // on every open, need to populate the interactable elements (NOPE)

        // FishIconButtons (Grid Container)
        // Button minSize = 48 * 48

        // For the descriptions, need to populate per button press, but for the first one, just make it the first fish
        // so each button need to have like a reference to each of the description field and populate on pressed?
        // ok or we use signal emit from each button on pressed to the root node to change the fields dynamically?
        // and umm, yeah, probably bettern to build a dictionary for faster access? (but uses more memory though)

        // I mean, actually can just make it an array and access by the index since fish id always starts at zero
        // and they are ordered from 0 to the last fish

        // so let's try creating exported properties for the GridContainer of buttons
        // and also for each field in the description column (next page)

        // then, create a inherited button class from texture button, having properties to represent the fish id that it contains
        // also have a method to subscribe to the each button's pressed signal or other function to refresh the desc (sth like that) on each button press
        // either we use build a dictionary here, or we use the Find function to get the element in the list (let's try find for now, don't need another dict)

        // WAIT, we don't need to perform this on open, just execute it after loading the FishRepository (yeah, cuz it won't ever change, unless we modify the list)
        // BUT, still probably need to use some sort of signal to handle the UI open properly
        // but it'll do for now
        // hmm... then do we even need the autoload?
        // we can probably just make it a regular node in the scene, and reference it here, since child need to be ready before parent is ready
        // FUCKING IDIOT, WE'VE ALREADY TALK THIS THROUGH BEFORE
        // Need to be autoload, since other systems will rely on this and need to access the information
        // JUST, we only need to execute these on ready to establish the content of the catalogue
    }


}
