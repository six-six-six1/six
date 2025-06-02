using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreathingTiles : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase breathingTile;
    public float speed = 1f;
    public Color minColor = new Color(0.5f, 0.5f, 0.5f);
    public Color maxColor = Color.white;

    private void Update()
    {
        // 获取所有呼吸Tile的位置
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.GetTile(pos) == breathingTile)
            {
                float breathValue = (Mathf.Sin(Time.time * speed + pos.x + pos.y) + 1) / 2f;
                Color currentColor = Color.Lerp(minColor, maxColor, breathValue);
                tilemap.SetColor(pos, currentColor);
            }
        }
    }
}
