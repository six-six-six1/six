using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class camerafarset : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float searchDelay = 0.5f; // 可选：自定义延迟时间

    IEnumerator Start()
    {
        // 等待一帧（或指定时间）确保 Player 生成
        yield return new WaitForSeconds(searchDelay);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found! 请检查：1. Player 的 Tag 是否为 'Player'；2. Player 是否已生成。");
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
