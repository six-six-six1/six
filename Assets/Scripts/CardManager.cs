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


    private void Awake()
    {
        if (Instance == null) Instance = this;
        // else Destroy(gameObject);
        ValidateProbabilities();
    }

    private void Start()
    {

        DrawInitialHand();
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
            totalProb += card.drawProbability;
        }

        // 自动归一化处理
        if (Mathf.Abs(totalProb - 100f) > 0.1f)
        {
            Debug.LogWarning($"概率总和非100%（当前{totalProb}%），已自动调整");
            foreach (var card in allCardTypes)
            {
                card.drawProbability = (card.drawProbability / totalProb) * 100f;
            }
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
        int canDraw = Mathf.Min(count, maxHandSize - currentHand.Count);
        for (int i = 0; i < canDraw; i++)
        {
            DrawProbabilityBasedCard();
        }
    }
    /// <summary>
    /// 核心抽卡方法 - 按概率直接抽取
    /// </summary>
    private void DrawProbabilityBasedCard()
    {
        // 1. 计算概率分布
        float[] probabilities = new float[5];
        for (int i = 0; i < 5; i++)
        {
            probabilities[i] = allCardTypes[i].drawProbability;
        }

        // 2. 按概率随机选择
        int cardIndex = GetRandomCardIndex(probabilities);
        CardData drawnCard = allCardTypes[cardIndex];

        // 3. 加入手牌
        currentHand.Add(drawnCard);
        Debug.Log($"抽到: {drawnCard.type} (概率: {drawnCard.drawProbability}%)");

        // 4. 更新UI
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

    

    void DrawInitialHand()
    {
        for (int i = 0; i < initialHandSize; i++)
        {
            DrawProbabilityBasedCard();
        }
    }

    
    public bool PlayCard(CardData card, GameObject cardUIInstance = null)
    {
        if (!currentHand.Remove(card)) return false;

        if (cardUIInstance != null)
        {
            StartCoroutine(FadeOutAndDestroy(cardUIInstance));
        }

        CardUIManager.Instance?.UpdateHandUI(currentHand);

        if (currentHand.Count == 0)
        {
            TurnManager.Instance?.EndPlayerTurn();
        }

        return true;
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