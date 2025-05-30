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

    [Header("各关卡概率配置")]
    [Tooltip("每个关卡的独立概率(1-100)")]
    [Range(1, 100)]
    public List<float> levelProbabilities = new List<float>() { 20f, 20f, 20f, 20f, 20f }; // 默认每个关卡20%

    /// <summary>
    /// 获取指定关卡的最终概率
    /// </summary>
    public float GetProbabilityForLevel(int level)
    {
        // 确保关卡编号在有效范围内
        int index = Mathf.Clamp(level - 1, 0, levelProbabilities.Count - 1);
        return Mathf.Clamp(levelProbabilities[index], 1f, 100f);
    }

    

    [Header("Move Card")]
    public int moveDistance = 1;

    [Header("Shock Card")]
    public int maxClearDistance = 3;


    // 编辑器模式下自动验证
#if UNITY_EDITOR
    private void OnValidate()
    {
        // 确保有5个关卡的配置
        while (levelProbabilities.Count < 5)
        {
            levelProbabilities.Add(20f); // 默认值
        }
    }
#endif

}
