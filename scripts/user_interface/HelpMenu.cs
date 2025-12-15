using Godot;
using System;

public partial class HelpMenu : Control
{
	[Export] public RichTextLabel LeftPage;
	[Export] public RichTextLabel RightPage;

	[Export] public Button PreviousPage;
	[Export] public Button NextPage;

	[Export] public TextureButton CloseButton;

	// I'll just write all the contents here just to finish it (should be from some text file I guess)
	private readonly Tuple<string, string>[] PageContents =
	[
		new (
			""" 
			[center]~ Controls ~[/center]

			[ul] W [Move Up][/ul]
			[ul] A [Move Left][/ul]
			[ul] S [Move Down][/ul]
			[ul] D [Move Right][/ul]

			[ul] Left Click [Fishing][/ul]
			[ul] Right Click [Fishing][/ul]
			[ul] Scroll [Swap Bait][/ul]

			[ul] C [Show Catalogue][/ul]
			[ul] Esc [Pause Menu][/ul]
			""",
			"""
			[center]~ How to Fish 1 ~[/center]

			Left click & Release to cast while standing still. Hold left click to cast further.

			Left click while no fish is hooked to cancel the cast.

			Bobber can only be cast into water bodies.
			"""
		),
		new (
			""" 	
			[center]~ How to Fish 2 ~[/center]

			Fish will notice the cast bait and will be attracted to nibble at it if it SATISFIES their preferences.

			Left click at the precise timing when the fish bites to hook the fish.

			Missing the timing results in the bait being eaten, requiring another cast.
			""",
			"""
			[center]~ How to Fish 3 ~[/center]

			To catch a fish, perform various actions based on its behaviour (colour prompt):

			When the fish is [color=LimeGreen]hooked[/color], hold left click.

			When the fish is [color=Gold]wrestling[/color], left click repeatedly.

			When the fish is [color=Crimson]diving[/color], hold right click.
			"""			
		),
		new (
			""" 	
			[center]~ How to Fish 4 ~[/center]

			A fish's preference is defined by their diet type and size.

			Swap between various baits and use the correct one to discover the preferences of each fish.
			""",
			"""
			[center]~ How to Fish 5 ~[/center]

			[center]
			6 Available Baits
			[/center]
			For Carnivores:
			* Worm
			* Small Fish
			* Fish Flesh Chunk

			For Herbivores:
			* Pea
			* Walnut
			* Bread
			"""	
		),
		new (
			""" 	
			[center]~ How to fish 6 ~[/center]

			The catch progress of each fish species is recorded in the catalogue.

			Various information of every fish species discovered is documented in the catalgoue.

			[color=MediumBlue]Quick Tip:
			Hints to finding undiscovered fish species can be found in the catalogue.[/color]
			""",
			"""
			[center]~ Afterword ~[/center]
			
			Go explore the islands and all the fishing locations.

			Fish and discover the various fish species the game has to offer.

			There is no definite goal in this game. Just enjoy (I hope....).
			"""	
		)
	];

	private int _currentPage = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PreviousPage.Disabled = true;
		
		PreviousPage.Pressed += () =>
		{
			NextPage.Disabled = false;

			if (_currentPage > 0)
			{
				AudioManager.Instance.PlaySfx(this, SoundEffect.PaperFlip, false);
				_currentPage -= 1;
			}
			
			if (_currentPage == 0) 
				PreviousPage.Disabled = true;
			
			LeftPage.Text = PageContents[_currentPage].Item1;
			RightPage.Text = PageContents[_currentPage].Item2;

			
		};

		NextPage.Pressed += () =>
		{
			PreviousPage.Disabled = false;

			if (_currentPage < PageContents.Length - 1)
			{
				_currentPage += 1;
				AudioManager.Instance.PlaySfx(this, SoundEffect.PaperFlip, false);
			}
				
			if (_currentPage == PageContents.Length - 1) 
				NextPage.Disabled = true;
			
			LeftPage.Text = PageContents[_currentPage].Item1;
			RightPage.Text = PageContents[_currentPage].Item2;
		};

		CloseButton.MouseEntered += () => {AudioManager.Instance.PlaySfx(this, SoundEffect.ButtonHover, false);};
		CloseButton.Pressed += () => 
		{
			ShowHideHelpMenu();
			AudioManager.Instance.PlaySfx(this, SoundEffect.PaperPlacedDown, false);
		};
	}

	public void ShowHideHelpMenu()
	{
		if (!Visible)
		{
			GrabFocus();
			
			// Initialise the help menu (maybe can extract into another function)
			_currentPage = 0;
			LeftPage.Text = PageContents[_currentPage].Item1;
			RightPage.Text = PageContents[_currentPage].Item2;

			PreviousPage.Disabled = true;
			NextPage.Disabled = false;


			Visible = true;
		}
		else
		{
			ReleaseFocus();
			Visible = false;
		}
	}


}
