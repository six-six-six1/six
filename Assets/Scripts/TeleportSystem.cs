using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TeleportSFX
{
    public AudioClip setupSound;      // ���ô�������Ч
    public AudioClip teleportSound;   // ���ͳɹ���Ч
    public GameObject teleportEffect; // ������ЧԤ����
    [Range(0, 1)] public float volume = 0.8f;
}

public class TeleportData
{
    public Vector3Int startTelportPos;    // ��ʼ���͵�λ��
    public Vector3Int targetTelportPos;   // Ŀ�괫�͵�λ��
}

// ������ϵͳ
public class TeleportSystem : MonoBehaviour
{
    [Header("��������")]
    public Tilemap baseMap;
    public TileBase teleporHexTile;
    public static TeleportSystem Instance;

    [Header("��Ч����Ч")]
    public TeleportSFX sfx;  // ��Ч��Ч����

    private AudioSource audioSource;
    private List<TeleportData> allTeleportDataList = new List<TeleportData>();
    private TeleportData currentTeleportData;
    private List<Vector3Int> normalPosList = new List<Vector3Int>();
    private Vector3Int exitPos;
    private System.Action onSelectTeleportAction;
    // ��TeleportSystem�������
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
    /// �������Ƿ�վ�ڴ�������
    /// </summary>
    public void CheckPlayerOnTeleport(Vector3Int playerGridPos)
    {
        foreach (var teleportData in allTeleportDataList)
        {
            if (teleportData.startTelportPos == playerGridPos)
            {
                Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(teleportData.targetTelportPos);
                PlayerController.Instance.SetPlayerPos(targetPos);
                PlayTeleportEffects(teleportData); // ���Ŵ���Ч��
                RemoveTeleport(teleportData);
                break;
            }
        }
    }

    /// <summary>
    /// ���Ŵ�����Ч����Ч
    /// </summary>
    private void PlayTeleportEffects(TeleportData teleportData)
    {
        // ���Ŵ�����Ч
        if (sfx.teleportSound != null)
        {
            audioSource.PlayOneShot(sfx.teleportSound, sfx.volume);
        }

        // ������Ŀ���������Ч
        if (sfx.teleportEffect != null)
        {
            Vector3 startWorldPos = baseMap.GetCellCenterWorld(teleportData.startTelportPos);
            Vector3 targetWorldPos = baseMap.GetCellCenterWorld(teleportData.targetTelportPos);

            Instantiate(sfx.teleportEffect, startWorldPos, Quaternion.identity);
            Instantiate(sfx.teleportEffect, targetWorldPos, Quaternion.identity);
        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    public void Trigger(Vector3Int position)
    {
        foreach (var teleportData in allTeleportDataList)
        {
            if (teleportData.startTelportPos == position)
            {
                Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(teleportData.targetTelportPos);
                PlayerController.Instance.SetPlayerPos(targetPos);
                PlayTeleportEffects(teleportData); // ���Ŵ���Ч��
                RemoveTeleport(teleportData);
                break;
            }
        }
    }

    /// <summary>
    /// ��λ�ø�������������
    /// </summary>
    public void CreateTeleport(Vector3Int position, System.Action complete)
    {
        // �������ô�������Ч
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

        // ��������³����
        int exitRandom = Random.Range(0, normalPosList.Count);
        HexGridSystem.Instance.SetExitTile(normalPosList[exitRandom], exitPos);


        // �����¼�֪ͨ
        OnExitPositionChanged?.Invoke(normalPosList[exitRandom]);
    }

    /// <summary>
    /// ɾ��������
    /// </summary>
    private void RemoveTeleport(TeleportData teleportData)
    {
        HexGridSystem.Instance.SetNormalTile(teleportData.startTelportPos);
        HexGridSystem.Instance.SetNormalTile(teleportData.targetTelportPos);
        allTeleportDataList.Remove(teleportData);
    }

    /// <summary>
    /// ������д�����
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