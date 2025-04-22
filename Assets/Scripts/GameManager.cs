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
        turnManager.StartPlayerTurn();
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
