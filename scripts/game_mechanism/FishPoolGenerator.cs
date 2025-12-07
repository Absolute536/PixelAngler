using Godot;
using System;
using System.Collections.Generic;

public partial class FishPoolGenerator
{
    public List<FishSpecies> GetRandomFishPool(string spawnLocation)
    {
        FishRarity rarity = GetRandomRarity();
        TimeOfDay time = InGameTime.Instance.GetCurrentTimeOfDay();

        return FishRepository.Instance.FilterSpeciesByTimeAndLocation(time, spawnLocation, rarity);
    }

    private FishRarity GetRandomRarity()
    {
        float randomfloat =  GD.Randf();

        // Literally just Godot's documentation example (good distribution though)
        if (randomfloat < 0.8f)
            return FishRarity.Common; // 80%
        else if (randomfloat < 0.95f)
            return FishRarity.Uncommon; // 15%
        else
            return FishRarity.Rare; //5%
    }
}