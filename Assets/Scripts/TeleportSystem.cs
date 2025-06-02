using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TeleportSFX
{
    public AudioClip setupSound;      // 设置传送门音效
    public AudioClip teleportSound;   // 传送成功音效
    public GameObject teleportEffect; // 传送特效预制体
    [Range(0, 1)] public float volume = 0.8f;
}

public class TeleportData
{
    public Vector3Int startTelportPos;    // 开始传送点位置
    public Vector3Int targetTelportPos;   // 目标传送点位置
}

// 传送门系统
public class TeleportSystem : MonoBehaviour
{
    [Header("基础设置")]
    public Tilemap baseMap;
    public TileBase teleporHexTile;
    public static TeleportSystem Instance;

    [Header("音效与特效")]
    public TeleportSFX sfx;  // 音效特效配置

    private AudioSource audioSource;
    private List<TeleportData> allTeleportDataList = new List<TeleportData>();
    private TeleportData currentTeleportData;
    private List<Vector3Int> normalPosList = new List<Vector3Int>();
    private Vector3Int exitPos;
    private System.Action onSelectTeleportAction;
    // 在TeleportSystem类中添加
    public static event System.Action<Vector3Int> OnExitPositionChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 检查玩家是否站在传送门上
    /// </summary>
    public void CheckPlayerOnTeleport(Vector3Int playerGridPos)
    {
        foreach (var teleportData in allTeleportDataList)
        {
            if (teleportData.startTelportPos == playerGridPos)
            {
                Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(teleportData.targetTelportPos);
                PlayerController.Instance.SetPlayerPos(targetPos);
                PlayTeleportEffects(teleportData); // 播放传送效果
                RemoveTeleport(teleportData);
                break;
            }
        }
    }

    /// <summary>
    /// 播放传送音效和特效
    /// </summary>
    private void PlayTeleportEffects(TeleportData teleportData)
    {
        // 播放传送音效
        if (sfx.teleportSound != null)
        {
            audioSource.PlayOneShot(sfx.teleportSound, sfx.volume);
        }

        // 在起点和目标点生成特效
        if (sfx.teleportEffect != null)
        {
            Vector3 startWorldPos = baseMap.GetCellCenterWorld(teleportData.startTelportPos);
            Vector3 targetWorldPos = baseMap.GetCellCenterWorld(teleportData.targetTelportPos);

            Instantiate(sfx.teleportEffect, startWorldPos, Quaternion.identity);
            Instantiate(sfx.teleportEffect, targetWorldPos, Quaternion.identity);
        }
    }

    /// <summary>
    /// 触发传送门
    /// </summary>
    public void Trigger(Vector3Int position)
    {
        foreach (var teleportData in allTeleportDataList)
        {
            if (teleportData.startTelportPos == position)
            {
                Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(teleportData.targetTelportPos);
                PlayerController.Instance.SetPlayerPos(targetPos);
                PlayTeleportEffects(teleportData); // 播放传送效果
                RemoveTeleport(teleportData);
                break;
            }
        }
    }

    /// <summary>
    /// 在位置附近创建传送门
    /// </summary>
    public void CreateTeleport(Vector3Int position, System.Action complete)
    {
        // 播放设置传送门音效
        if (sfx.setupSound != null)
        {
            audioSource.PlayOneShot(sfx.setupSound, sfx.volume);
        }

        onSelectTeleportAction = complete;
        currentTeleportData = new TeleportData();
        allTeleportDataList.Add(currentTeleportData);

        Vector3Int[] directions = HexGridSystem.Instance.GetNeighborDirectionsForPosition(position);
        List<Vector3Int> createPosList = new List<Vector3Int>();

        foreach (var dir in directions)
        {
            Vector3Int neighborPos = position + dir;
            if (HexGridSystem.Instance.IsNormalHexTile(neighborPos))
            {
                createPosList.Add(neighborPos);
            }
        }

        int index = createPosList.Count > 1 ? Random.Range(0, createPosList.Count) : 0;
        currentTeleportData.startTelportPos = createPosList[index];
        baseMap.SetTile(currentTeleportData.startTelportPos, teleporHexTile);

        normalPosList.Clear();
        foreach (var pos in baseMap.cellBounds.allPositionsWithin)
        {
            if (HexGridSystem.Instance.IsNormalHexTile(pos) && pos != position)
            {
                HexGridSystem.Instance.HighlightHex(pos, true);
                normalPosList.Add(pos);
            }
            else if (HexGridSystem.Instance.IsExitHexTile(pos))
            {
                exitPos = pos;
            }
        }
        TilemapClickHandler.OnHexClicked += OnSelectTelportClick;
    }

    private void OnSelectTelportClick(Vector3Int position)
    {
        TilemapClickHandler.OnHexClicked -= OnSelectTelportClick;
        HexGridSystem.Instance.ClearAllHighlights();
        baseMap.SetTile(position, teleporHexTile);
        currentTeleportData.targetTelportPos = position;
        onSelectTeleportAction?.Invoke();

        // 随机设置新撤离点
        int exitRandom = Random.Range(0, normalPosList.Count);
        HexGridSystem.Instance.SetExitTile(normalPosList[exitRandom], exitPos);


        // 触发事件通知
        OnExitPositionChanged?.Invoke(normalPosList[exitRandom]);
    }

    /// <summary>
    /// 删除传送门
    /// </summary>
    private void RemoveTeleport(TeleportData teleportData)
    {
        HexGridSystem.Instance.SetNormalTile(teleportData.startTelportPos);
        HexGridSystem.Instance.SetNormalTile(teleportData.targetTelportPos);
        allTeleportDataList.Remove(teleportData);
    }

    /// <summary>
    /// 清空所有传送门
    /// </summary>
    public void ClearTeleport()
    {
        TilemapClickHandler.OnHexClicked -= OnSelectTelportClick;
        while (allTeleportDataList.Count > 0)
        {
            RemoveTeleport(allTeleportDataList[0]);
        }
    }
}