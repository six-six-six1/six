using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Audio Settings")]
    public AudioClip moveStartSound;    // 开始移动音效
   // public AudioClip moveEndSound;      // 移动结束音效
   // public AudioClip moveLoopSound;     // 移动循环音效
    [Range(0, 1)] public float moveVolume = 0.8f;

    [Header("Visual Effects")]
    public ParticleSystem moveParticles;    // 移动粒子特效
    public GameObject teleportEffectPrefab; // 传送特效预制体
    public GameObject moveTrailEffectPrefab; // 新增：移动拖尾特效预制体
    public float effectDuration = 1f;       // 特效持续时间

    private Vector3 targetPosition;
    private bool isMoving;
    private Coroutine movementRoutine;
    private AudioSource audioSource;
    private GameObject currentTrailEffect; // 当前拖尾特效实例
    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 初始化音频组件
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        targetPosition = transform.position;
    }

    // 播放音效的辅助方法
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

    // 停止当前音效
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
        Debug.Log($"请求移动到: {hexCoords}");
        if (!HexGridSystem.Instance.IsHexWalkable(hexCoords))
        {
            Debug.LogWarning($"目标位置 {hexCoords} 不可行走");
            return;
        }

        if (isMoving)
        {
            Debug.LogWarning("正在移动中，无法接受新指令");
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
        Debug.Log("移动协程开始");
        isMoving = true;

        // 播放开始移动音效
        PlaySound(moveStartSound, moveVolume);

        // 播放移动循环音效
     //   PlaySound(moveLoopSound, moveVolume * 0.6f, true);

        // 启动移动粒子特效
        if (moveParticles != null)
        {
            moveParticles.Play();
        }

        // 4. 创建移动拖尾特效
        if (moveTrailEffectPrefab != null)
        {
            currentTrailEffect = Instantiate(moveTrailEffectPrefab, transform.position, Quaternion.identity);
            currentTrailEffect.transform.SetParent(transform); // 拖尾跟随玩家
        }

        Vector3 targetPos = HexGridSystem.Instance.GetHexCenterPosition(target);
        Debug.Log($"目标世界坐标: {targetPos} 距离：{Vector3.Distance(transform.position, targetPos)}");

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            // 更新拖尾特效位置
            if (currentTrailEffect != null)
            {
                currentTrailEffect.transform.position = transform.position;
            }
            yield return null;
        }

        transform.position = targetPos; // 强制位置对齐
        Debug.Log("移动完成");
        isMoving = false;

        // 停止移动音效和特效
        StopSound();
        if (moveParticles != null)
        {
            moveParticles.Stop();
        }

        // 3. 销毁拖尾特效
        if (currentTrailEffect != null)
        {
            Destroy(currentTrailEffect);
            currentTrailEffect = null;
        }
        // 播放移动结束音效
        // PlaySound(moveEndSound, moveVolume * 0.8f);

        Vector3Int cellPos = HexGridSystem.Instance.WorldToCell(targetPos);

        if (HexGridSystem.Instance.IsExitHexTile(cellPos))
        {
            GameManager.Instance.GameOver(true);
        }
        else if (HexGridSystem.Instance.IsCardHexTile(cellPos))
        {
            // 补卡点
            CardManager.Instance.AddCard(3);
            HexGridSystem.Instance.SetNormalTile(cellPos);
        }
        else if (HexGridSystem.Instance.IsTeleportHexTile(cellPos))
        {
            // 传送门特效
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
            yield return null; // 每帧更新，确保 Cinemachine 能跟随
        }
        transform.position = targetPos;
        isMoving = false;
        CardUIManager.Instance?.EndMoveTargetSelection();
        Debug.Log("移动完成并清除高亮");
    }
}