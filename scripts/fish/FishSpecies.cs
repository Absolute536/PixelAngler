using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class FishSpecies : Resource
{
    [Export] public int FishId; // ID for the species
    [Export] public string SpeciesName; // Species name of the fish
    [Export] public int MovementSpeed; // Umm, maybe movement speed can be here to give each fish a more distinctive "personality"?
    [Export] public Texture2D SpriteTexture; // Sprite Texture

    [Export] public FishRarity Rarity;
    [Export] public SizeClass Size;
    [Export] public FishDiet DietType;

    // Cuz we only need to check for hours before spawnning
    // Diurnal (Day only)
    // Noctural (Night only)
    // Crepuscular (Dawn & Dusk only)
    // Cathemeral (Anytime)

    // 0300 - 0659 -> Dawn
    // 0700 - 1659 -> Day
    // 1700 - 2059 -> Dusk
    // 2100 - 0259 -> Night

    // Modular arithmetic
    // f (n) = (n + 1) mod k
    // (00 ~ 24) mod 24, so that 24 wrap back to 00
    
    // current hour, if >= min spawn, else check if < max
    // [Export] public TimeOfDay[] SpawnTimes; -- doesn't work, can't export
    // [Export] public Godot.Collections.Array<TimeOfDay> SpawnTimes; -- it works, but I don't like it
    // Guess we'll just make do with 4 booleans
    [Export] public bool IsDayActive;
    [Export] public bool IsNightActive;
    [Export] public bool IsDuskActive;
    [Export] public bool IsDawnActive;

    [Export] public string[] SpawnLocations;

    [Export] public float Aggresiveness;

    // requirement can be here as well
}

public enum FishRarity
{
    Common,
    Uncommon,
    Rare
}

public enum SizeClass
{
    Small,
    Medium,
    Large
}

public enum FishDiet
{
    Herbivorous,
    Carnivorous,
    Omnivorous
}

public enum BaitType
{
    SmallHerbivoreBait,
    LargeHerbivoreBait,
    SmallCarnivoreBait,
    LargeCarnivoreBait
}
