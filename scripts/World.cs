namespace GameWorld;

using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using static TileConfig;
using static TileType;
using GamePlayer;
using SignalBusNS;

// IMPORTANT!!!
// Remember to have a 1-tile distance between > 2 different tile types 
// Because of the limitation here (only 2-tile configuration sprites are drawn)

public enum TileConfig
{
	Primary,
	Secondary
}

// Tiles that are higher (higher z-index ish~) will be the PRIMARY over its counterpart -> greater value
// Should flip the enum definition order so that they are automatically ordered
public enum TileType
{
	Water,
	Dirt,
	Grass,
	Path,
	// Damn these MountainLayer shites are hacky af (cuz I insisted on using 1 TileMapLayer for the terrain) Why so stupid?
	MountainLayer1,
	MountainLayer2,
	MountainLayer3
}

public partial class World : Node2D
{
	[Export] TileMapLayer DisplayLayer;
	[Export] TileMapLayer WorldLayer;
	[Export] Vector2I GrassTileWorldAtlas;
	[Export] Vector2I DirtTileWorldAtlas;
	[Export] Vector2I PathTileWorldAtlas;
	[Export] Vector2I WaterTileWorldAtlas;
	[Export] Vector2I MountainLayerOneWorldAtlas;
	[Export] Vector2I MountainLayerTwoWorldAtlas;
	[Export] Vector2I MountainLayerThreeWorldAtlas;

	private Player PPlayer;

	private const int TileNeighbourhoodSize = 4;

	// Array of the 4 neighbourhood calculation vectors
	// Used for calculating (converting) world coordinate to display coordinate and vice versa
	// From world to display, use the vector at the opposite side instead, as the directions are flipped
	// (0, 0) is for top left on display OR bottom right for world, but it's somewhat trivial

	// [World to Display] top left, bottom left, top right, bottom right
	// [Display to World] bottom right, top right, bottom left, top left
	readonly Vector2I[] NEIGHBOURS = [new(1, 1), new(0, 1), new(0, 0), new(1, 0)];

	// readonly Dictionary<string, Vector2I> NeighbourCalculationVectors = new()
	// {
	// 	{"TopLeftDisplayVector", new (0, 0)}, // Bottom Right for World (kinda trivial)
	// 	{"BottomLeftDisplayVector", new (0, 1)}, // Top Right for World
	// 	{"TopRightDisplayVector", new (1, 0)}, // Bottom Left for World
	// 	{"BottomRightDisplayVector", new (1, 1)} // Top Left for World
	// };

	// Dictionary of 4-neighbour TileType -> Atlas coordinate of the tile to be selected
	// TileConfig starts from the top left corner and go clockwise
	readonly Dictionary<Tuple<TileConfig, TileConfig, TileConfig, TileConfig>, Vector2I> neighbourhoodRules = new() {
		{new (Primary, Primary, Primary, Primary), new(0 , 0)}, // Full Primary
		{new (Primary, Secondary, Secondary, Secondary), new(2, 2)}, // Top Left Corner
		{new (Secondary, Primary, Secondary, Secondary), new(0, 2)}, // Top Right Corner
		{new (Secondary, Secondary, Primary, Secondary), new(0, 0)}, // Bottom Right Corner
		{new (Secondary, Secondary, Secondary, Primary), new(2, 0)}, // Bottom Left Corner
		{new (Primary, Primary, Secondary, Secondary), new(1, 2)}, // Top Side
		{new (Secondary, Primary, Primary, Secondary), new(0, 1)}, // Right Side
		{new (Primary, Secondary, Secondary, Primary), new(2, 1)}, // Left Side
		{new (Secondary, Secondary, Primary, Primary), new(1, 0)}, // Bottom Side
		{new (Primary, Primary, Secondary, Primary), new(1, 4)}, // Empty Bottom Right Corner
		{new (Primary, Primary, Primary, Secondary), new(0, 4)}, // Empty Bottom Left Corner
		{new (Primary, Secondary, Primary, Primary), new(1, 3)}, // Empty Top Right Corner
		{new (Secondary, Primary, Primary, Primary), new(0, 3)}, // Empty Top Left Corner
		{new (Primary, Secondary, Primary, Secondary), new(0, 5)}, // Left Diagonal Corners
		{new (Secondary, Primary, Secondary, Primary), new(0, 6)}, // Right Diagonal Corners
	};

	// aMaZiNg HaCk TAT AHHHHHHHHHHHHHHHHHHHHHHHH!!!!
	// Because I want to have everything inside a single TileMapLayer, so we had to use MoUnTaInLaYeR{#}
	// And manually specify the tile combination of each level with the higher/lower level
	// AND THEY ALL POINT TO THE SAME TILE SOURCE (aMaZiNg)
	readonly Dictionary<(TileType primaryTile, TileType secondaryTile), int> tileCombinationSource = new()
	{
		{new (Grass, Water), 0},
		{new (Water, Water), 2},
		{new (Grass, Grass), 1},
		{new (Grass, Dirt), 4},
		{new (Dirt, Dirt), 3},
		{new (Dirt, Water), 5},
		{new (Path, Path), 6},
		{new (Path, Grass), 7},
		{new (MountainLayer1, Grass), 8},
		{new (MountainLayer1, MountainLayer1), 1},
		{new (MountainLayer2, MountainLayer1), 8},
		{new (MountainLayer2, Grass), 8},
		{new (MountainLayer2, MountainLayer2), 1},
		{new (MountainLayer3, MountainLayer2), 8},
		{new (MountainLayer3, Grass), 8},
		{new (MountainLayer3, MountainLayer3), 1}
	};


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// worldAtlasTileType.Add(GrassTileWorldAtlas, Grass);
		// worldAtlasTileType.Add(DirtTileWorldAtlas, Dirt);
		// worldAtlasTileType.Add(WaterTileWorldAtlas, Water);
		// worldAtlasTileType.Add(PathTileWorldAtlas, Path);

		if (!Engine.IsEditorHint())
		{
			PPlayer = GetNode<CharacterBody2D>("WorldEntities/Player") as Player;

			foreach (Vector2I worldCoord in WorldLayer.GetUsedCells())
			{
				PopulateDisplayTile(worldCoord);
			}

			// SignalBus.Instance.PositionChanged += GetTileType;
			// PPlayer.LocationChanged = OnPlayerLocationChanged;
		}
	}

	/*
	 * Let's Note down the things we need to do here and see how to improve it
	 * 1. Iterate through the used cells in WorldLayer (populated tiles)
	 * 2. Get the position (coordinate in Vector2I) of each tile
	 * 3. Use the information to determine the location and type of the corresponding 4 (neighbour) tiles in DisplayLayer
	 * 4. Populate those neighbour tiles in DisplayLayer
	 * 5. Repeat until all tiles are populated
	 */

	// Populate tiles of the 4 neighbour tiles in DisplayLayer
	private void PopulateDisplayTile(Vector2I worldLayerCoordinate)
	{
		// Calculate coordinate of each of the neighbours
		Vector2I[] neighbourDisplayCoords = CalculateDisplayLayerNeighbourCoordinates(worldLayerCoordinate);

		// Determine the tile type of he 4 corners in each neighbour
		foreach (Vector2I neighbourDisplayCoord in neighbourDisplayCoords)
		{
			TileType[] cornerTileTypes = CalculateCornerTileTypes(neighbourDisplayCoord);

			// Determine the display tile atlas coordinate
			Vector2I displayLayerAtlasCoord = CalculateTileConfigurationNeighbourhoodAtlas(cornerTileTypes);

			// Determine the tile set source id from the corner tile combination
			int tileSetSourceId = CalculateTileSetSourceId(cornerTileTypes);

			// Populate the cell with the designated tile based on calculation results
			DisplayLayer.SetCell(neighbourDisplayCoord, tileSetSourceId, displayLayerAtlasCoord);
		}
	}

	private Vector2I[] CalculateDisplayLayerNeighbourCoordinates(Vector2I worldLayerCoordinate)
	{
		Vector2I[] displayLayerNeighbourCoords = new Vector2I[4]; // Shoudl I use NEIGHBOURS.Length or 4? Hmm
		for (int i = 0; i < displayLayerNeighbourCoords.Length; i++)
			displayLayerNeighbourCoords[i] = worldLayerCoordinate + NEIGHBOURS[i];

		return displayLayerNeighbourCoords;
	}

	private TileType[] CalculateCornerTileTypes(Vector2I displayLayerCoordinate)
	{
		TileType[] cornerTileTypes = new TileType[4];
		for (int i = 0; i < cornerTileTypes.Length; i++)
			cornerTileTypes[i] = WorldCoordToTileType(displayLayerCoordinate - NEIGHBOURS[i]); // Also start from top left and go clockwise

		return cornerTileTypes;
	}

	private Vector2I CalculateTileConfigurationNeighbourhoodAtlas(TileType[] cornerTileTypes)
	{
		TileType primaryTile = cornerTileTypes.Max();

		TileConfig[] tileTypeConfiguration = new TileConfig[cornerTileTypes.Length];

		for (int i = 0; i < cornerTileTypes.Length; i++)
		{
			if (cornerTileTypes[i] == primaryTile)
				tileTypeConfiguration[i] = Primary;
			else
				tileTypeConfiguration[i] = Secondary;
		}

		Tuple<TileConfig, TileConfig, TileConfig, TileConfig> tileConfiguration = new(tileTypeConfiguration[0], tileTypeConfiguration[1], tileTypeConfiguration[2], tileTypeConfiguration[3]);

		return neighbourhoodRules[tileConfiguration];
	}
	private int CalculateTileSetSourceId(TileType[] cornerTileTypes)
	{
		TileType primaryTile = cornerTileTypes.Max();
		TileType secondaryTile = cornerTileTypes.Min();

		return tileCombinationSource[new(primaryTile, secondaryTile)];
	}

	/*
	 * THE TASK AT HAND
	 * From the world coornidate, convert to display coordinate
	 * Using display coordinate, find out the TileType of the 4 corners (4 neighbouring tiles in world layer)
	 * The problem now is how to determine which of the two TileType found from the display coord is PRIMARY & SECONDARY
	 * If only a single TileType is found (full tile), should it be primary or secondary?
	 */

	private TileType WorldCoordToTileType(Vector2I worldCoord)
	{
		Vector2I worldAtlas = WorldLayer.GetCellAtlasCoords(worldCoord);
		// Console.WriteLine(worldCoord);

		if (worldAtlas == GrassTileWorldAtlas)
			return Grass;
		else if (worldAtlas == WaterTileWorldAtlas)
			return Water;
		else if (worldAtlas == DirtTileWorldAtlas)
			return Dirt;
		else if (worldAtlas == PathTileWorldAtlas)
			return Path;
		else if (worldAtlas == MountainLayerOneWorldAtlas)
			return MountainLayer1;
		else if (worldAtlas == MountainLayerTwoWorldAtlas)
			return MountainLayer2;
		else if (worldAtlas == MountainLayerThreeWorldAtlas)
			return MountainLayer3;
		else
			return Grass;
	}
	
	public TileType GetTileTypeFromPosition(Vector2 position)
    {
		Vector2I worldCoord = WorldLayer.LocalToMap(position); // In this case, player's position == GlobalPosition (so no need to convert) kinda fragile though
		return WorldCoordToTileType(worldCoord);
    }

	public string GetWorldLocationFromPosition(Vector2 position)
	{
		Vector2I worldCoord = WorldLayer.LocalToMap(position);
		TileData tileData = WorldLayer.GetCellTileData(worldCoord);
		if (tileData != null)
			if (tileData.HasCustomData("Location"))
				return tileData.GetCustomData("Location").ToString();

		return "Location Not Found.";
	}
}
