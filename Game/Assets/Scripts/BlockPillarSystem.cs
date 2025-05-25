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
        // ����Ƿ�����Ч���赲��λ��
        if (baseMap.GetTile(pillarPosition) != blockPillarTile) return;

        // ʵ���赲�������߼�
        // ������Ҫ����赲�������߼�
    }

    public bool CheckForBlockLine(Vector3Int pillar1, Vector3Int pillar2)
    {
        // ��������赲��֮���Ƿ�����γ��赲��
        // ����true����ɹ��γ��赲��
        return false; // ��ʱ��
    }
}
