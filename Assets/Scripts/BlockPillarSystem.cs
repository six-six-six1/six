using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockPillarSystem : MonoBehaviour
{
    public Tilemap baseMap;
    public TileBase blockPillarTile;
    public static BlockPillarSystem Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void ActivatePillar(Vector3Int pillarPosition)
    {
        // 检查是否是有效的阻挡柱位置
        if (baseMap.GetTile(pillarPosition) != blockPillarTile) return;

        // 实现阻挡柱激活逻辑
        // 这里需要添加阻挡线生成逻辑
    }

    public bool CheckForBlockLine(Vector3Int pillar1, Vector3Int pillar2)
    {
        // 检查两个阻挡柱之间是否可以形成阻挡线
        // 返回true如果成功形成阻挡线
        return false; // 暂时简化
    }
}
