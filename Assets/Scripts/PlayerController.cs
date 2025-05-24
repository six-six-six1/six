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
                // ����Ŀ��λ�ú���߼�
            }
        }
    }

    public void MoveToHex(Vector3Int hexCoords)
    {
        if (!HexGridSystem.Instance.IsHexWalkable(hexCoords))
        {
            Debug.LogWarning($"Ŀ��λ�� {hexCoords} ��������");
            return;
        }

        if (isMoving)
        {
            Debug.LogWarning("�����ƶ��У��޷�������ָ��");
            return;
        }

        StartCoroutine(MovementRoutine(hexCoords));
    }

    private void OnTriggerEnter2D(Collider2D other)//ʤ������
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

        Debug.Log($"�ƶ���ɣ���ǰλ�ã�{transform.position}");
        isMoving = false;
    }
}
