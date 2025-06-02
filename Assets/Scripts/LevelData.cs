using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewLevel", menuName = "关卡/关卡数据")]
public class LevelData : ScriptableObject
{
    [Header("基础信息")]
    public int levelID = 1;                  // 唯一关卡编号（如101=第1章第1关）
    public string levelName = "默认关卡";     // 关卡显示名称
    [TextArea] public string description;    // 关卡描述
    public Sprite thumbnail;                 // 关卡选择界面的缩略图

    [Header("场景配置")]
    public string sceneName;                 // 关联的场景名称
    // 或使用SceneAsset（需UnityEditor命名空间）
    // public SceneAsset sceneAsset;         // 更安全的场景引用方式

    [Header("卡牌设置")]
    public int initialHandSize = 3;          // 本关卡初始手牌数
    public int maxHandSize = 7;              // 本关卡手牌上限
    public int cardsPerTurn = 2;             // 本关卡每回合抽牌数

    [Header("黑雾设置")]
    public bool limitDarkTileExpansion = true; // 是否限制黑雾传染
    public int maxExpansionPerTurn = 2;       // 每回合最大传染数量


    [Header("关卡状态")]
    public bool isUnlocked;                  // 是否已解锁

    // 存储解锁状态到PlayerPrefs
    public void SaveUnlockState()
    {
        PlayerPrefs.SetInt($"Level_{levelID}_Unlocked", isUnlocked ? 1 : 0);
    }

    // 从PlayerPrefs加载解锁状态
    public void LoadUnlockState()
    {
        isUnlocked = PlayerPrefs.GetInt($"Level_{levelID}_Unlocked", 0) == 1;
    }
}
