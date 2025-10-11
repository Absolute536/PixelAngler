using Godot;
using System;
using GameWorld;

public partial class GlobalManager : Node
{
    public static GlobalManager Agent { get; private set; }

    /*
     * List of Functions that should be accessible (exposed) globally
     * - Get TileType based on Position
     * - Get Location based on Position (custom data of tile)

     Or should I just straight up use group and access each one there?
     But I suppose having a Global manager would make things cleaner 
     */
    private World gameWorld;

    public override void _Ready()
    {
        Agent = this;
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