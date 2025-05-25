using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DarkTileSystem : MonoBehaviour
{
    public Tilemap baseMap;
    public TileBase darkTile;
    public TileBase normalTile;
    public static DarkTileSystem Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void ExpandDarkTiles(int expandCount)
    {
        List<Vector3Int> newDarkTiles = new List<Vector3Int>();

        // �ҳ��������кڰ��ؿ�
        foreach (var pos in baseMap.cellBounds.allPositionsWithin)
        {
            if (baseMap.GetTile(pos) == darkTile)
            {
                // ����������ڷ���
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

        // Ӧ���µĺڰ��ؿ�
        foreach (var tilePos in newDarkTiles)
        {
            baseMap.SetTile(tilePos, darkTile);
        }

        CheckPlayerCaught();
    }

    private Vector3Int GetHexNeighbor(Vector3Int position, int direction)
    {
        // �������������������
        Vector3Int[] hexDirections = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0),   // ��
            new Vector3Int(1, -1, 0),  // ����
            new Vector3Int(0, -1, 0),  // ����
            new Vector3Int(-1, 0, 0),  // ��
            new Vector3Int(-1, 1, 0),  // ����
            new Vector3Int(0, 1, 0)    // ����
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
}
