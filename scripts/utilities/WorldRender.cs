namespace WorldRender;

using Godot;
using System;
using System.Collections.Generic;
using static TileConfiguration;
using static TileType;

public enum TileType
{
    Water,
    Dirt,
    Grass,
    Path,
    MountainLayer1,
    MountainLayer2,
    MountainLayer3
}

public enum TileConfiguration
{
    MajorTile,
    MinorTile
}

public class WorldRender
{
    private Vector2I _grassAtlas;

    readonly Dictionary<Tuple<TileConfiguration, TileConfiguration, TileConfiguration, TileConfiguration>, Vector2I> TileNeighbourhoodAtlas = new()
    {
        {new (MajorTile, MajorTile, MajorTile, MajorTile), new (0, 0)}, // All Corners
        {new (MajorTile, MinorTile, MinorTile, MinorTile), new (2, 2)}, // Top Left Corner
        {new (MinorTile, MajorTile, MinorTile, MinorTile), new (0, 2)}, // Top Right Corner
        {new (MinorTile, MinorTile, MajorTile, MinorTile), new (0, 0)}, // Bottom Right Corner
        {new (MinorTile, MinorTile, MinorTile, MajorTile), new (2, 0)}, // Bottom Left Corner
        {new (MajorTile, MajorTile, MinorTile, MinorTile), new (1, 2)}, // Top Side
        {new (MinorTile, MajorTile, MajorTile, MinorTile), new (0, 1)}, // Right Side
        {new (MajorTile, MinorTile, MinorTile, MajorTile), new (2, 1)}, // Left Side
        {new (MinorTile, MinorTile, MajorTile, MajorTile), new (1, 0)}, // Bottom Side
        {new (MajorTile, MajorTile, MinorTile, MajorTile), new (1, 4)}, // Empty Bottom Right Corner
        {new (MajorTile, MajorTile, MajorTile, MinorTile), new(0, 4)}, // Empty Bottom Left Corner
		{new (MajorTile, MinorTile, MajorTile, MajorTile), new(1, 3)}, // Empty Top Right Corner
		{new (MinorTile, MajorTile, MajorTile, MajorTile), new(0, 3)}, // Empty Top Left Corner
		{new (MajorTile, MinorTile, MajorTile, MinorTile), new(0, 5)}, // Left Diagonal Corners
		{new (MinorTile, MajorTile, MinorTile, MajorTile), new(0, 6)}, // Right Diagonal Corners
    };
}