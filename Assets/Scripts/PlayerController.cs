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

    public void MoveToHex(Vector3 hexPosition)
    {
        targetPosition = hexPosition;
        isMoving = true;
    }
    private void OnTriggerEnter2D(Collider2D other)//胜利条件
    {
        if (other.CompareTag("ExitPoint"))
        {
            GameManager.Instance.GameOver(true);
        }
    }
}
