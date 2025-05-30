using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("�������Ч")]
    public GameObject shockwaveEffectPrefab;  // ������н���Ч
    public GameObject tileDestroyEffectPrefab; // �ؿ�������Ч

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
    /// ���ų������Ч
    /// </summary>
    public void PlayShockwaveEffect(Vector3 startPos, Vector3 endPos, float duration)
    {
        GameObject effect = Instantiate(shockwaveEffectPrefab, startPos, Quaternion.identity);

        // ���ó��������
        Vector3 direction = (endPos - startPos).normalized;
        if (direction != Vector3.zero)
        {
            effect.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }

        // ���ó��������
        float distance = Vector3.Distance(startPos, endPos);
        effect.transform.localScale = new Vector3(1, distance, 1);

        Destroy(effect, duration);
    }

    /// <summary>
    /// ���ŵؿ�������Ч
    /// </summary>
    public void PlayTileDestroyEffect(Vector3 position)
    {
        GameObject effect = Instantiate(tileDestroyEffectPrefab, position, Quaternion.identity);
        Destroy(effect, 2f); // 2����Զ�����
    }
}
