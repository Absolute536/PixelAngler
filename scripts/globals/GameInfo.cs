using GameWorld;
using Godot;

public partial class GameInfo : Node
{
    public static GameInfo Instance { get; private set; }

    private World gameWorld;

    public override void _Ready()
    {
        Instance = this; // assign static property as the autoloaded node (point to the current node in scene tree)
        gameWorld = GetTree().GetFirstNodeInGroup("World") as World;
    }

    public TileType GetTileType(Vector2 position)
    {
        return gameWorld.GetTileTypeFromPosition(position);
    }

    public string GetWorldLocation(Vector2 position)
    {
        return gameWorld.GetWorldLocationFromPosition(position);
    }

}