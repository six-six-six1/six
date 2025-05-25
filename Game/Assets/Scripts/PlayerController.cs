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
    //             // ����Ŀ��λ�ú���߼�
    //         }
    //     }
    // }

    public void MoveToHex(Vector3Int hexCoords)
    {
        Debug.Log($"�����ƶ���: {hexCoords}");
        if (!HexGridSystem.Instance.IsHexWalkable(hexCoords))
        {
            Debug.LogWarning($"Ŀ��λ�� {hexCoords} ��������");
            return;
        }

        Debug.Log($"isMoving: {isMoving}");

        if (isMoving)
        {
            Debug.LogWarning("�����ƶ��У��޷�������ָ��");
            return;
        }
        
        if (movementRoutine != null)
            StopCoroutine(movementRoutine);
        movementRoutine = StartCoroutine(MovementRoutine(hexCoords));
    }

    private void OnTriggerEnter2D(Collider2D other) //ʤ������
    {
        if (other.CompareTag("ExitPoint"))
        {
            GameManager.Instance.GameOver(true);
        }
    }

    private IEnumerator MovementRoutine(Vector3Int target)
    {
        Debug.Log("�ƶ�Э�̿�ʼ");
        isMoving = true;
        Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(target);
        Debug.Log($"Ŀ����������: {targetPos} ���룺{Vector3.Distance(transform.position, targetPos)}");

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            //Debug.Log($"Ŀ����������: {targetPos} ���룺{Vector3.Distance(transform.position, targetPos)}�ƶ��ľ���{moveSpeed * Time.deltaTime}");

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPos; // ǿ��λ�ö���
        Debug.Log("�ƶ����");
        isMoving = false;
    }
}