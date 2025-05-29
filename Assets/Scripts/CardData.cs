using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Hex Escape/Card Data")]
public class CardData : ScriptableObject
{
    public enum CardType { Move, EnergyStone, Replenish, Shock, Teleport }

    public CardType type;
    public Sprite icon;
    public string description;



    [Header("Move Card")]
    public int moveDistance = 1;

    [Header("Shock Card")]
    public int maxClearDistance = 3;

    [Header("基础概率")]
    [Tooltip("基础抽取概率(0-100)")]
    [Range(1, 100)]
    public float baseProbability = 20f;  // 默认基础概率20%

    [Header("关卡概率调整")]
    [Tooltip("每个关卡的独立概率调整")]
    public List<LevelProbability> levelProbabilities = new List<LevelProbability>();

    // 关卡概率调整数据结构
    [System.Serializable]
    public class LevelProbability
    {
        public int level;                 // 关卡编号
        [Tooltip("基础概率调整值(-50到+50)")]
        [Range(-50, 50)]
        public float probabilityAdjustment; // 概率调整值
    }

    /// <summary>
    /// 获取指定关卡的最终概率
    /// </summary>
    /// <param name="level">目标关卡</param>
    /// <returns>计算后的概率值</returns>
    public float GetProbabilityForLevel(int level)
    {
        float probability = baseProbability;  // 从基础概率开始

        // 应用该关卡的调整值
        foreach (var lp in levelProbabilities)
        {
            if (lp.level == level)
            {
                probability += lp.probabilityAdjustment;
                break;
            }
        }

        // 确保概率在1%-100%范围内
        return Mathf.Clamp(probability, 1f, 100f);
    }

    // 编辑器模式下自动验证
#if UNITY_EDITOR
    private void OnValidate()
    {
        // 确保包含所有关卡的调整配置（假设有5个关卡）
        for (int i = 1; i <= 5; i++)
        {
            bool exists = false;
            foreach (var lp in levelProbabilities)
            {
                if (lp.level == i)
                {
                    exists = true;
                    break;
                }
            }

            // 如果缺少某个关卡的配置，自动添加默认值
            if (!exists)
            {
                levelProbabilities.Add(new LevelProbability { level = i });
            }
        }
    }
#endif
}
