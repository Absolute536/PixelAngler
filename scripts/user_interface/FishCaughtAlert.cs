using Godot;
using System;

public partial class FishCaughtAlert : Control
{
	[Export] public Label SpeciesName;
	[Export] public TextureRect SpeciesSprite;
	[Export] public Label SizeLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

}
