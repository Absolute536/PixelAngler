using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using static TileConfig;
using static TileType;

// IMPORTANT!!!
// Remember to have a 1-tile distance between > 2 different tile types 
// Because of the limitation here (only 2-tile configuration sprites are drawn)

public enum TileConfig {
	PRIMARY,
	SECONDARY
}

// Tiles that are higher (higher z-index ish~) will be the PRIMARY over its counterpart -> greater value
public enum TileType {
	MOUNTAIN = 50,
	GRASS = 40,
	PATH = 30, // probably should be the same as grass (we'll see later)
	DIRT = 20,
	WATER = 10,

}

public partial class World : Node2D
{
	[Export] TileMapLayer displayLayer;
	[Export] TileMapLayer worldLayer;
	[Export] Vector2I GrassTileWorldAtlas;
	[Export] Vector2I DirtTileWorldAtlas;
	[Export] Vector2I PathTileWorldAtlas;
	[Export] Vector2I WaterTileWorldAtlas;

	readonly Dictionary<Vector2I, TileType> worldAtlasTileType = [];

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
		{new (PRIMARY, PRIMARY, PRIMARY, PRIMARY), new(1, 1)}, // Full Primary
		{new (PRIMARY, SECONDARY, SECONDARY, SECONDARY), new(2, 2)}, // Top Left Corner
		{new (SECONDARY, PRIMARY, SECONDARY, SECONDARY), new(0, 2)}, // Top Right Corner
		{new (SECONDARY, SECONDARY, PRIMARY, SECONDARY), new(0, 0)}, // Bottom Right Corner
		{new (SECONDARY, SECONDARY, SECONDARY, PRIMARY), new(2, 0)}, // Bottom Left Corner
		{new (PRIMARY, PRIMARY, SECONDARY, SECONDARY), new(1, 2)}, // Top Side
		{new (SECONDARY, PRIMARY, PRIMARY, SECONDARY), new(0, 1)}, // Right Side
		{new (PRIMARY, SECONDARY, SECONDARY, PRIMARY), new(2, 1)}, // Left Side
		{new (SECONDARY, SECONDARY, PRIMARY, PRIMARY), new(1, 0)}, // Bottom Side
		{new (PRIMARY, PRIMARY, SECONDARY, PRIMARY), new(1, 4)}, // Empty Bottom Right Corner
		{new (PRIMARY, PRIMARY, PRIMARY, SECONDARY), new(0, 4)}, // Empty Bottom Left Corner
		{new (PRIMARY, SECONDARY, PRIMARY, PRIMARY), new(1, 3)}, // Empty Top Right Corner
		{new (SECONDARY, PRIMARY, PRIMARY, PRIMARY), new(0, 3)}, // Empty Top Left Corner
		{new (PRIMARY, SECONDARY, PRIMARY, SECONDARY), new(0, 5)}, // Left Diagonal Corners
		{new (SECONDARY, PRIMARY, SECONDARY, PRIMARY), new(0, 6)}, // Right Diagonal Corners
		{new (SECONDARY, SECONDARY, SECONDARY, SECONDARY), new(8, 3)} // Full Secondary
	};

	readonly Dictionary<Tuple<TileType, TileType>, int> tileCombinationSource = new()
	{
		{new (GRASS, WATER), 0}
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		worldAtlasTileType.Add(GrassTileWorldAtlas, GRASS);
		worldAtlasTileType.Add(DirtTileWorldAtlas, DIRT);
		worldAtlasTileType.Add(WaterTileWorldAtlas, WATER);
		worldAtlasTileType.Add(PathTileWorldAtlas, PATH);

		if (!Engine.IsEditorHint())
			foreach (Vector2I worldCoord in worldLayer.GetUsedCells())
			{
				ShowDisplayTile(worldCoord);
			}
	}

	private void ShowDisplayTile(Vector2I worldCoord)
	{
		// Random rand = new Random();
		// [Above] attempt to display variation in full tiles

		// const int topLeftDisplayVector = 0;
		// const int bottomLeftDisplayVector = 1;
		// const int topRightDisplayVector = 2;
		// const int bottomRightDisplayVector = 3;

		// Iterate through the 4 neighbours and display the 4 corners
		for (int i = 0; i < NEIGHBOURS.Length; i++)
		{
			Vector2I displayCoord = worldCoord + NEIGHBOURS[i];

			Tuple<int, Vector2I> displayInfo = CalculateDisplayTile(displayCoord);
			// baseDisplayLayer.SetCell(displayCoord, 0, new Vector2I(rand.Next(5), 0)); // Display the base (SECONDARY) tile layer (choose randomly)
			displayLayer.SetCell(displayCoord, displayInfo.Item1, displayInfo.Item2);

		}
	}

	private Tuple<int, Vector2I> CalculateDisplayTile(Vector2I displayCoord)
	{
		/*
		 * Get the TileType of the 4 corners of the display tile specified by the displayCoord
		 * Figure out what Tiles they are and which one is the PRIMARY & SECONDARY
		 */

		// Compute the TileType of the 4 corners and arrange into an array with the order [top left, top right, bottom right, bottom left]
		// This order aligns with the key in the neighbourhoodRules dictionary
		TileType[] tileTypes = new TileType[4];
		for (int i = 0; i < 4; i++)
			tileTypes[i] = WorldCoordToTileType(displayCoord - NEIGHBOURS[i]);

		// Find out which one is the PRIMARY & SECONDARY
		TileConfig[] tileConfigs = new TileConfig[4];
		TileType primaryTile = tileTypes.Max(); // Find TileType in array with Max value (PRIMARY)
		TileType secondaryTile = tileTypes.Min(); // Find TileType in array with Min value (SECONDARY)
		for (int i = 0; i < tileTypes.Length; i++)
		{
			if (tileTypes[i] == primaryTile)
				tileConfigs[i] = PRIMARY;
			else
				tileConfigs[i] = SECONDARY;
		}
		// PROBLEM: if all WATER, it maps to all primary
		
		// Determine the TileType of all the 4 corners by calling TranslateWordCoordToType, passing in the corresponding world coordinate
		// World coordinate is calculated by subtracting the display layer coordinate with the set vector (definef in NEIGHBOURS)
		// TileType topLeft = WorldCoordToTileType(displayCoord - NEIGHBOURS[3]);
		// TileType topRight = WorldCoordToTileType(displayCoord - NEIGHBOURS[1]);
		// TileType bottomRight = WorldCoordToTileType(displayCoord); // No need to subtract with (0, 0)
		// TileType bottomLeft = WorldCoordToTileType(displayCoord - NEIGHBOURS[2]);

		return
		new(
			//tileCombinationSource[new(primaryTile, secondaryTile)],
			0,
			neighbourhoodRules[new(tileConfigs[0], tileConfigs[1], tileConfigs[2], tileConfigs[3])]
		);
	}

	private TileType WorldCoordToTileType(Vector2I worldCoord)
	{
		Vector2I worldAtlas = worldLayer.GetCellAtlasCoords(worldCoord);

		if (worldAtlas == GrassTileWorldAtlas)
			return GRASS;
		else if (worldAtlas == WaterTileWorldAtlas)
			return WATER;
		else
			return WATER;
	}
}
