using Godot;
using System;
using System.Collections.Generic;

public partial class World : Node2D
{
	[Export] TileMapLayer displayLayer;
	[Export] TileMapLayer worldLayer;

	private Vector2I dirtPlaceHolder = new(0, 0);
	private Vector2I grassPlaceholder = new(1, 0);

	// Array of the 4 neighbourhood vectors 
	readonly Vector2I[] NEIGHBOURS = [new(0, 0), new(0, 1), new(1, 0), new(1, 1)];

	private readonly Vector2I TOPRIGHTVECTOR = new(0, 1);

	// Dictionary of 4-neighbour TileType -> Atlas coordinate of the tile to be selected
	readonly Dictionary<Tuple<TileType, TileType, TileType, TileType>, Vector2I> neighbourhoodRules = new() {
		{new (TileType.GRASS, TileType.GRASS, TileType.GRASS, TileType.GRASS), new (1, 1)}, // Full Grass
		{new (TileType.GRASS, TileType.DIRT, TileType.DIRT, TileType.DIRT), new (2, 2)}, // Top Left Corner
		{new (TileType.DIRT, TileType.GRASS, TileType.DIRT, TileType.DIRT), new (0, 2)}, // Top Right Corner
		{new (TileType.DIRT, TileType.DIRT, TileType.GRASS, TileType.DIRT), new (0, 0)}, // Bottom Right Corner
		{new (TileType.DIRT, TileType.DIRT, TileType.DIRT, TileType.GRASS), new (2, 0)}, // Bottom Left Corner
		{new (TileType.GRASS, TileType.GRASS, TileType.DIRT, TileType.DIRT), new (1, 2)}, // Top Side
		{new (TileType.DIRT, TileType.GRASS, TileType.GRASS, TileType.DIRT), new (0, 1)}, // Right Side
		{new (TileType.GRASS, TileType.DIRT, TileType.DIRT, TileType.GRASS), new (2, 1)}, // Left Side
		{new (TileType.DIRT, TileType.DIRT, TileType.GRASS, TileType.GRASS), new (1, 0)}, // Bottom Side
		{new (TileType.GRASS, TileType.GRASS, TileType.DIRT, TileType.GRASS), new (1, 4)}, // Empty Bottom Right Corner
		{new (TileType.GRASS, TileType.GRASS, TileType.GRASS, TileType.DIRT), new (0, 4)}, // Empty Bottom Left Corner
		{new (TileType.GRASS, TileType.DIRT, TileType.GRASS, TileType.GRASS), new (1, 3)}, // Empty Top Right Corner
		{new (TileType.DIRT, TileType.GRASS, TileType.GRASS, TileType.GRASS), new (0, 3)}, // Empty Top Left Corner
		{new (TileType.GRASS, TileType.DIRT, TileType.GRASS, TileType.DIRT), new (0, 5)}, // Left Diagonal Corners
		{new (TileType.DIRT, TileType.GRASS, TileType.DIRT, TileType.GRASS), new (0, 6)}, // Right Diagonal Corners
		{new (TileType.DIRT, TileType.DIRT, TileType.DIRT, TileType.DIRT), new (8, 3)} // Full Dirt
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		if (!Engine.IsEditorHint())
			foreach (Vector2I worldCoord in worldLayer.GetUsedCells())
			{
				ShowDisplayTile(worldCoord);
			}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// 	foreach (Vector2I worldCoord in worldLayer.GetUsedCells())
	// 	{
	// 		ShowDisplayTile(worldCoord);
	// 	}
	// }

	private void ShowDisplayTile(Vector2I coord)
	{
		Random rand = new Random();
		// Iterate through the 4 neighbours and display the 4 corners
		for (int i = 0; i < NEIGHBOURS.Length; i++)
		{
			Vector2I displayCoord = coord + NEIGHBOURS[i];

			Vector2I displayInfo = CalculateDisplayTile(displayCoord);
			// baseDisplayLayer.SetCell(displayCoord, 0, new Vector2I(rand.Next(5), 0)); // Display the base (Dirt) tile layer (choose randomly)
			displayLayer.SetCell(displayCoord, 0, displayInfo);

		}
	}

	private Vector2I CalculateDisplayTile(Vector2I displayCoord)
	{
		// Determine the TileType of all the 4 corners by calling TranslateWordCoordToType, passing in the corresponding world coordinate
		// World coordinate is calculated by subtracting the display layer coordinate with the set vector (define in NEIGHBOURS)
		TileType topLeft = TranslateWordCoordToType(displayCoord - NEIGHBOURS[3]);
		TileType topRight = TranslateWordCoordToType(displayCoord - NEIGHBOURS[1]);
		TileType bottomLeft = TranslateWordCoordToType(displayCoord - NEIGHBOURS[2]);
		TileType bottomRight = TranslateWordCoordToType(displayCoord); // No need to subtract with (0, 0)

		return neighbourhoodRules[new(topLeft, topRight, bottomRight, bottomLeft)];
	}

	private TileType TranslateWordCoordToType(Vector2I coord)
	{
		Vector2I worldAtlas = worldLayer.GetCellAtlasCoords(coord);
		if (worldAtlas == grassPlaceholder)
			return TileType.GRASS;

		return TileType.DIRT;
	}

}

public enum TileType {
	DIRT,
	GRASS
}
