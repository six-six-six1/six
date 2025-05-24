using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    private Vector3 targetPosition;
    private bool isMoving;
    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                // 到达目标位置后的逻辑
            }
        }
    }

    public void MoveToHex(Vector3Int hexCoords)
    {
        if (!HexGridSystem.Instance.IsHexWalkable(hexCoords))
        {
            Debug.LogWarning($"目标位置 {hexCoords} 不可行走");
            return;
        }

        if (isMoving)
        {
            Debug.LogWarning("正在移动中，无法接受新指令");
            return;
        }

        StartCoroutine(MovementRoutine(hexCoords));
    }

    private void OnTriggerEnter2D(Collider2D other)//胜利条件
    {
        if (other.CompareTag("ExitPoint"))
        {
            GameManager.Instance.GameOver(true);
        }
    }

    private IEnumerator MovementRoutine(Vector3Int target)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                HexGridSystem.Instance.GetHexCenterPosition(target),
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        Debug.Log($"移动完成，当前位置：{transform.position}");
        isMoving = false;
    }
}
