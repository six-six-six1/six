using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
    public Transform PlayerSpawn;

    // 新增字段
    [Header("UI References")]
    public GameObject winUI;
    public GameObject loseUI;
    public Button nextLevelButton;
    public Button retryButton;


    public TurnManager turnManager;
    public CardManager cardManager;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        // 初始化按钮事件
        nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        retryButton.onClick.AddListener(ReloadCurrentLevel);

        // 隐藏UI
        winUI.SetActive(false);
        loseUI.SetActive(false);


        SpawnPlayer();

        // 确保TurnManager存在并初始化
        if (turnManager == null)
        {
            turnManager = FindObjectOfType<TurnManager>();
        }

        // 确保DarkTileSystem已初始化
        if (DarkTileSystem.Instance == null)
        {
            var darkTileSystem = FindObjectOfType<DarkTileSystem>();
            if (darkTileSystem != null) darkTileSystem.Init();
        }

        // 新增：绑定回合结束事件
        turnManager.onTurnEnded.AddListener(OnTurnEnded);
        turnManager.StartPlayerTurn();


        BlockPillarSystem.Instance.Init();
    }
    private void OnDestroy()
    {
        // 清除事件监听，防止内存泄漏
        if (turnManager != null)
        {
            turnManager.onTurnEnded.RemoveListener(OnTurnEnded);
        }
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
       Vector3 playeralive = new Vector3(PlayerSpawn.position.x, PlayerSpawn.transform.position.y, PlayerSpawn.transform.position.z);

    return playeralive;
    }
    public void GameOver(bool win)
    {
        if (win)
        {
            ShowWinUI();
        }
        else
        {
            ShowLoseUI();
        }

        // 暂停游戏逻辑
        Time.timeScale = 0;
    }
    private void ShowWinUI()
    {
        if (winUI == null)
        {
            Debug.LogError("winUI未在Inspector中赋值!");
            return;
        }

        winUI.SetActive(true);

        bool hasNextLevel = LevelManager.Instance != null && LevelManager.Instance.HasNextLevel();

        if (nextLevelButton != null)
        {
            nextLevelButton.interactable = hasNextLevel;
        }
        else
        {
            Debug.LogError("nextLevelButton未在Inspector中赋值!");
        }
    }

    private void ShowLoseUI()
    {
        loseUI.SetActive(true);
    }

    private void OnNextLevelClicked()
    {
        Time.timeScale = 1; // 恢复游戏时间
        LevelManager.Instance.UnlockAndLoadNextLevel();
    }



    public void ReloadCurrentLevel()
    {
        Time.timeScale = 1;
        // 先取消所有事件监听
        if (turnManager != null)
        {
            turnManager.onTurnEnded.RemoveAllListeners();
        }
        // 重载场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
