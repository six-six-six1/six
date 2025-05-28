using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleportData
{
    public Vector3Int startTelportPos;//开始传送点位置
    public Vector3Int targetTelportPos;//目标传送点位置

}

//传送门系统
public class TeleportSystem : MonoBehaviour
{
    public Tilemap baseMap;
    public TileBase teleporHexTile;
    public static TeleportSystem Instance;

    private List<TeleportData> allTeleportDataList = new List<TeleportData>(); //所哟传送门的信息
    private TeleportData currentTeleportData;//当前传送门数据
    private List<Vector3Int> normalPosList = new List<Vector3Int>();//正常格子数组
    private Vector3Int exitPos;//撤离点的位置

    private System.Action onSelectTeleportAction;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //触发传送门
    public void Trigger(Vector3Int position)
    {
        for (int i = 0; i < allTeleportDataList.Count; i++)
        {
            TeleportData teleportData = allTeleportDataList[i];
            if (teleportData.startTelportPos == position) //判断传送门跟移动的位置是否一样
            {
                Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(teleportData.targetTelportPos);
                PlayerController.Instance.SetPlayerPos(targetPos);
                RemoveTeleport(teleportData);
            }
        }
    }

    //在位置附近创建传送门
    public void CreateTeleport(Vector3Int position,System.Action complete)
    {
        onSelectTeleportAction = complete;
        currentTeleportData = new TeleportData();
        allTeleportDataList.Add(currentTeleportData);

        // 调用新方法获取方向数组
        Vector3Int[] directions = HexGridSystem.Instance.GetNeighborDirectionsForPosition(position);

        List<Vector3Int> createPosList = new List<Vector3Int>();

        // 高亮所有可能方向的首个格子
        foreach (var dir in directions)
        {
            Vector3Int neighborPos = position + dir;
            // 仅检查格子是否存在Tile，不限制是否为DarkHexTile
            if (HexGridSystem.Instance.IsNormalHexTile(neighborPos))
            {
                createPosList.Add(neighborPos);
            }
        }

        int index = 0;
        if (createPosList.Count > 1)
        {
            index = Random.Range(0, createPosList.Count);
        }

        currentTeleportData.startTelportPos = createPosList[index];
        Debug.Log($"传送门起点：{currentTeleportData.startTelportPos}");
        baseMap.SetTile(currentTeleportData.startTelportPos, teleporHexTile);

        normalPosList.Clear();
        //获取所有正常的方块变成可防止传送门的高亮
        foreach (var pos in baseMap.cellBounds.allPositionsWithin)
        {
            if (HexGridSystem.Instance.IsNormalHexTile(pos) && pos != position)//不能等于当前的位置
            {
                HexGridSystem.Instance.HighlightHex(pos, true);
                normalPosList.Add(pos);
            } else if (HexGridSystem.Instance.IsExitHexTile(pos))//获取当前撤离点的位置
            {
                exitPos = pos;
            }
        }
        TilemapClickHandler.OnHexClicked += OnSelectTelportClick;
    }

    void OnSelectTelportClick(Vector3Int position)
    {
        TilemapClickHandler.OnHexClicked -= OnSelectTelportClick;
        HexGridSystem.Instance.ClearAllHighlights();
        baseMap.SetTile(position, teleporHexTile);
        currentTeleportData.targetTelportPos = position;
        Debug.Log($"传送门终点：{currentTeleportData.targetTelportPos}");
        onSelectTeleportAction?.Invoke();

        int exitRandom = Random.Range(0, normalPosList.Count);
        Vector3Int newExitPos = normalPosList[exitRandom]; //随机一个撤离点的位置
        HexGridSystem.Instance.SetExitTile(newExitPos,exitPos);

    }

    //删除传送门
    void RemoveTeleport(TeleportData teleportData)
    {
        HexGridSystem.Instance.SetNormalTile(teleportData.startTelportPos);
        HexGridSystem.Instance.SetNormalTile(teleportData.targetTelportPos);
        allTeleportDataList.Remove(teleportData);
    }
}
