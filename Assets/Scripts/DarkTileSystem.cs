using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DarkTileSystem : MonoBehaviour
{
    [Header("Tilemap References")]
    public Tilemap tilemap;
    public Tilemap baseMap;
    public TileBase darkTile;
    public TileBase normalTile;
    public static DarkTileSystem Instance;

    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();

    private void Awake()
    {
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ExpandDarkTiles(int expandCount)
    {
        List<Vector3Int> newDarkTiles = new List<Vector3Int>();

        // 找出所有现有黑暗地块
        foreach (var pos in baseMap.cellBounds.allPositionsWithin)
        {
            if (baseMap.GetTile(pos) == darkTile)
            {
                // 检查六个相邻方向
                for (int i = 0; i < 6; i++)
                {
                    Vector3Int neighbor = GetHexNeighbor(pos, i);
                    if (baseMap.GetTile(neighbor) == normalTile &&
                        !newDarkTiles.Contains(neighbor) &&
                        newDarkTiles.Count < expandCount)
                    {
                        newDarkTiles.Add(neighbor);
                    }
                }
            }
        }

        // 应用新的黑暗地块
        foreach (var tilePos in newDarkTiles)
        {
            baseMap.SetTile(tilePos, darkTile);
        }

        CheckPlayerCaught();
    }

    private Vector3Int GetHexNeighbor(Vector3Int position, int direction)
    {
        // 六边形网格的六个方向
        Vector3Int[] hexDirections = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),   // 右
            new Vector3Int(1, -1, 0),  // 右上
            new Vector3Int(0, -1, 0),  // 左上
            new Vector3Int(-1, 0, 0),  // 左
            new Vector3Int(-1, 1, 0),  // 左下
            new Vector3Int(0, 1, 0)    // 右下
        };

        return position + hexDirections[direction];
    }

    private void CheckPlayerCaught()
    {
        Vector3Int playerHex = baseMap.WorldToCell(PlayerController.Instance.transform.position);
        if (baseMap.GetTile(playerHex) == darkTile)
        {
            GameManager.Instance.GameOver(false);
        }
    }

    public void ClearDarkTile(Vector3Int position)
    {
        // 验证 Tilemap 和 darkTile 是否已赋值
        if (tilemap == null || darkTile == null)
        {
            Debug.LogError("Tilemap 或 darkTile 未配置！");
            return;
        }

        // 检查目标位置是否是 DarkHexTile
        if (baseMap.GetTile(position) == darkTile)
        {
            // 恢复原始 Tile（如果已存储）
            if (originalTiles.TryGetValue(position, out TileBase original))
            {
                baseMap.SetTile(position, original);
                originalTiles.Remove(position);
                Debug.Log($"已清除DarkHexTile：{position}，恢复为 {original.name}");
            }
            else
            {
                // 默认恢复为预设的normalTile（白色地块）
                baseMap.SetTile(position, normalTile);
                Debug.LogWarning($"位置 {position} 无原始记录，恢复为默认Tile");
            }
            // 强制刷新显示
            baseMap.RefreshTile(position);
        }
    }
}
