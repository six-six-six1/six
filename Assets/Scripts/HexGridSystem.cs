using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexGridSystem : MonoBehaviour
{
    public static HexGridSystem Instance;

    // 新增必要字段声明
    [Header("Tilemap References")] public Tilemap tilemap; // 需要拖拽绑定到 Inspector

    public TileBase normalHexTile;
    public TileBase exitHexTile;

    private void Awake()
    {
        // 单例初始化
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Debug.LogWarning("Multiple HexGridSystem instances detected.");
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject); // 跨场景保留对象（如需）

        // 确保tilemap引用不为空
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
            if (tilemap == null)
            {
                Debug.LogError("Tilemap未绑定且当前GameObject无Tilemap组件！");
            }
        }

        Debug.Log($"HexGridSystem初始化完成，挂载对象：{gameObject.name}");
    }

    // 六边形邻居方向 (平顶六边形)
    // HexGridSystem.cs
    private Vector3Int[] GetNeighborDirections(bool isOddRow)
    {
        // 平顶六边形 YXZ 模式下的邻居方向
        return isOddRow
            ?
            // 奇数行方向
            new Vector3Int[]
            {
                new Vector3Int(1, 0, 0),   // 上
                new Vector3Int(1, -1, 0),  // 左上
                new Vector3Int(1, 1, 0),  // 右上
                new Vector3Int(-1, 0, 0),  // 下
                new Vector3Int(0, -1, 0),  // 左下
                new Vector3Int(0, 1, 0)    // 右下
            }
            :
            // 偶数行方向
            new Vector3Int[]
            {
                new Vector3Int(1, 0, 0),   // 上
                new Vector3Int(0, -1, 0),  // 左上
                new Vector3Int(0, 1, 0),  // 右上
                new Vector3Int(-1, 0, 0),  // 下
                new Vector3Int(-1, -1, 0),  // 左下
                new Vector3Int(-1, 1, 0)    // 右下
            };
    }

    public Vector3Int[] GetNeighborDirectionsForPosition(Vector3Int position)
    {
        // 判断当前行是否为奇数行
        bool isOddRow = (position.y % 2) != 0;
        // 调用私有方法获取方向
        return GetNeighborDirections(isOddRow);
    }

    // 坐标转换方法
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap引用丢失！");
            return Vector3Int.zero;
        }
        return tilemap.WorldToCell(worldPosition);
    }

    public Vector3 GetHexCenterPosition(Vector3Int cellPosition)
    {
        // 确保 Tilemap 已正确配置为六边形布局
        return tilemap.GetCellCenterWorld(cellPosition);
    }

    // 有效性验证方法
    public bool IsHexValid(Vector3Int position)
    {
        return tilemap.GetTile(position) != null;
    }

    [Header("Highlight")] public TileBase highlightTile;

    public List<TileBase> highlightTileList = new();
    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();

    public void HighlightHex(Vector3Int position, bool highlight)
    {
        if (!IsHexValid(position)) return; // 新增判断

        Debug.Log("高亮");
        if (highlight)
        {
            originalTiles[position] = tilemap.GetTile(position);
            tilemap.SetTile(position, highlightTile);
            tilemap.SetColor(position, Color.yellow); // 叠加颜色而非替换Tile
            highlightTileList.Add(tilemap.GetTile(position));

        }
        else
        {
            tilemap.SetTile(position, originalTiles[position]);
            tilemap.SetColor(position, Color.white);
        }
    }

    public void ClearAllHighlights()
    {
        foreach (var pos in originalTiles.Keys)
        {
            tilemap.SetTile(pos, originalTiles[pos]);
            tilemap.SetColor(pos, Color.white); // 强制重置颜色
        }

        originalTiles.Clear();
        highlightTileList.Clear();
    }

    public List<Vector3Int> GetWalkableNeighbors(Vector3Int center)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        bool isOddRow = (center.y % 2) != 0;

        foreach (var dir in GetNeighborDirections(isOddRow))
        {
            Vector3Int neighborPos = center + dir;
            if (IsHexValid(neighborPos) && IsHexWalkable(neighborPos))
            {
                neighbors.Add(neighborPos);
            }
        }

        return neighbors;
    }

    public List<Vector3Int> GetTilesInDirection(Vector3Int startPos, Vector3Int direction, int maxDistance)
    {
        List<Vector3Int> tiles = new List<Vector3Int>();
        Vector3Int currentPos = startPos;
        int directionIndex = ConvertDirectionToIndex(startPos, direction);


        for (int i = 0; i < maxDistance; i++)
        {
            // 判断当前行的奇偶性，假设按 currentPos.y 来判断
            bool isOddRow = (currentPos.y % 2 != 0);
            Vector3Int[] neighborDirs = GetNeighborDirections(isOddRow);
            Vector3Int correctDir = neighborDirs[directionIndex];
            currentPos += correctDir;
            if (!IsHexValid(currentPos))
                break;
            tiles.Add(currentPos);
        }

        return tiles;
    }

    public int ConvertDirectionToIndex(Vector3Int startPos, Vector3Int direction)
    {
        // 根据起始行的奇偶判断
        bool isOddRow = (startPos.y % 2 != 0);
        Vector3Int[] neighborDirs = GetNeighborDirections(isOddRow);

        // 遍历邻居方向数组，如果匹配则返回索引
        for (int i = 0; i < neighborDirs.Length; i++)
        {
            if (neighborDirs[i] == direction)
                return i;
        }
        // 如果没有匹配上，可以返回-1或者抛出异常
        return -1;
    }

    // 在Scene视图中可视化邻居方向
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector3Int center = WorldToCell(transform.position);
        bool isOddRow = (center.y % 2) != 0;
        var directions = GetNeighborDirections(isOddRow);

        Gizmos.color = Color.cyan;
        foreach (var dir in directions)
        {
            Vector3Int neighborPos = center + dir;
            Vector3 worldPos = GetHexCenterPosition(neighborPos);
            Gizmos.DrawLine(GetHexCenterPosition(center), worldPos);
            Gizmos.DrawSphere(worldPos, 0.1f);
        }
    }

    //设置成正常的方块
    public void SetNormalTile(Vector3Int position)
    {
        tilemap.SetTile(position,normalHexTile);
    }

    //设置撤离点
    public void SetExitTile(Vector3Int newPosition,Vector3Int position)
    {
        tilemap.SetTile(newPosition, exitHexTile);
        tilemap.SetTile(position,normalHexTile);
    }

    // 新增可行走性验证
    public bool IsHexWalkable(Vector3Int hexCoords)
    {
        TileBase tile = tilemap.GetTile(hexCoords);
        return tile != null
            && tile.name != "BlockPillarHexTile"
            && tile.name != "DarkHexTile"; // DarkHexTile不可行走，但可高亮
    }

    public bool IsDarkHexTile(Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);
        // 检查 Tile 名称是否为 "DarkHexTile"
        return tile != null && tile.name == "DarkHexTile";
    }

    //判断是否为出口
    public bool IsExitHexTile(Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);
        return tile != null && tile.name == "ExitHexTile";
    }

    //判断是否为补卡点
    public bool IsCardHexTile(Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);
        return tile != null && tile.name == "CardReplenishHexTile";
    }

    //判断是否为正常格子（无任何功能性的格子）
    public bool IsNormalHexTile(Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);
        return tile != null && tile.name == "NormalHexTile";
    }

    //判断是否为传送门
    public bool IsTeleportHexTile(Vector3Int position)
    {
        TileBase tile = tilemap.GetTile(position);
        return tile != null && tile.name == "TeleporHexTile";
    }
}