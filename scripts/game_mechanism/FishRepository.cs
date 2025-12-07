using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

public partial class FishRepository : Node
{
    // so make this an autoload as well, but need to take note of the order
    public static FishRepository Instance {get; private set;}

    public List<FishSpecies> FishSpeciesInformation {get => _fishSpeciesInformation;}
    private List<FishSpecies> _fishSpeciesInformation = new ();

    public List<FishDescription> FishDescription {get => _fishDescriptions;}
    private List<FishDescription> _fishDescriptions = new ();
    
    public override void _Ready()
    {
        Instance = this;
        InitialiseFishRepository();
    }

    private void InitialiseFishRepository()
    {
        // load the resouce files
        // reference: https://docs.godotengine.org/en/stable/classes/class_projectsettings.html#class-projectsettings-method-globalize-path
        // so we convert the "local" path to an absolute path in OS

        // just use res:// for now OR not?
        // string path = OS.HasFeature("editor") ? "res://resources/fish_species" : GetAbsolutePathToResourcesDirectory();

        // Initialise the FishSpecies resources list
        string resourcePath = GetAbsolutePathToResourcesDirectory();
        int speciesCount = Directory.GetFiles(resourcePath).Length - 2; // minue 2 for now cuz of the test file and the json file (move later?)
        
        for (int i = 0; i < speciesCount; i++)
        {
            // just use res:// for now
            FishSpecies speciesInfo = GD.Load<FishSpecies>($"res://resources/fish_species/fish_species_{i}.tres");
            // https://www.reddit.com/r/godot/comments/13u9w0j/loading_resources_in_exported_projects/ (handy for export)
            // FishSpecies speciesInfo = GD.Load<FishSpecies>(resourcePath.PathJoin($"fish_species_{i}.tres"));

            // test for export
            // if (OS.HasFeature("windows"))
            // {
            //     ConfigFile remapFile = GD.Load<ConfigFile>(resourcePath.PathJoin($"fish_species_{i}.tres.remap"));
            //     speciesInfo = GD.Load<FishSpecies>(remapFile.GetValue("remap", "path").ToString());
            //     GD.Print(remapFile.GetValue("remap", "path").ToString());
            // }
            _fishSpeciesInformation.Add(speciesInfo);
        }

        // Initialise the description list
        string jsonString = File.ReadAllText(GetAbsolutePathToDescJSONDirectory());
        // GD.Print(jsonString);
        FishSpeciesDescriptionJSON descriptionJson = JsonSerializer.Deserialize<FishSpeciesDescriptionJSON>(jsonString); // apparently '!' means null forgiving operator
        GD.Print(descriptionJson.WrittenDescription.Count);
        foreach (FishDescription fishDesc in descriptionJson.WrittenDescription)
        {
            _fishDescriptions.Add(fishDesc);
        }
    }

    private string GetAbsolutePathToResourcesDirectory()
    {   
        // If running from editor, convert using "res://"
        if (OS.HasFeature("editor"))
        {
            return ProjectSettings.GlobalizePath("res://resources/fish_species");
        }
        else // this is for exported
        {
            return OS.GetExecutablePath().GetBaseDir().PathJoin("resources/fish_species");
        }
    }

    private string GetAbsolutePathToDescJSONDirectory()
    {   
        // If running from editor, convert using "res://"
        if (OS.HasFeature("editor"))
        {
            return ProjectSettings.GlobalizePath("res://resources/fish_species/fish_species_desc.json");
        }
        else // this is for exported
        {
            return OS.GetExecutablePath().GetBaseDir().PathJoin("resources/fish_species/fish_species_desc.json");
        }
    }

    public List<FishSpecies> FilterSpeciesByTimeAndLocation(TimeOfDay time, string location, FishRarity rarity)
    {
        return time switch // oh wow, switch expression huh (like a functional form) good job VSC
        {
            TimeOfDay.Day => _fishSpeciesInformation.FindAll(x => x.IsDayActive && x.Rarity == rarity && x.SpawnLocations.Contains(location)),
            TimeOfDay.Night => _fishSpeciesInformation.FindAll(x => x.IsNightActive && x.Rarity == rarity && x.SpawnLocations.Contains(location)),
            TimeOfDay.Dusk => _fishSpeciesInformation.FindAll(x => x.IsDuskActive && x.Rarity == rarity && x.SpawnLocations.Contains(location)),
            TimeOfDay.Dawn => _fishSpeciesInformation.FindAll(x => x.IsDawnActive && x.Rarity == rarity && x.SpawnLocations.Contains(location)),
            _ => [],// return empty list as default case, though it will never happen LMAO (cuz enum right?)
        };
    }
}

// Class representing the species description's JSON
public class FishSpeciesDescriptionJSON
{
    [JsonInclude]
    public IList<FishDescription>? WrittenDescription = new List<FishDescription>();
}

// Class containing the information on each fish's description
public class FishDescription
{
    [JsonInclude]
    public int FishId {get; set;}
    [JsonInclude]
    public string SpeciesDescription {get; set;}
    [JsonInclude]
    public string Hint {get; set;}
}