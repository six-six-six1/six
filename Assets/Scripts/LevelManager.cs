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
        }
        else
        {
            Destroy(gameObject);
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

        // 同步配置到卡牌管理器
        if (CardManager.Instance != null)
        {
            CardManager.Instance.ApplyLevelSettings(targetLevel);
        }
    }

    /// <summary>
    /// 获取第一个未解锁的关卡
    /// </summary>
    public LevelData GetNextUnlockedLevel()
    {
        // 实际项目中可结合PlayerPrefs存储解锁状态
        return allLevels.Find(x => !x.isUnlocked);
    }
}
