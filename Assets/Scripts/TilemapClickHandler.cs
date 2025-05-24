// TilemapClickHandler.cs �½��ű�
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TilemapClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event System.Action<Vector3Int> OnHexClicked;

    private Tilemap tilemap;
    private Camera mainCamera;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        mainCamera = Camera.main;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);

        if (tilemap.HasTile(cellPos))
        {
            OnHexClicked?.Invoke(cellPos);
        }
        // ��ӵ�����Ϣ
        Debug.Log($"���λ�ã���Ļ {eventData.position} -> ���� {worldPos} -> ���� {cellPos}");

        if (tilemap.HasTile(cellPos))
        {
            OnHexClicked?.Invoke(cellPos);
        }
    }
}