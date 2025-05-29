using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Settings")]
    public List<CardData> allCardTypes;
    public int initialHandSize = 7; // 改为初始7张
    public int maxHandSize = 7;
    public int cardsPerTurn = 2; // 每回合补充2张

    public List<CardData> currentHand = new List<CardData>();
    public static CardManager Instance;
    // 添加公共访问器
    public List<CardData> CurrentHand => currentHand;
   
    [Header("关卡设置")]
    [SerializeField] private int currentLevel = 1;  // 当前关卡
    public int CurrentLevel => currentLevel;        // 当前关卡属性


    private void Awake()
    {
        // 单例模式初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景不销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ValidateProbabilities(); // 验证概率配置
        DrawInitialHand();
    }

    /// <summary>
    /// 设置当前关卡
    /// </summary>
    /// <param name="level">目标关卡</param>
    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, 5); // 限制关卡范围1-5
        Debug.Log($"切换到关卡{currentLevel}");
        ValidateProbabilities(); // 重新验证概率
    }


    /// <summary>
    /// 验证概率配置
    /// </summary>
    private void ValidateProbabilities()
    {
        if (allCardTypes.Count != 5)
        {
            Debug.LogError("必须配置5种卡牌！");
            return;
        }

        float totalProb = 0f;
        foreach (var card in allCardTypes)
        {
            totalProb += card.GetProbabilityForLevel(currentLevel);
        }

        // 检查概率总和是否为100%
        if (Mathf.Abs(totalProb - 100f) > 0.1f)
        {
            Debug.LogWarning($"关卡{currentLevel}概率总和为{totalProb}%（应为100%）");
        }
    }

    // 合并为一个RefillHand方法
    public void RefillHand()
    {
        AddCard(cardsPerTurn);
    }

    // 补充手牌数量
    public void AddCard(int count)
    {
        // 计算实际可抽取数量（不超过手牌上限）
        int canDraw = Mathf.Min(count, maxHandSize - currentHand.Count);
        for (int i = 0; i < canDraw; i++)
        {
            DrawCard(); // 抽取单张卡牌
        }
    }
   
    /// <summary>
    /// 核心抽卡逻辑
    /// </summary>
    private void DrawCard()
    {
        // 检查手牌是否已满
        if (currentHand.Count >= maxHandSize)
        {
            Debug.LogWarning("手牌已满！");
            return;
        }

        // 1. 计算当前关卡的概率分布
        float[] probabilities = new float[allCardTypes.Count];
        float totalProb = 0f;

        // 获取每种卡牌在当前关卡的概率
        for (int i = 0; i < allCardTypes.Count; i++)
        {
            probabilities[i] = allCardTypes[i].GetProbabilityForLevel(currentLevel);
            totalProb += probabilities[i];
        }

        // 2. 概率归一化（确保总和为100%）
        if (Mathf.Abs(totalProb - 100f) > 0.1f)
        {
            for (int i = 0; i < probabilities.Length; i++)
            {
                probabilities[i] = (probabilities[i] / totalProb) * 100f;
            }
        }

        // 3. 按概率随机选择卡牌
        float randomPoint = Random.Range(0f, 100f); // 随机点
        float cumulative = 0f;                     // 累计概率
        CardData drawnCard = allCardTypes[0];      // 默认卡牌

        // 根据概率分布选择卡牌
        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulative += probabilities[i];
            if (randomPoint <= cumulative)
            {
                drawnCard = allCardTypes[i];
                break;
            }
        }

        // 4. 添加卡牌到手牌
        currentHand.Add(drawnCard);
        Debug.Log($"抽取卡牌: {drawnCard.type} (关卡{currentLevel}概率: {drawnCard.GetProbabilityForLevel(currentLevel)}%)");

        // 5. 更新UI显示
        CardUIManager.Instance?.UpdateHandUI(currentHand);
    }



    /// <summary>
    /// 根据概率分布随机选择卡牌索引
    /// </summary>
    private int GetRandomCardIndex(float[] probabilities)
    {
        float total = 0f;
        foreach (float prob in probabilities)
        {
            total += prob;
        }

        float randomPoint = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulative += probabilities[i];
            if (randomPoint <= cumulative)
            {
                return i;
            }
        }

        return 0; // 默认返回第一个
    }


    /// <summary>
    /// 抽取初始手牌
    /// </summary>
    void DrawInitialHand()
    {
        currentHand.Clear(); // 清空当前手牌
        for (int i = 0; i < initialHandSize; i++)
        {
            DrawCard(); // 抽取初始手牌
        }
    }

    
    public bool PlayCard(CardData card, GameObject cardUIInstance = null)
    {
        // 从手牌移除卡牌
        if (!currentHand.Remove(card)) return false;

        // 销毁卡牌UI对象
        if (cardUIInstance != null)
        {
            Destroy(cardUIInstance);
        }

        // 更新UI显示
        CardUIManager.Instance?.UpdateHandUI(currentHand);

        // 如果手牌为空，结束回合
        if (currentHand.Count == 0)
        {
            TurnManager.Instance?.EndPlayerTurn();
        }

        return true;
    }
    public void ApplyLevelSettings(LevelData levelData)
    {
        // 更新手牌设置
        initialHandSize = levelData.initialHandSize;
        maxHandSize = levelData.maxHandSize;
        cardsPerTurn = levelData.cardsPerTurn;

        // 设置当前关卡ID
        currentLevel = levelData.levelID;

        // 应用全局概率调整（可选）
        foreach (var card in allCardTypes)
        {
            card.ApplyLevelMultiplier(levelData.probabilityMultiplier);
        }

        Debug.Log($"已应用关卡设置：{levelData.levelName}");
    }

    private IEnumerator FadeOutAndDestroy(GameObject card)
    {
        CanvasGroup canvasGroup = card.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = card.AddComponent<CanvasGroup>();

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(card);
    }
}