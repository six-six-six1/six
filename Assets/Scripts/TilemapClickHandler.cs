// TilemapClickHandler.cs 新建脚本
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
        // 添加调试信息
        Debug.Log($"点击位置：屏幕 {eventData.position} -> 世界 {worldPos} -> 格子 {cellPos}");

        if (tilemap.HasTile(cellPos))
        {
            OnHexClicked?.Invoke(cellPos);
        }
    }
}