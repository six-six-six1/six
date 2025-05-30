using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("冲击波特效")]
    public GameObject shockwaveEffectPrefab;  // 冲击波行进特效
    public GameObject tileDestroyEffectPrefab; // 地块销毁特效

    public static EffectManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 播放冲击波特效
    /// </summary>
    public void PlayShockwaveEffect(Vector3 startPos, Vector3 endPos, float duration)
    {
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

        Destroy(effect, duration);
    }

    /// <summary>
    /// 播放地块销毁特效
    /// </summary>
    public void PlayTileDestroyEffect(Vector3 position)
    {
        GameObject effect = Instantiate(tileDestroyEffectPrefab, position, Quaternion.identity);
        Destroy(effect, 2f); // 2秒后自动销毁
    }
}
