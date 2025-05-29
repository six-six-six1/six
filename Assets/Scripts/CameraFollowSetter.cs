using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFollowSetter : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        // ȷ��PlayerԤ�����Ѿ�ʵ����
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && virtualCamera != null)
        {
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
        else
        {
            Debug.LogError("Player or Virtual Camera not found!");
        }
    }
}
