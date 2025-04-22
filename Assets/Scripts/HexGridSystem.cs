using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexGridSystem : MonoBehaviour
{
    public Tilemap baseMap;
    public Tilemap obstacleMap;

    public TileBase normalTile;
    public TileBase darkTile;
    public TileBase cardReplenishTile;
    public TileBase blockPillarTile;
    public TileBase exitTile;
    // 在每个管理器脚本的顶部添加
    public static HexGridSystem Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public Vector3Int GetHexAtPosition(Vector3 worldPosition)
    {
        return baseMap.WorldToCell(worldPosition);
    }

    public Vector3 GetHexCenterPosition(Vector3Int hexCoords)
    {
        return baseMap.GetCellCenterWorld(hexCoords);
    }

    public bool IsHexWalkable(Vector3Int hexCoords)
    {
        // 检查是否是普通地块或特定功能地块
        TileBase tile = baseMap.GetTile(hexCoords);
        return tile == normalTile || tile == cardReplenishTile || tile == blockPillarTile;
    }
}
