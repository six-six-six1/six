using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


//��������������Ϣ
public class ActivationBlockData
{
    public List<Vector3Int> blockPoss = new List<Vector3Int>();//��������λ��
    public List<Vector3Int> protectPos = new List<Vector3Int>();//��������λ��
    public int hp = 3;//������������ֵ
}

public class BlockPillarSystem : MonoBehaviour
{
    public Tilemap baseMap;
    public TileBase blockPillarTile;
    public TileBase blockPillarHighlightTile;//����ʯ����
    public static BlockPillarSystem Instance;

    public List<Vector3Int> blockPosList = new List<Vector3Int>();//��������λ������
    Dictionary<Vector3Int, TileBase> activationPosList = new Dictionary<Vector3Int, TileBase>();//�����������λ������
    List<ActivationBlockData> allActivationBlockDataList = new List<ActivationBlockData>(); //���м����
    public bool isUseEnergyStone = false;
    private int useUseEnergyStoneCount = 0; //ʹ������ʯ����
    private ActivationBlockData currentActivationBlockData; //��ǰ�������Ϣ
    private int protectMaxCount = 4;

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
            if (!activationPosList.ContainsKey(blockPosList[i]))//ֻ�ܰ�δ����ĸ���
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
        // ����Ƿ�����Ч���赲��λ��
        if (baseMap.GetTile(pillarPosition) != blockPillarTile) return;

        // ʵ���赲�������߼�
        // ������Ҫ����赲�������߼�
    }

    public bool CheckForBlockLine(Vector3Int pillar1, Vector3Int pillar2)
    {
        // ��������赲��֮���Ƿ�����γ��赲��
        // ����true����ɹ��γ��赲��
        return true; // ��ʱ��
    }

    //�Ƿ�����������
    public bool IsIntercept(Vector3Int targetPos, Vector3Int direction)
    {
        for (int i = 0; i < allActivationBlockDataList.Count; i++)
        {
            ActivationBlockData activationBlockData = allActivationBlockDataList[i];
            for (int j = 0; j < activationBlockData.protectPos.Count; j++)
            {
                if (activationBlockData.protectPos[j] == targetPos) //���ڱ����ص�λ��
                {
                    Debug.Log($"������������λ��{activationBlockData.blockPoss[0]},{activationBlockData.blockPoss[1]},���ص�λ��{activationBlockData.protectPos}");
                    activationBlockData.hp--;
                    if (activationBlockData.hp == 0) //����ֵ�ľ����Ƴ�
                    {
                        Debug.Log($"���������ľ�,�Ƴ������������,λ��{activationBlockData.blockPoss[0]},{activationBlockData.blockPoss[1]}");
                        RemoveActivationBlockData(activationBlockData);
                    }
                    return true;
                }
            }
        
        }
        return false;
    }

    //�Ƴ��������������
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

                for (int i = 0; i < 6; i++) //��������
                {
                    Vector3Int newPos = blockPosition;
                    List<Vector3Int> protectPosList = new List<Vector3Int>();
                    for (int j = 0; j < protectMaxCount + 1; j++)
                    {
                        Vector3Int[] directions = HexGridSystem.Instance.GetNeighborDirectionsForPosition(newPos);
                        newPos = newPos + directions[i];
                        protectPosList.Add(newPos);
                        Vector3Int[] directions2 = HexGridSystem.Instance.GetNeighborDirectionsForPosition(newPos);
                        Vector3Int pos2 = newPos + directions2[i];
                        if (pos2 == currentActivationBlockData.blockPoss[0])
                        {
                            Debug.Log("�������������߱�׼" + pos2);
                            isActivation = true;
                            currentActivationBlockData.protectPos = protectPosList;
                            break;
                        }
                    }
                }
            }
            else
            {
                isActivation = true;
            }

            if (!isActivation)
            {
                Debug.Log("δ�ܼ���:"+ blockPosition);
                return;
            }
            Debug.Log("������������" + blockPosition);
            currentActivationBlockData.blockPoss.Add(blockPosition);
            activationPosList.Add(blockPosition,baseMap.GetTile(blockPosition));
            baseMap.SetTile(blockPosition, blockPillarTile);
            useUseEnergyStoneCount--;
            if (useUseEnergyStoneCount == 0)
            {
                isUseEnergyStone = false;
                //�ָ�ʣ�¸�����������
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
                for (int j = 0; j < activationBlockData.protectPos.Count; j++)
                {
                    Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(activationBlockData.protectPos[j]);
                    Vector3 pos = new Vector3(targetPos.x, targetPos.y);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
           

            }
        }

    }

}
