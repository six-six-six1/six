// TilemapClickHandler.cs 新建脚本

using System;
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
        // 1. 检查是否收到点击事件
        Debug.Log("收到点击事件！");

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;
        Debug.Log($"屏幕坐标: {eventData.position} → 世界坐标: {worldPos}");
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
        Debug.Log($"格子坐标: {cellPos}, 是否有Tile: {tilemap.HasTile(cellPos)}");
        //if (tilemap.HasTile(cellPos))
        //{
        //    OnHexClicked?.Invoke(cellPos);
        //}

    }


    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;
            Vector3Int cellPos = tilemap.WorldToCell(worldPos);
            var temp_Tilemap = tilemap.GetTile(cellPos);

            //Debug.LogError(tilemap.GetColor(cellPos));
            Debug.Log(
                $"点击位置：屏幕 {Input.mousePosition} -> 世界 {worldPos} -> 格子坐标 {cellPos}, 是否有Tile: {tilemap.HasTile(cellPos)}");
            if (tilemap.HasTile(cellPos))
            {

                for (int i = 0; i < HexGridSystem.Instance.highlightTileList.Count; i++)
                {
                    if (temp_Tilemap == HexGridSystem.Instance.highlightTileList[i])
                    {
                        Debug.Log("触发 Hex 点击事件");

                        OnHexClicked?.Invoke(cellPos);
                    }
                }
            }
        }
    }
}