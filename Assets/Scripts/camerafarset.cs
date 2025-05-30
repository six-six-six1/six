using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class camerafarset : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float searchDelay = 0.5f; // ��ѡ���Զ����ӳ�ʱ��

    IEnumerator Start()
    {
        // �ȴ�һ֡����ָ��ʱ�䣩ȷ�� Player ����
        yield return new WaitForSeconds(searchDelay);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! ���飺1. Player �� Tag �Ƿ�Ϊ 'Player'��2. Player �Ƿ������ɡ�");
            yield break;
        }

        if (virtualCamera == null)
        {
            Debug.LogError("Virtual Camera not found!");
            yield break;
        }

        virtualCamera.Follow = player.transform;
        virtualCamera.LookAt = player.transform;
    }
}
