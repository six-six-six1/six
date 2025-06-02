using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ExitArrowGuide : MonoBehaviour
{
    [Header("UI References")]
    public Image directionIndicator;  // ����ָʾ��UI
    public RectTransform indicatorRect;

    [Header("Settings")]
    public float edgeOffset = 200f;   // ��Ļ��Եƫ����

    private Camera mainCamera;
    private Vector3 exitWorldPosition;
    private bool needsUpdate = false; // ����������Ƿ���Ҫ����λ��


    private void Start()
    {
        mainCamera = Camera.main;
        FindExitPosition();
        directionIndicator.enabled = false;

        // ע���¼�����
        TeleportSystem.OnExitPositionChanged += HandleExitPositionChanged;
    }

    private void OnDestroy()
    {
        // �Ƴ��¼�����
        TeleportSystem.OnExitPositionChanged -= HandleExitPositionChanged;
    }

    private void Update()
    {
        if (exitWorldPosition == Vector3.zero || needsUpdate)
        {
            FindExitPosition();
            needsUpdate = false;
        }

        if (exitWorldPosition == Vector3.zero) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(exitWorldPosition);
        bool isOffScreen = screenPos.x <= 0 || screenPos.x >= Screen.width ||
                          screenPos.y <= 0 || screenPos.y >= Screen.height;

        directionIndicator.enabled = isOffScreen;

        if (isOffScreen)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 screenBounds = screenCenter - new Vector3(edgeOffset, edgeOffset, 0);

            Vector3 direction = (screenPos - screenCenter).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            indicatorRect.rotation = Quaternion.Euler(0, 0, angle);
            indicatorRect.anchoredPosition = direction * edgeOffset;
        }
    }

    private void FindExitPosition()
    {
        Tilemap tilemap = HexGridSystem.Instance?.tilemap;
        if (tilemap == null) return;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (HexGridSystem.Instance.IsExitHexTile(pos))
            {
                exitWorldPosition = HexGridSystem.Instance.GetHexCenterPosition(pos);
                break;
            }
        }
    }

    // �������������λ�ñ仯�¼�
    private void HandleExitPositionChanged(Vector3Int newExitPos)
    {
        exitWorldPosition = HexGridSystem.Instance.GetHexCenterPosition(newExitPos);
    }   
}
