using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 特效管理系统
/// 功能：管理游戏中的视觉特效和音效播放
/// </summary>
public class EffectManager : MonoBehaviour
{
    [Header("视觉特效")]
    public GameObject shockwaveEffectPrefab;    // 冲击波行进特效预制体
    public GameObject tileDestroyEffectPrefab;  // 地块销毁特效预制体

    [Header("音效配置")]
    public AudioClip shockwaveSound;           // 冲击波音效
    public AudioClip tileDestroySound;         // 地块销毁音效

    [Range(0, 1)] public float soundVolume = 0.8f; // 音效音量

    public static EffectManager Instance;

    private AudioSource audioSource; // 音频播放组件

    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
            // 添加音频源组件
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = soundVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 播放冲击波特效（含音效）
    /// </summary>
    /// <param name="startPos">起始位置</param>
    /// <param name="endPos">结束位置</param>
    /// <param name="duration">持续时间</param>
    public void PlayShockwaveEffect(Vector3 startPos, Vector3 endPos, float duration)
    {
        // 播放视觉特效
        GameObject effect = Instantiate(shockwaveEffectPrefab, startPos, Quaternion.identity);

        // 设置冲击波方向
        Vector3 direction = (endPos - startPos).normalized;
        if (direction != Vector3.zero)
        {
            effect.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }

        // 设置冲击波长度
        float distance = Vector3.Distance(startPos, endPos);
        effect.transform.localScale = new Vector3(1, distance, 1);

        // 播放冲击波音效
        if (shockwaveSound != null)
        {
            audioSource.PlayOneShot(shockwaveSound);
        }
        else
        {
            Debug.LogWarning("未设置冲击波音效！");
        }

        Destroy(effect, duration);
    }

    /// <summary>
    /// 播放地块销毁特效（含音效）
    /// </summary>
    /// <param name="position">特效位置</param>
    public void PlayTileDestroyEffect(Vector3 position)
    {
        // 播放视觉特效
        GameObject effect = Instantiate(tileDestroyEffectPrefab, position, Quaternion.identity);

        // 播放销毁音效
        if (tileDestroySound != null)
        {
            audioSource.PlayOneShot(tileDestroySound);
        }
        else
        {
            Debug.LogWarning("未设置地块销毁音效！");
        }

        Destroy(effect, 2f); // 2秒后自动销毁
    }

    /// <summary>
    /// 设置全局音效音量
    /// </summary>
    /// <param name="volume">0-1之间的音量值</param>
    public void SetSoundVolume(float volume)
    {
        soundVolume = Mathf.Clamp01(volume);
        audioSource.volume = soundVolume;
    }
}