using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public partial class SaveLoadUtil : Node
{
    // remember to autoload, now working on adding the random size parameter
    public static SaveLoadUtil Instance {get; private set;}

    // It's either I let others modify this copy OR I have another copy for others, and this one's get; only? (probably slightly safe that way)
    public GameSave LoadedGameSave {get => _loadedGameSave;} // wait external classes can still modify the properties in this one (Dang it)
    private GameSave _loadedGameSave;

    // public GameSave CurrentGameSave {get => _currentGameSave;}
    // private GameSave _currentGameSave;

    public int SaveStatePerformed = 0; // check against this?

    public override void _Ready()
    {
        Instance = this;
        LoadGameStateFromFile();
    }

    // This one is to handle quit request from OS (like clicking on the X button)
    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            GetTree().CallGroup("PersistentState", "SaveState");
            SaveGameStateToFile();
            GetTree().Quit();
        }
    }

    private void LoadGameStateFromFile()
    {
        string jsonString;
        try
        {
            // Parse the json string from user:// if exist
            using Godot.FileAccess jsonFile = Godot.FileAccess.Open("user://game_save/save_data.json", Godot.FileAccess.ModeFlags.Read);
            jsonString = jsonFile.GetAsText();
            
            // Idiot, need to deserialise here as well (I am so STUPID)
            _loadedGameSave = JsonSerializer.Deserialize<GameSave>(jsonString);
        } catch (Exception)
        {
            // if file not exist (exception caught), read from the template instead, and create the directory and save file at the user:// location
            GD.PushError("File does not exist, loading save file template.");

            using Godot.FileAccess templateJsonFile = Godot.FileAccess.Open("res://resources/save_data_template.json", Godot.FileAccess.ModeFlags.Read);
            // obtain the template jsonString and deserialise it into _loadedGameSave
            jsonString = templateJsonFile.GetAsText();
            _loadedGameSave = JsonSerializer.Deserialize<GameSave>(jsonString);

            DirAccess.MakeDirAbsolute(ProjectSettings.GlobalizePath("user://game_save"));
            
            // using keyword - so that it's disposed even if exception
            using Godot.FileAccess fileAccess = Godot.FileAccess.Open("user://game_save/save_data.json", Godot.FileAccess.ModeFlags.Write);

            // so after the above line, the file should be created already
            // now, do I use godot's method or C#'s method? (c# first I guess)

            // hmm, doesn't work cuz it's being used by Godot's process? (Yup)
            // File.WriteAllText(ProjectSettings.GlobalizePath("user://game_save/save_data.json"), jsonString); // write the template jsonString
            fileAccess.StoreString(jsonString);
        }
    }

    public void SaveGameStateToFile()
    {
        // hmm, then on save & quit click, call group their SaveState() function, they will modify a field in CurrentGameState
        // then how to wait until everyone's done before writing to file?
        // point to the user:// straight away, cuz it's guaranteed to be created on game load
        string jsonString = JsonSerializer.Serialize<GameSave>(_loadedGameSave);
        using Godot.FileAccess saveFile = Godot.FileAccess.Open("user://game_save/save_data.json", Godot.FileAccess.ModeFlags.Write);
        saveFile.StoreString(jsonString);
        // File.WriteAllText(ProjectSettings.GlobalizePath("user://game_save/save_data.json"), jsonString);

        // so every node that saves will increment save state performed by 1 (race condition? probably ok I hope, cuz single thread?)
        // then we wait by checking the save performed against the node count in the group (idk if this will cause freezing though)
        // maybe we can check in the physics process or sth like that (pooling 60 times per second)

        // await Task<bool> something something
        // OH OK!!! events are synchronous, so that means I can have persistent classes subscribe to an event
        // raise the event on quit button pressed, then call this method
        // But how about call group? is it synchronous?
        // it says "acts immediately on all selected nodes at once, which may cause stuttering in some performance-intensive situations"
        // so probably? (and there is a DEFERRED flag, not default)
        // while (SaveStatePerformed < GetTree().GetNodeCountInGroup("PresistentGroup"));
    }
}

public class GameSave
{
    [JsonInclude]
    public float PlayerGlobalPositionX {get; set;}
    [JsonInclude]
    public float PlayerGlobalPositionY {get; set;}
    [JsonInclude]
    public float MinutesPassed {get; set;} // roughly can accomodate up to 550k + days
    [JsonInclude]
    public List<FishCatchRecord> CatchRecords {get; set;}

}

public class FishCatchRecord
{
    [JsonInclude]
    public int FishId {get; set;}
    [JsonInclude]
    public int NumbersCaught {get; set;}
    [JsonInclude]
    public float LargestCaught {get; set;}
    [JsonInclude]
    public float SmallestCaught {get; set;}
}