using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TilemapClickHandler : MonoBehaviour, IPointerClickHandler
{
    public static event System.Action<Vector3Int> OnHexClicked;
    public static event System.Action<Vector3Int> OnBlockClicked;

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

            if (temp_Tilemap)
            {
                Debug.Log(
          $"点击位置：屏幕 {Input.mousePosition} -> 世界 {worldPos} -> 格子坐标 {cellPos}, 是否有Tile: {tilemap.HasTile(cellPos)},类型 -> {tilemap.GetTile(cellPos).name}");
            }
      

            if (HexGridSystem.Instance == null || HexGridSystem.Instance.highlightTileList == null)
            {
                Debug.LogError("HexGridSystem 或 highlightTileList 未初始化！");
                return;
            }

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

                for (int i = 0; i < BlockPillarSystem.Instance.blockPosList.Count; i++)
                {
                    if (cellPos == BlockPillarSystem.Instance.blockPosList[i])
                    {
                        OnBlockClicked?.Invoke(cellPos);
                    }
                }
            }
        }
    }
}