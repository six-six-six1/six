using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleportData
{
    public Vector3Int startTelportPos;//��ʼ���͵�λ��
    public Vector3Int targetTelportPos;//Ŀ�괫�͵�λ��

}

//������ϵͳ
public class TeleportSystem : MonoBehaviour
{
    public Tilemap baseMap;
    public TileBase teleporHexTile;
    public static TeleportSystem Instance;

    private List<TeleportData> allTeleportDataList = new List<TeleportData>(); //��Ӵ�����ŵ���Ϣ
    private TeleportData currentTeleportData;//��ǰ����������
    private List<Vector3Int> normalPosList = new List<Vector3Int>();//������������
    private Vector3Int exitPos;//������λ��

    private System.Action onSelectTeleportAction;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //����������
    public void Trigger(Vector3Int position)
    {
        for (int i = 0; i < allTeleportDataList.Count; i++)
        {
            TeleportData teleportData = allTeleportDataList[i];
            if (teleportData.startTelportPos == position) //�жϴ����Ÿ��ƶ���λ���Ƿ�һ��
            {
                Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(teleportData.targetTelportPos);
                PlayerController.Instance.SetPlayerPos(targetPos);
                RemoveTeleport(teleportData);
            }
        }
    }

    //��λ�ø�������������
    public void CreateTeleport(Vector3Int position,System.Action complete)
    {
        onSelectTeleportAction = complete;
        currentTeleportData = new TeleportData();
        allTeleportDataList.Add(currentTeleportData);

        // �����·�����ȡ��������
        Vector3Int[] directions = HexGridSystem.Instance.GetNeighborDirectionsForPosition(position);

        List<Vector3Int> createPosList = new List<Vector3Int>();

        // �������п��ܷ�����׸�����
        foreach (var dir in directions)
        {
            Vector3Int neighborPos = position + dir;
            // ���������Ƿ����Tile���������Ƿ�ΪDarkHexTile
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
        Debug.Log($"��������㣺{currentTeleportData.startTelportPos}");
        baseMap.SetTile(currentTeleportData.startTelportPos, teleporHexTile);

        normalPosList.Clear();
        //��ȡ���������ķ����ɿɷ�ֹ�����ŵĸ���
        foreach (var pos in baseMap.cellBounds.allPositionsWithin)
        {
            if (HexGridSystem.Instance.IsNormalHexTile(pos) && pos != position)//���ܵ��ڵ�ǰ��λ��
            {
                HexGridSystem.Instance.HighlightHex(pos, true);
                normalPosList.Add(pos);
            } else if (HexGridSystem.Instance.IsExitHexTile(pos))//��ȡ��ǰ������λ��
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
        Debug.Log($"�������յ㣺{currentTeleportData.targetTelportPos}");
        onSelectTeleportAction?.Invoke();

        int exitRandom = Random.Range(0, normalPosList.Count);
        Vector3Int newExitPos = normalPosList[exitRandom]; //���һ��������λ��
        HexGridSystem.Instance.SetExitTile(newExitPos,exitPos);

    }

    //ɾ��������
    void RemoveTeleport(TeleportData teleportData)
    {
        HexGridSystem.Instance.SetNormalTile(teleportData.startTelportPos);
        HexGridSystem.Instance.SetNormalTile(teleportData.targetTelportPos);
        allTeleportDataList.Remove(teleportData);
    }
}
