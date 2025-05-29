using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Ҫ���������
    public float smoothing = 5f;  // ����ƽ����
    public Vector3 offset;  // �������Ŀ���ƫ����

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
        }
    }
}
