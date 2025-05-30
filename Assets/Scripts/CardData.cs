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

    [Header("���ؿ���������")]
    [Tooltip("ÿ���ؿ��Ķ�������(1-100)")]
    [Range(1, 100)]
    public List<float> levelProbabilities = new List<float>() { 20f, 20f, 20f, 20f, 20f }; // Ĭ��ÿ���ؿ�20%

    /// <summary>
    /// ��ȡָ���ؿ������ո���
    /// </summary>
    public float GetProbabilityForLevel(int level)
    {
        // ȷ���ؿ��������Ч��Χ��
        int index = Mathf.Clamp(level - 1, 0, levelProbabilities.Count - 1);
        return Mathf.Clamp(levelProbabilities[index], 1f, 100f);
    }

    

    [Header("Move Card")]
    public int moveDistance = 1;

    [Header("Shock Card")]
    public int maxClearDistance = 3;


    // �༭��ģʽ���Զ���֤
#if UNITY_EDITOR
    private void OnValidate()
    {
        // ȷ����5���ؿ�������
        while (levelProbabilities.Count < 5)
        {
            levelProbabilities.Add(20f); // Ĭ��ֵ
        }
    }
#endif

}
