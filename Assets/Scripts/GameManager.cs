using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int initialCardCount = 5;
    public int cardsPerTurn = 2;
    public int maxHandSize = 7;

    [Header("References")]
    public Transform playerPrefab;
    public Transform exitPoint;

    public TurnManager turnManager;
    public CardManager cardManager;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        SpawnPlayer();
        // 新增：绑定回合结束事件
        turnManager.onTurnEnded.AddListener(OnTurnEnded);
        turnManager.StartPlayerTurn();
    }

    private void OnTurnEnded()
    {
        // 设置每回合感染1-2个地块
        int expandCount = Random.Range(1, 3);
        DarkTileSystem.Instance.ExpandDarkTiles(expandCount);
    }
    private void SpawnPlayer()
    {
        Instantiate(playerPrefab, GetStartPosition(), Quaternion.identity);
    }

    private Vector3 GetStartPosition()
    {
        // 简单实现 - 实际应该根据地图设计确定
        return new Vector3(0, 0, 0);
    }
    public void GameOver(bool win)
    {
        Debug.Log(win ? "You Win!" : "Game Over");
        // 这里可以添加游戏结束逻辑
    }
}
