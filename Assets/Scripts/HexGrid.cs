using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexGrid : MonoBehaviour
{
    public Tilemap tilemap;

    // 六边形邻居方向 (平顶六边形)
    private static readonly Vector3Int[] neighborDirections =
    {
        new Vector3Int(1, 0, 0),    // 右
        new Vector3Int(0, 1, 0),    // 右上
        new Vector3Int(-1, 1, 0),    // 左上
        new Vector3Int(-1, 0, 0),    // 左
        new Vector3Int(0, -1, 0),    // 左下
        new Vector3Int(1, -1, 0)     // 右下
    };

    public List<Vector3Int> GetNeighbors(Vector3Int position)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        foreach (var dir in neighborDirections)
        {
            Vector3Int neighborPos = position + dir;
            if (tilemap.GetTile(neighborPos) != null)
            {
                neighbors.Add(neighborPos);
            }
        }

        return neighbors;
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }

    public Vector3 CellToWorld(Vector3Int cellPosition)
    {
        return tilemap.CellToWorld(cellPosition);
    }
}
