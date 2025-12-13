using Godot;
using SignalBusNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class FishCatalogueUi : Control
{
    [Export] GridContainer FishSelectionButtonsContainer;
    [Export] TextureRect FishSpriteDisplay;
    [Export] Label SpeciesNameLabel;
    [Export] RichTextLabel RarityLabel;
    [Export] Label NumbersCaughtLabel;
    [Export] Label LargestCaughtLabel;
    [Export] Label SmallestCaughtLabel;
    [Export] Label LocationLabel;
    [Export] Label SpawnTimeLabel;
    [Export] Label SizeLabel;
    [Export] Label DietLabel;
    [Export] Label DescriptionLabel;
    [Export] Label HintLabel;

    [Export] ScrollContainer FishSelectionScrollContainer;
    [Export] ScrollContainer FishInformationScrollContainer;

    private List<CatalogueButton> FishSelectionButtons = new ();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
        // CallDeferred(Control.MethodName.GrabFocus);

        InitialiseFishCatalogue();
        SignalBus.Instance.CatchProgressUpdated += HandleProgressUpdate;
    }

    // use unhandled_input, cuz the child control doesn't consume the OpenCatalogue event. Using Gui_input doesn't work, cuz the children has focus, and theirs will be triggered instead
    // public override void _UnhandledInput(InputEvent @event)
    // {
        // if (@event.IsActionPressed("ShowCatalogue"))
        // {
        //     if (Visible)
        //     {
        //         Visible = false;
        //         ReleaseFocus();

        //         GetTree().Paused = false;
        //     }
		// 	    // Visible = false;
        //     // FocusMode = FocusModeEnum.None;
        // }
    // }

    public void ShowHideCatalogue()
    {
        if (Visible)
        {
            Visible = false;
            ReleaseFocus();
            GetTree().Paused = false;
            AudioManager.Instance.PlaySfx(this, SoundEffect.PaperPlacedDown, false);
        }
        else
        {
            GetTree().Paused = true;

            Visible = true;
            GrabFocus();
            FocusMode = FocusModeEnum.All;
            AudioManager.Instance.PlaySfx(this, SoundEffect.PaperFlip, false);

            UpdateSpeciesInformationDisplay(0); // show first one
            FishSelectionScrollContainer.ScrollVertical = 0; // fish buttons scroll to top
        }  
    }
    private void InitialiseFishCatalogue()
    {
        List<FishSpecies> allFishSpecies = FishRepository.Instance.FishSpeciesInformation;
        List<FishDescription> allSpeciesDesc = FishRepository.Instance.FishDescription;

        // Shader shadowShader = GD.Load<Shader>("res://resources/shaders/fish.gdshader"); // load first
        ShaderMaterial shadowShader = GD.Load<ShaderMaterial>("res://resources/shaders/fish_underwater_shader.tres");

        foreach (FishSpecies species in allFishSpecies)
        {
            CatalogueButton selectionButton = new ();
            selectionButton.ButtonId = species.FishId;

            // selectionButton.TextureNormal = species.SpriteTexture; // It works, but not as well as I think cuz of the sprite size, might need to create individual button?
            selectionButton.AddChild(selectionButton.IconSprite);
            selectionButton.IconSprite.Texture = species.SpriteTexture;
            selectionButton.IconSprite.SetAnchorsPreset(LayoutPreset.Center, true);
            selectionButton.IconSprite.CustomMinimumSize = new Vector2(species.SpriteTexture.GetSize().X, 16); // just use the original size, cuz 48, 48 can contain it?

            // OR, create the border, and put the fish as a textureRect like the sprite texture
            selectionButton.CustomMinimumSize = new Vector2(48, 48);
            selectionButton.FishSelectionChanged = UpdateSpeciesInformationDisplay; // just assign, cuz only 1

            selectionButton.MouseEntered += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);}; // enter only
            selectionButton.Pressed += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonClick, false);};

            FishSelectionButtons.Add(selectionButton);
            // shoudl be up there
            // so we're initialising the icons based on progress (which would be loaded from the save file already)
            if (FishRepository.Instance.FishCatchProgress[species.FishId].NumbersCaught == 0)
            {
                selectionButton.IconSprite.Material = shadowShader;
            }
                

            FishSelectionButtonsContainer.AddChild(selectionButton);
        }

        // For the information, just use the first one
        // FishSpecies firstFishSpecies = allFishSpecies[0];
        // FishSpriteDisplay.Texture = firstFishSpecies.SpriteTexture;
        // FishSpriteDisplay.CustomMinimumSize = firstFishSpecies.SpriteTexture.GetSize() * 2; // So (16, 16) --> (32, 32)
        // SpeciesNameLabel.Text = "Species: " + firstFishSpecies.SpeciesName;
        // NumbersCaughtLabel.Text = "Numbers Caught: " + 0; // change later
        // // LocationLabel.Text = "Location: " + firstFishSpecies.SpawnLocations.ToString(); // yeah probably won't work
        // LocationLabel.Text = FormatLocationString(firstFishSpecies);
        // SpawnTimeLabel.Text = FormatSpawnTimeString(firstFishSpecies);
        // SizeLabel.Text = "Size: " + firstFishSpecies.Size.ToString();
        // DietLabel.Text = "Diet: " + firstFishSpecies.DietType.ToString();
        // DescriptionLabel.Text = allSpeciesDesc[0].SpeciesDescription;
        // HintLabel.Text = allSpeciesDesc[0].Hint;
        UpdateSpeciesInformationDisplay(0); // can I just do this LMAO? Yep.
    }

    private void HandleProgressUpdate(int fishId, float sizeMeasurement)
    {
        // so this one will subscribe AFTER the repository to ensure the repository update is triggered first, then update here
        // Or.... we subscribe to a delegate from the repository itself?
        // yeah it works, just... not very good
        // it relies on the repository being invoked first? [YES] --> this works
        // Wait, not really, cuz the fishId information is propagated here too
        // just, I think it's probably better if it is encapsulated within the FishRepository
        CatalogueButton targetBtn = FishSelectionButtons[fishId];
        if (targetBtn.ButtonId == fishId && FishRepository.Instance.FishCatchProgress[fishId].NumbersCaught > 0)
        {
            targetBtn.IconSprite.Material = null;
        }
        // I guess this is safe?
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

    private void UpdateSpeciesInformationDisplay(int buttonId)
    {
        FishInformationScrollContainer.ScrollVertical = 0; // scroll to the top 

        List<FishSpecies> speciesList = FishRepository.Instance.FishSpeciesInformation;
        List<FishDescription> descList = FishRepository.Instance.FishDescription;

        FishSpecies targetSpecies = speciesList[buttonId];
        FishSpriteDisplay.Texture = targetSpecies.SpriteTexture;

        FishCatchRecord record = FishRepository.Instance.FishCatchProgress[targetSpecies.FishId];
        if (record.NumbersCaught == 0)
        {
            // ok, probably gonna try to use the shader or the hard way is to create versions of the sprite
            // FishSpriteDisplay.Texture = 
            // ahh, guess it'll just make it a texture rect like the display one
            // then the button's texture can have a proper border (yeah.)
            FishSpriteDisplay.Material = new ShaderMaterial() {Shader = GD.Load<Shader>("res://resources/shaders/fish.gdshader")};
            SpeciesNameLabel.Text = "Species: Undiscovered";
            RarityLabel.Text = "Rarity: Undiscovered";
            LocationLabel.Text = "Locations: \n    Undiscovered";
            SpawnTimeLabel.Text = "Active Times: \n    Undiscovered";
            SizeLabel.Text = "Size: " + "Undiscovered";
            DietLabel.Text = "Diet: " + "Undiscovered";
            DescriptionLabel.Text = "Undiscovered.";
        }
        else
        {
            FishSpriteDisplay.Material = null; // remove the shader
            FishSpriteDisplay.CustomMinimumSize = targetSpecies.SpriteTexture.GetSize() * 2; // So (16, 16) --> (32, 32)
            SpeciesNameLabel.Text = "Species: " + targetSpecies.SpeciesName;
            RarityLabel.Text = FormatRarityString(targetSpecies);
            LocationLabel.Text = FormatLocationString(targetSpecies);
            SpawnTimeLabel.Text = FormatSpawnTimeString(targetSpecies);
            SizeLabel.Text = "Size: " + targetSpecies.Size.ToString();
            DietLabel.Text = "Diet: " + targetSpecies.DietType.ToString();
            DescriptionLabel.Text = descList[buttonId].SpeciesDescription;
        }

        NumbersCaughtLabel.Text = "Numbers Caught: " + record.NumbersCaught.ToString(); // it works
        LargestCaughtLabel.Text = "Largest Caught: " + $"{record.LargestCaught:F2}m";
        SmallestCaughtLabel.Text = "Smallest Caught: " + $"{record.SmallestCaught:F2}m";
        HintLabel.Text = descList[buttonId].Hint; // hint must always be visible
    }

    private string FormatRarityString(FishSpecies fish)
    {
        if (fish.Rarity == FishRarity.Common)
            return "Rarity: [color=4185c0]Common[/color]";
        else if (fish.Rarity == FishRarity.Uncommon)
            return "Rarity: [color=2aa565]Uncommon[/color]";
        else
            return "Rarity: [color=bf333b]Rare[/color]";
        
        //4185c0 for common (pulse 35569a)
    }

    private string FormatSpawnTimeString(FishSpecies fish)
    {
        StringBuilder spawnTime = new ();
        spawnTime.Append("Active Times: \n");
        
        List<string> timeConditions = new ();
        if (fish.IsDayActive) {timeConditions.Add("Day");}
        if (fish.IsNightActive) {timeConditions.Add("Night");}
        if (fish.IsDawnActive) {timeConditions.Add("Dawn");}
        if (fish.IsDuskActive) {timeConditions.Add("Dusk");}

        for (int i = 0; i < timeConditions.Count; i++)
        {
            spawnTime.Append("    " + timeConditions[i]);

            if (i != timeConditions.Count - 1)
                spawnTime.Append("\n");
        }


        return spawnTime.ToString();
    }

    private string FormatLocationString(FishSpecies fish)
    {
        StringBuilder location = new ();
        location.Append("Locations: \n");
        
        for (int i = 0; i < fish.SpawnLocations.Length; i++)
        {
            location.Append("    " + fish.SpawnLocations[i]); // somehow \t doesn't work

            if (i != fish.SpawnLocations.Length - 1)
                location.Append("\n");
        }

        return location.ToString();
    }


}
