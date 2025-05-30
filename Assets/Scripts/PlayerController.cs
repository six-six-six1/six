using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Audio Settings")]
    public AudioClip moveStartSound;    // ��ʼ�ƶ���Ч
   // public AudioClip moveEndSound;      // �ƶ�������Ч
   // public AudioClip moveLoopSound;     // �ƶ�ѭ����Ч
    [Range(0, 1)] public float moveVolume = 0.8f;

    [Header("Visual Effects")]
    public ParticleSystem moveParticles;    // �ƶ�������Ч
    public GameObject teleportEffectPrefab; // ������ЧԤ����
    public GameObject moveTrailEffectPrefab; // �������ƶ���β��ЧԤ����
    public float effectDuration = 1f;       // ��Ч����ʱ��

    private Vector3 targetPosition;
    private bool isMoving;
    private Coroutine movementRoutine;
    private AudioSource audioSource;
    private GameObject currentTrailEffect; // ��ǰ��β��Чʵ��
    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // ��ʼ����Ƶ���
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        targetPosition = transform.position;
    }

    // ������Ч�ĸ�������
    private void PlaySound(AudioClip clip, float volume = 1f, bool loop = false)
    {
        if (clip != null && audioSource != null)
        {
            if (loop)
            {
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.volume = volume;
                audioSource.Play();
            }
            else
            {
                audioSource.PlayOneShot(clip, volume);
            }
        }
    }

    // ֹͣ��ǰ��Ч
    private void StopSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
        }
    }

    public void MoveToHex(Vector3Int hexCoords)
    {
        Debug.Log($"�����ƶ���: {hexCoords}");
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

        if (movementRoutine != null)
            StopCoroutine(movementRoutine);

        movementRoutine = StartCoroutine(MovementRoutine(hexCoords));
    }

    public void SetPlayerPos(Vector3 position)
    {
        transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D other)
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

        // ���ſ�ʼ�ƶ���Ч
        PlaySound(moveStartSound, moveVolume);

        // �����ƶ�ѭ����Ч
     //   PlaySound(moveLoopSound, moveVolume * 0.6f, true);

        // �����ƶ�������Ч
        if (moveParticles != null)
        {
            moveParticles.Play();
        }

        // 4. �����ƶ���β��Ч
        if (moveTrailEffectPrefab != null)
        {
            currentTrailEffect = Instantiate(moveTrailEffectPrefab, transform.position, Quaternion.identity);
            currentTrailEffect.transform.SetParent(transform); // ��β�������
        }

        Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(target);
        Debug.Log($"Ŀ����������: {targetPos} ���룺{Vector3.Distance(transform.position, targetPos)}");

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            // ������β��Чλ��
            if (currentTrailEffect != null)
            {
                currentTrailEffect.transform.position = transform.position;
            }
            yield return null;
        }

        transform.position = targetPos; // ǿ��λ�ö���
        Debug.Log("�ƶ����");
        isMoving = false;

        // ֹͣ�ƶ���Ч����Ч
        StopSound();
        if (moveParticles != null)
        {
            moveParticles.Stop();
        }

        // 3. ������β��Ч
        if (currentTrailEffect != null)
        {
            Destroy(currentTrailEffect);
            currentTrailEffect = null;
        }
        // �����ƶ�������Ч
        // PlaySound(moveEndSound, moveVolume * 0.8f);

        Vector3Int cellPos = HexGridSystem.Instance.WorldToCell(targetPos);

        if (HexGridSystem.Instance.IsExitHexTile(cellPos))
        {
            GameManager.Instance.GameOver(true);
        }
        else if (HexGridSystem.Instance.IsCardHexTile(cellPos))
        {
            // ������
            CardManager.Instance.AddCard(3);
            HexGridSystem.Instance.SetNormalTile(cellPos);
        }
        else if (HexGridSystem.Instance.IsTeleportHexTile(cellPos))
        {
            // ��������Ч
            if (teleportEffectPrefab != null)
            {
                GameObject effect = Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, effectDuration);
            }
            TeleportSystem.Instance.Trigger(cellPos);
        }
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null; // ÿ֡���£�ȷ�� Cinemachine �ܸ���
        }
        transform.position = targetPos;
        isMoving = false;
        CardUIManager.Instance?.EndMoveTargetSelection();
        Debug.Log("�ƶ���ɲ��������");
    }
}