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

    [Header("��������")]
    [Tooltip("������ȡ����(0-100)")]
    [Range(1, 100)]
    public float baseProbability = 20f;  // Ĭ�ϻ�������20%

    [Header("�ؿ����ʵ���")]
    [Tooltip("ÿ���ؿ��Ķ������ʵ���")]
    public List<LevelProbability> levelProbabilities = new List<LevelProbability>();

    // �ؿ����ʵ������ݽṹ
    [System.Serializable]
    public class LevelProbability
    {
        public int level;                 // �ؿ����
        [Tooltip("�������ʵ���ֵ(-50��+50)")]
        [Range(-50, 50)]
        public float probabilityAdjustment; // ���ʵ���ֵ
    }

    /// <summary>
    /// ��ȡָ���ؿ������ո���
    /// </summary>
    /// <param name="level">Ŀ��ؿ�</param>
    /// <returns>�����ĸ���ֵ</returns>
    public float GetProbabilityForLevel(int level)
    {
        float probability = baseProbability;  // �ӻ������ʿ�ʼ

        // Ӧ�øùؿ��ĵ���ֵ
        foreach (var lp in levelProbabilities)
        {
            if (lp.level == level)
            {
                probability += lp.probabilityAdjustment;
                break;
            }
        }

        // ȷ��������1%-100%��Χ��
        return Mathf.Clamp(probability, 1f, 100f);
    }

    // �༭��ģʽ���Զ���֤
#if UNITY_EDITOR
    private void OnValidate()
    {
        // ȷ���������йؿ��ĵ������ã�������5���ؿ���
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

            // ���ȱ��ĳ���ؿ������ã��Զ����Ĭ��ֵ
            if (!exists)
            {
                levelProbabilities.Add(new LevelProbability { level = i });
            }
        }
    }
#endif
}
