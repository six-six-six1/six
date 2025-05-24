using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexGridSystem : MonoBehaviour
{
    public static HexGridSystem Instance;
    // ������Ҫ�ֶ�����
    [Header("Tilemap References")]
    public Tilemap tilemap; // ��Ҫ��ק�󶨵� Inspector

    private void Awake()
    {
        // ������ʼ��
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Multiple HexGridSystem instances detected.");
        }
        else
        {
            Instance = this;
        }
    }

    // �������ھӷ��� (ƽ��������)
    // HexGridSystem.cs
    private Vector3Int[] GetNeighborDirections(bool isOddRow)
    {
        // ƽ�������� YXZ ģʽ�µ��ھӷ���
        return isOddRow ?
            // �����з���
            new Vector3Int[]
            {
            new Vector3Int(1, 0, 0),   // �� (East)
            new Vector3Int(1, 1, 0),    // ���� (Northeast)
            new Vector3Int(0, 1, 0),    // ���� (Northwest)
            new Vector3Int(-1, 0, 0),   // �� (West)
            new Vector3Int(0, -1, 0),   // ���� (Southwest)
            new Vector3Int(1, -1, 0)    // ���� (Southeast)
            } :
            // ż���з���
            new Vector3Int[]
            {
            new Vector3Int(1, 0, 0),   // �� (East)
            new Vector3Int(0, 1, 0),    // ���� (Northeast)
            new Vector3Int(-1, 1, 0),   // ���� (Northwest)
            new Vector3Int(-1, 0, 0),   // �� (West)
            new Vector3Int(-1, -1, 0),  // ���� (Southwest)
            new Vector3Int(0, -1, 0)     // ���� (Southeast)
            };
    }

    // ����ת������
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return tilemap.WorldToCell(worldPosition);
    }

    public Vector3 GetHexCenterPosition(Vector3Int cellPosition)
    {
        // ȷ�� Tilemap ����ȷ����Ϊ�����β���
        return tilemap.GetCellCenterWorld(cellPosition);
    }
    // ��Ч����֤����
    public bool IsHexValid(Vector3Int position)
    {
        return tilemap.GetTile(position) != null;
    }

    [Header("Highlight")]
    public TileBase highlightTile;
    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();

    // HexGridSystem.cs �޸ĸ����߼�
    public void HighlightHex(Vector3Int position, bool highlight)
    {
        if (!IsHexWalkable(position)) return; // �����ж�

        if (highlight)
        {
            originalTiles[position] = tilemap.GetTile(position);
            tilemap.SetTile(position, highlightTile);
            tilemap.SetColor(position, Color.yellow); // ������ɫ�����滻Tile
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
        }
        originalTiles.Clear();
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

    // ��Scene��ͼ�п��ӻ��ھӷ���
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

    // ��������������֤
    public bool IsHexWalkable(Vector3Int hexCoords)
    {
        // ������Ҫ���������Ϸ����ʵ��
        // ʾ��������ϰ���ͺ���
        return tilemap.GetTile(hexCoords) != null
            && tilemap.GetTile(hexCoords).name != "BlockPillarHexTile"
            && tilemap.GetTile(hexCoords).name != "DarkHexTile";
    }
}