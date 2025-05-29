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

    [Header("Level Settings")]
    public List<string> levelScenes = new List<string>(); // 按顺序存放各关卡场景名
    private int currentLevelIndex = 0;

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
        nextLevelButton.onClick.AddListener(LoadNextLevel);
        retryButton.onClick.AddListener(ReloadCurrentLevel);

        // 隐藏UI
        winUI.SetActive(false);
        loseUI.SetActive(false);


        SpawnPlayer();
        // 新增：绑定回合结束事件
        turnManager.onTurnEnded.AddListener(OnTurnEnded);
        turnManager.StartPlayerTurn();

        BlockPillarSystem.Instance.Init();
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
        winUI.SetActive(true);
        // 如果是最后一关，禁用"下一关"按钮
        nextLevelButton.interactable = currentLevelIndex < levelScenes.Count - 1;
    }

    private void ShowLoseUI()
    {
        loseUI.SetActive(true);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1; // 恢复时间
        currentLevelIndex++;

        if (currentLevelIndex < levelScenes.Count)
        {
            SceneManager.LoadScene(levelScenes[currentLevelIndex]);
        }
        else
        {
            // 所有关卡完成
            SceneManager.LoadScene("MainMenu"); // 返回主菜单
        }
    }

    public void ReloadCurrentLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
