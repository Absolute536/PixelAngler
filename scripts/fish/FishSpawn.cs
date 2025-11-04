using GameWorld;
using Godot;
using System;

public partial class FishSpawn: Node
{
    public void InitialiseSpawnPosition(Vector2 centrePosition)
    {
        TileType tileType = GameInfo.Instance.GetTileType(centrePosition);
    }



    private bool IsPositionInWater(Vector2 position)
    {
        return GameInfo.Instance.GetTileType(position) == TileType.Water;
    }
}