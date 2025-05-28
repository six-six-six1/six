using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


//激活能量柱的信息
public class ActivationBlockData
{
    public List<Vector3Int> blockPoss = new List<Vector3Int>();//能量柱的位置
    public Vector3Int protectPos;//被保护的位置
    public int hp = 3;//能量柱的生命值
}

public class BlockPillarSystem : MonoBehaviour
{
    public Tilemap baseMap;
    public TileBase blockPillarTile;
    public TileBase blockPillarHighlightTile;//能量石高亮
    public static BlockPillarSystem Instance;

    public List<Vector3Int> blockPosList = new List<Vector3Int>();//能量柱的位置数组
    Dictionary<Vector3Int, TileBase> activationPosList = new Dictionary<Vector3Int, TileBase>();//激活的能量柱位置数组
    List<ActivationBlockData> allActivationBlockDataList = new List<ActivationBlockData>(); //所有激活的
    public bool isUseEnergyStone = false;
    private int useUseEnergyStoneCount = 0; //使用能量石次数
    private ActivationBlockData currentActivationBlockData; //当前激活的信息

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Init()
    {

        GetBlockPosList();

        TilemapClickHandler.OnBlockClicked += OnOnBlockClick;
    }

    public void UseEnergyStone()
    {
        isUseEnergyStone = true;
        useUseEnergyStoneCount = 2;
        for (int i = 0; i < blockPosList.Count; i++)
        {
            if (!activationPosList.ContainsKey(blockPosList[i]))//只能把未激活的高亮
            {
                TileBase tileBase = baseMap.GetTile(blockPosList[i]);
                baseMap.SetTile(blockPosList[i], blockPillarHighlightTile);
            }
        }
        currentActivationBlockData = new ActivationBlockData();
        allActivationBlockDataList.Add(currentActivationBlockData);
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
        return true; // 暂时简化
    }

    //是否被能量柱阻拦
    public bool IsIntercept(Vector3Int targetPos, Vector3Int direction)
    {
        for (int i = 0; i < allActivationBlockDataList.Count; i++)
        {
            ActivationBlockData activationBlockData = allActivationBlockDataList[i];
            if (activationBlockData.protectPos == targetPos) //存在被拦截的位置
            {
                Debug.Log($"拦截能量柱的位置{activationBlockData.blockPoss[0]},{activationBlockData.blockPoss[1]},拦截的位置{activationBlockData.protectPos}");
                activationBlockData.hp--;
                if (activationBlockData.hp == 0) //生命值耗尽则移除
                {
                    Debug.Log($"拦截能量耗尽,移除激活的能量柱,位置{activationBlockData.blockPoss[0]},{activationBlockData.blockPoss[1]}");
                    RemoveActivationBlockData(activationBlockData);
                }

                return true;
            }
        }
        return false;
    }

    //移除掉激活的能量柱
    void RemoveActivationBlockData(ActivationBlockData activationBlockData)
    {
        activationPosList.Remove(activationBlockData.blockPoss[0]);
        activationPosList.Remove(activationBlockData.blockPoss[1]);
        allActivationBlockDataList.Remove(activationBlockData);
    }

    void GetBlockPosList()
    {
        foreach (var position in baseMap.cellBounds.allPositionsWithin)
        {
            if (baseMap.HasTile(position))
            {
                TileBase tile = baseMap.GetTile(position);
                //Debug.Log($"Tile at {position} is {tile.name}");
                if (tile.name == "BlockPillarHexTile")
                {
                    blockPosList.Add(position);
                }
            }
        }
    }

    void OnOnBlockClick(Vector3Int blockPosition)
    {
        if (!isUseEnergyStone) return;

        if (!activationPosList.ContainsKey(blockPosition))
        {
            bool isActivation = false;
            if (currentActivationBlockData.blockPoss.Count == 1)
            {
                Vector3Int[] directions = HexGridSystem.Instance.GetNeighborDirectionsForPosition(blockPosition);
                for (int i = 0; i < directions.Length; i++)
                {
                    Vector3Int pos1 = blockPosition + directions[i];
                    Vector3Int[] directions2 = HexGridSystem.Instance.GetNeighborDirectionsForPosition(pos1);
                    Vector3Int pos2 = pos1 + directions2[i];
                    if (pos2 == currentActivationBlockData.blockPoss[0])
                    {
                        Debug.Log("符合能量柱连线标准" + pos2);
                        isActivation = true;
                        currentActivationBlockData.protectPos = pos1;
                        break;
                    }
                }
            }
            else
            {
                isActivation = true;
            }

            if (!isActivation)
            {
                Debug.Log("未能激活:"+ blockPosition);
                return;
            }
            Debug.Log("激活能量柱：" + blockPosition);
            currentActivationBlockData.blockPoss.Add(blockPosition);
            activationPosList.Add(blockPosition,baseMap.GetTile(blockPosition));
            baseMap.SetTile(blockPosition, blockPillarTile);
            useUseEnergyStoneCount--;
            if (useUseEnergyStoneCount == 0)
            {
                isUseEnergyStone = false;
                //恢复剩下高亮的能量柱
                for (int i = 0; i < blockPosList.Count; i++)
                {
                    if (!activationPosList.ContainsKey(blockPosList[i]))
                    {
                        TileBase tileBase = baseMap.GetTile(blockPosList[i]);
                        baseMap.SetTile(blockPosList[i], blockPillarTile);
                    }
                }
            }
        }
    }



    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        for (int i = 0; i < allActivationBlockDataList.Count; i++)
        {
            if (allActivationBlockDataList[i].blockPoss.Count > 1)
            {
                ActivationBlockData activationBlockData = allActivationBlockDataList[i];
                Gizmos.color = Color.red;
                Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(activationBlockData.protectPos);
                Vector3 pos =  new Vector3(targetPos.x, targetPos.y);
                Gizmos.DrawCube(pos, Vector3.one);

            }
        }

    }

}
