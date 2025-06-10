using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public int cardsPerTurn = 2; // 每回合补充牌数
    public int maxCardsPerTurn = 7; // 设为极大值，改为手动结束回合

    [Header("Audio Settings")]
    public AudioClip turnEndSound; // 回合结束音效
    [Range(0, 1)] public float turnEndVolume = 0.8f;
    private AudioSource audioSource;

    // 新增回合事件
    public UnityEvent onTurnStarted;
    public UnityEvent onTurnEnded;

    private int currentTurn;
    public bool isPlayerTurn;
    public int CurrentTurn { get; private set; } = 1;
    public int CardsPlayedThisTurn { get; private set; }

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

        // 初始化音频组件
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // 播放音效的辅助方法
    private void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void StartPlayerTurn()
    {
        CurrentTurn++;
        CardsPlayedThisTurn = 0; // 重置本回合出牌计数
        isPlayerTurn = true;

        Debug.Log($"回合 {CurrentTurn} 开始");
        // 触发回合开始事件
        onTurnStarted?.Invoke();
        // 每回合开始补充卡牌
        CardManager.Instance.RefillHand();
    }

    public void EndPlayerTurn()
    {

        Debug.Log($"第四关回合结束前 - CurrentTurn: {CurrentTurn}, isPlayerTurn: {isPlayerTurn}");
        isPlayerTurn = false;
        Debug.Log("回合结束");

        // 播放回合结束音效
        PlaySound(turnEndSound, turnEndVolume);

        // 触发回合结束事件
        onTurnEnded?.Invoke();
        // 直接开始新回合（移除敌人回合延迟）
        StartPlayerTurn();
        Debug.Log($"第四关回合结束后 - CurrentTurn: {CurrentTurn}, isPlayerTurn: {isPlayerTurn}");
    }

    public void RegisterCardPlayed()
    {
        CardsPlayedThisTurn++;

        // 自动结束回合逻辑（可选）
        if (CardsPlayedThisTurn >= maxCardsPerTurn)
        {
            EndPlayerTurn();
        }
    }

    public bool CanPlayCard()
    {
        return isPlayerTurn; // 只要在玩家回合就可以出牌
    }
    // 在TurnManager.cs中添加重置方法
public void ResetTurnManager()
    {
        CurrentTurn = 1;
        CardsPlayedThisTurn = 0;
        isPlayerTurn = true;

        // 清除所有事件监听
        onTurnStarted.RemoveAllListeners();
        onTurnEnded.RemoveAllListeners();

        // 重新初始化
        StartPlayerTurn();
    }
}