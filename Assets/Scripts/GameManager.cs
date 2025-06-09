using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
// 游戏管理器，负责全局游戏逻辑控制
public class GameManager : MonoBehaviour
{
    // 单例实例
    public static GameManager Instance;

    // 游戏设置部分
    [Header("Game Settings")]
    public int initialCardCount = 5; // 初始手牌数量
    public int cardsPerTurn = 2;      // 每回合抽牌数量
    public int maxHandSize = 7;       // 最大手牌数量

    // 引用部分
    [Header("References")]
    public Transform playerPrefab;    // 玩家预制体
    public Transform exitPoint;       // 出口点
    public Transform PlayerSpawn;     // 玩家生成点

    // UI引用部分
    [Header("UI References")]
    public GameObject winUI;         // 胜利UI
    public GameObject loseUI;        // 失败UI
    public Button nextLevelButton;   // 下一关按钮
    public Button retryButton;       // 重试按钮
    public Button globalReturnButton; // 拖拽你的按钮到这个字段

    [Header("说明面板")]
    public GameObject helpPanel;      // 说明面板的UI对象
    public Button helpButton;         // 打开面板的按钮
    public Button closeHelpButton;    // 关闭面板的按钮

    // 管理器引用
    public TurnManager turnManager;  // 回合管理器
    public CardManager cardManager;  // 卡牌管理器

    // Awake在对象初始化时调用
    private void Awake()
    {
        // 实现单例模式
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // 如果已存在实例则销毁新实例
    }

    // Start在Awake之后，第一帧更新前调用
    private void Start()
    {
        // 初始化按钮事件
        nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        retryButton.onClick.AddListener(ReloadCurrentLevel);

        // 初始隐藏UI
        winUI.SetActive(false);
        loseUI.SetActive(false);

        // 生成玩家
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

        // 绑定回合结束事件
        turnManager.onTurnEnded.AddListener(OnTurnEnded);
        turnManager.StartPlayerTurn(); // 开始玩家回合

        // 初始化阻挡柱系统
        BlockPillarSystem.Instance.Init();
        // 初始化说明面板按钮
        if (helpButton != null)
        {
            helpButton.onClick.AddListener(ShowHelpPanel);
        }
        if (closeHelpButton != null)
        {
            closeHelpButton.onClick.AddListener(HideHelpPanel);
        }

        // 初始隐藏面板
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
        }
    }

    // 对象销毁时调用
    private void OnDestroy()
    {
        // 清除事件监听，防止内存泄漏
        if (turnManager != null)
        {
            turnManager.onTurnEnded.RemoveListener(OnTurnEnded);
        }
        if (globalReturnButton != null)
        {
            globalReturnButton.onClick.AddListener(ReturnToMenu);
        }
    }

    // 回合结束时的回调函数
    private void OnTurnEnded()
    {
        // 获取当前关卡数据
        LevelData currentLevel = LevelManager.Instance?.currentLevelData;

        if (currentLevel != null && !currentLevel.limitDarkTileExpansion)
        {
            // 无限制传染
            DarkTileSystem.Instance.ExpandDarkTiles(100, true); // 使用大数字和isUnlimited=true
        }
        else
        {
            // 有限制传染
            int expandCount = Random.Range(1, currentLevel.maxExpansionPerTurn + 1);
            DarkTileSystem.Instance.ExpandDarkTiles(expandCount);
        }
    }

    // 生成玩家
    private void SpawnPlayer()
    {
        Instantiate(playerPrefab, GetStartPosition(), Quaternion.identity);
    }

    // 获取起始位置
    private Vector3 GetStartPosition()
    {
        // 返回玩家生成点的位置
        Vector3 playeralive = new Vector3(
            PlayerSpawn.position.x,
            PlayerSpawn.transform.position.y,
            PlayerSpawn.transform.position.z
        );
        return playeralive;
    }

    // 游戏结束处理
    // 游戏结束处理
    public void GameOver(bool win)
    {
        // 暂停游戏逻辑
        Time.timeScale = 0;

        if (win)
        {
            // 检查是否是第五关（最后一关）
            if (LevelManager.Instance != null &&
                LevelManager.Instance.currentLevelData != null &&
                LevelManager.Instance.IsLastLevel())
            {
                // 如果是最后一关，显示通关界面
                if (EndGameManager.Instance != null)
                {
                    EndGameManager.Instance.ShowEndGameScreen();
                }
                else
                {
                    Debug.LogWarning("EndGameManager未找到，显示普通胜利UI");
                    ShowWinUI();
                }
            }
            else
            {
                // 不是最后一关，显示普通胜利UI
                ShowWinUI();
            }
        }
        else
        {
            ShowLoseUI(); // 显示失败UI
        }
    }

    // 显示胜利UI
    private void ShowWinUI()
    {
        if (winUI == null)
        {
            Debug.LogError("winUI未在Inspector中赋值!");
            return;
        }

        winUI.SetActive(true);

        // 检查是否有下一关
        bool hasNextLevel = LevelManager.Instance != null && LevelManager.Instance.HasNextLevel();

        // 获取按钮引用
        Button nextLevelBtn = winUI.transform.Find("NextLevelButton")?.GetComponent<Button>();
        Button menuBtn = winUI.transform.Find("MenuButton")?.GetComponent<Button>();
        Button restartBtn = winUI.transform.Find("RestartButton")?.GetComponent<Button>();

        // 设置下一关按钮状态
        if (nextLevelBtn != null)
        {
            nextLevelBtn.interactable = hasNextLevel;
            nextLevelBtn.onClick.RemoveAllListeners();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        // 设置菜单按钮
        if (menuBtn != null)
        {
            menuBtn.onClick.RemoveAllListeners();
            menuBtn.onClick.AddListener(() => LoadSceneByIndex(0)); // 0是开始场景的索引
        }

        // 设置重新开始按钮
        if (restartBtn != null)
        {
            restartBtn.onClick.RemoveAllListeners();
            restartBtn.onClick.AddListener(ReloadCurrentLevel);

        }
    }

    // 显示失败UI
    private void ShowLoseUI()
    {
        if (loseUI == null)
        {
            Debug.LogError("loseUI未在Inspector中赋值!");
            return;
        }

        loseUI.SetActive(true);

        // 获取按钮引用
        Button retryBtn = loseUI.transform.Find("RetryButton")?.GetComponent<Button>();
        Button menuBtn = loseUI.transform.Find("MenuButton")?.GetComponent<Button>();

        // 设置重试按钮
        if (retryBtn != null)
        {
            retryBtn.onClick.RemoveAllListeners();
            retryBtn.onClick.AddListener(ReloadCurrentLevel);
        }

        // 设置菜单按钮
        if (menuBtn != null)
        {
            menuBtn.onClick.RemoveAllListeners();
            menuBtn.onClick.AddListener(() => LoadSceneByIndex(0)); // 0是开始场景的索引
        }
    }
    // 显示说明面板
    public void ShowHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true);
            Time.timeScale = 0; // 可选：暂停游戏

        }
    }

    // 隐藏说明面板
    public void HideHelpPanel()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false);
            Time.timeScale = 1; // 恢复游戏
        }
    }
    // 按场景索引加载场景
    private void LoadSceneByIndex(int sceneIndex)
    {
        Time.timeScale = 1; // 恢复游戏时间
        SceneManager.LoadScene(sceneIndex);
    }

    // 下一关按钮点击事件
   public void OnNextLevelClicked()
    {
        Time.timeScale = 1; // 恢复游戏时间
                            // 使用LevelManager加载下一关
        if (LevelManager.Instance != null && LevelManager.Instance.currentLevelData != null)
        {
            int nextLevelID = LevelManager.Instance.currentLevelData.levelID + 1;
            LevelManager.Instance.LoadLevel(nextLevelID);
        }
        else
        {
            // 如果LevelManager不可用，回退到旧方法
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // 重新加载当前关卡
    public void ReloadCurrentLevel()
    {
        Time.timeScale = 1; // 恢复游戏时间

        // 清除所有事件监听
        if (CardUIManager.Instance != null)
        {
            CardUIManager.Instance.ClearAllEventListeners();
        }

        // 重置卡牌管理器
        if (CardManager.Instance != null)
        {
            CardManager.Instance.ResetForNewLevel();
        }

        // 重置回合管理器
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.ResetTurnManager();
        }

        // 重载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}