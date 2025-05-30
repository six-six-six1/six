using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("关卡列表")]
    public List<LevelData> allLevels = new List<LevelData>();

    [Header("当前关卡")]
    public LevelData currentLevelData;       // 当前运行的关卡数据

    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLevels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 初始化关卡解锁状态
    private void InitializeLevels()
    {
        // 默认解锁第一关
        if (allLevels.Count > 0)
        {
            allLevels[0].isUnlocked = true;
            allLevels[0].SaveUnlockState();
        }

        // 加载其他关卡的解锁状态
        foreach (var level in allLevels)
        {
            level.LoadUnlockState();
        }
    }

    /// <summary>
    /// 加载指定关卡
    /// </summary>
    public void LoadLevel(int targetLevelID)
    {
        LevelData targetLevel = allLevels.Find(x => x.levelID == targetLevelID);
        if (targetLevel == null)
        {
            Debug.LogError($"找不到ID为{targetLevelID}的关卡！");
            return;
        }

        currentLevelData = targetLevel;
        SceneManager.LoadScene(targetLevel.sceneName);

        // 确保卡牌管理器应用新关卡设置
        if (CardManager.Instance != null)
        {
            CardManager.Instance.ApplyLevelSettings(targetLevel);
        }
    }

    /// <summary>
    /// 解锁并加载下一关
    /// </summary>
    public void UnlockAndLoadNextLevel()
    {
        if (currentLevelData == null) return;

        int nextLevelID = currentLevelData.levelID + 1;
        LevelData nextLevel = allLevels.Find(x => x.levelID == nextLevelID);

        if (nextLevel != null)
        {
            nextLevel.isUnlocked = true;
            nextLevel.SaveUnlockState();
            LoadLevel(nextLevelID);
        }
        else
        {
            Debug.Log("已经是最后一关，返回主菜单");
            SceneManager.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// 获取第一个未解锁的关卡
    /// </summary>
    public LevelData GetNextUnlockedLevel()
    {
        return allLevels.Find(x => !x.isUnlocked);
    }

    public bool HasNextLevel()
    {
        if (currentLevelData == null || allLevels == null || allLevels.Count == 0)
            return false;

        return currentLevelData.levelID < allLevels.Count;
    }
    // 在 LevelManager.cs 中添加
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentLevelData != null && CardManager.Instance != null)
        {
            CardManager.Instance.ApplyLevelSettings(currentLevelData);
        }
    }
}
