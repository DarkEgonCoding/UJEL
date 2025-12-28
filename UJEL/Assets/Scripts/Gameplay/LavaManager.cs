using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LavaManager : MonoBehaviour
{
    public Tilemap lavaMap;
    public TileBase filledTile; // ground/rock tile to replace lava

    private List<Vector3Int> filledPositions;

    private void Start()
    {
        filledPositions = new List<Vector3Int>();
    }

    public bool IsLava(Vector3Int gridPos)
    {
        bool isOnTilemap = lavaMap.HasTile(gridPos);
        if (!isOnTilemap) return false; // If not on tilemap
        if (IsFilledAtPos(gridPos)) return false; // If the tile is filled by rock
        return true;
    }

    public bool IsFilledAtPos(Vector3Int gridPos)
    {
        foreach (Vector3Int pos in filledPositions)
        {
            if (pos == gridPos)
            {
                return true;
            }
        }
        return false;
    }

    public void FillLava(Vector3Int gridPos)
    {
        if (lavaMap.HasTile(gridPos))
        {
            lavaMap.SetTile(gridPos, filledTile);
            filledPositions.Add(gridPos);
        }
    }
}
