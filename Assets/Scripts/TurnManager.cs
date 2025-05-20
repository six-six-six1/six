using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public int cardsPerTurn = 2; // 每回合补充牌数
    public int maxCardsPerTurn = 7; // 设为极大值，改为手动结束回合
                                    // 新增回合事件
    public UnityEvent onTurnStarted;
    public UnityEvent onTurnEnded;

    private int currentTurn;
    private bool isPlayerTurn;
    public int CurrentTurn { get; private set; } = 1;
    public int CardsPlayedThisTurn { get; private set; }

   
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartPlayerTurn()
    {
        CurrentTurn++;
        Debug.Log($"回合 {CurrentTurn} 开始");
        // 触发回合开始事件
        onTurnStarted?.Invoke();
        // 每回合开始补充卡牌
        CardManager.Instance.RefillHand();
    }

    public void EndPlayerTurn()
    {
        Debug.Log("回合结束");
        // 触发回合结束事件
        onTurnEnded?.Invoke();
        // 直接开始新回合（移除敌人回合延迟）
        StartPlayerTurn();
    }

    
    private void RegisterCardPlayed()
    {
        CardsPlayedThisTurn++;

        // 自动结束回合逻辑（可选）
        if (CardsPlayedThisTurn >= maxCardsPerTurn)
        {
            EndPlayerTurn();
        }
    }
    //private IEnumerator EnemyTurn()
    //{
    //    yield return new WaitForSeconds(1f); // 模拟敌人思考时间
    //    StartPlayerTurn(); // 回到玩家回合
    //}

    public bool CanPlayCard()
    {
        return isPlayerTurn; // 只要在玩家回合就可以出牌
    }
}