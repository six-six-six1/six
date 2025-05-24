using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")] public float moveSpeed = 5f;

    private Vector3 targetPosition;
    private bool isMoving;
    private Coroutine movementRoutine;
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

    // private void Update()
    // {
    //     if (isMoving)
    //     {
    //         transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    //         if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
    //         {
    //             isMoving = false;
    //             // 到达目标位置后的逻辑
    //         }
    //     }
    // }

    public void MoveToHex(Vector3Int hexCoords)
    {
        Debug.Log($"请求移动到: {hexCoords}");
        if (!HexGridSystem.Instance.IsHexWalkable(hexCoords))
        {
            Debug.LogWarning($"目标位置 {hexCoords} 不可行走");
            return;
        }

        Debug.Log($"isMoving: {isMoving}");

        if (isMoving)
        {
            Debug.LogWarning("正在移动中，无法接受新指令");
            return;
        }
        
        if (movementRoutine != null)
            StopCoroutine(movementRoutine);
        movementRoutine = StartCoroutine(MovementRoutine(hexCoords));
    }

    private void OnTriggerEnter2D(Collider2D other) //胜利条件
    {
        if (other.CompareTag("ExitPoint"))
        {
            GameManager.Instance.GameOver(true);
        }
    }

    private IEnumerator MovementRoutine(Vector3Int target)
    {
        Debug.Log("移动协程开始");
        isMoving = true;
        Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(target);
        Debug.Log($"目标世界坐标: {targetPos} 距离：{Vector3.Distance(transform.position, targetPos)}");

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            //Debug.Log($"目标世界坐标: {targetPos} 距离：{Vector3.Distance(transform.position, targetPos)}移动的距离{moveSpeed * Time.deltaTime}");

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPos; // 强制位置对齐
        Debug.Log("移动完成");
        isMoving = false;
    }
}