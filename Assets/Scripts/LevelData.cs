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
    public int initialHandSize = 5;          // 本关卡初始手牌数
    public int maxHandSize = 7;              // 本关卡手牌上限
    public int cardsPerTurn = 2;             // 本关卡每回合抽牌数

    [Header("概率全局调整")]
    [Range(-50, 50)]
    public float probabilityMultiplier = 0;  // 全局概率调整系数（百分比）

    // 在LevelData中添加
    public bool isUnlocked;

    // 存储到PlayerPrefs
    public void SaveUnlockState()
    {
        PlayerPrefs.SetInt($"Level_{levelID}_Unlocked", isUnlocked ? 1 : 0);
    }
}
