using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ч����ϵͳ
/// ���ܣ�������Ϸ�е��Ӿ���Ч����Ч����
/// </summary>
public class EffectManager : MonoBehaviour
{
    [Header("�Ӿ���Ч")]
    public GameObject shockwaveEffectPrefab;    // ������н���ЧԤ����
    public GameObject tileDestroyEffectPrefab;  // �ؿ�������ЧԤ����

    [Header("��Ч����")]
    public AudioClip shockwaveSound;           // �������Ч
    public AudioClip tileDestroySound;         // �ؿ�������Ч

    [Range(0, 1)] public float soundVolume = 0.8f; // ��Ч����

    public static EffectManager Instance;

    private AudioSource audioSource; // ��Ƶ�������

    private void Awake()
    {
        // ����ģʽ��ʼ��
        if (Instance == null)
        {
            Instance = this;
            // �����ƵԴ���
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = soundVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ���ų������Ч������Ч��
    /// </summary>
    /// <param name="startPos">��ʼλ��</param>
    /// <param name="endPos">����λ��</param>
    /// <param name="duration">����ʱ��</param>
    public void PlayShockwaveEffect(Vector3 startPos, Vector3 endPos, float duration)
    {
        // �����Ӿ���Ч
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

        // ���ų������Ч
        if (shockwaveSound != null)
        {
            audioSource.PlayOneShot(shockwaveSound);
        }
        else
        {
            Debug.LogWarning("δ���ó������Ч��");
        }

        Destroy(effect, duration);
    }

    /// <summary>
    /// ���ŵؿ�������Ч������Ч��
    /// </summary>
    /// <param name="position">��Чλ��</param>
    public void PlayTileDestroyEffect(Vector3 position)
    {
        // �����Ӿ���Ч
        GameObject effect = Instantiate(tileDestroyEffectPrefab, position, Quaternion.identity);

        // ����������Ч
        if (tileDestroySound != null)
        {
            audioSource.PlayOneShot(tileDestroySound);
        }
        else
        {
            Debug.LogWarning("δ���õؿ�������Ч��");
        }

        Destroy(effect, 2f); // 2����Զ�����
    }

    /// <summary>
    /// ����ȫ����Ч����
    /// </summary>
    /// <param name="volume">0-1֮�������ֵ</param>
    public void SetSoundVolume(float volume)
    {
        soundVolume = Mathf.Clamp01(volume);
        audioSource.volume = soundVolume;
    }
}