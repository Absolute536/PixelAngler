using System;
using Godot;

[GlobalClass]
public partial class FishSpecies : Resource
{
    [Export] public string SpeciesName;
    [Export] public int MovementSpeed;
    [Export] public Texture2D SpriteTexture;

    // requirement can be here as well
}
