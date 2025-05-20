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

    public List<CardData> currentDeck = new List<CardData>();
    public List<CardData> currentHand = new List<CardData>();
    public static CardManager Instance;
    // 添加公共访问器
    public List<CardData> CurrentHand => currentHand;
    public int CardsRemaining => currentDeck.Count + currentHand.Count;

    private void Awake()
    {
        if (Instance == null) Instance = this;
       // else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeDeck();
        DrawInitialHand();
    }

    private void InitializeDeck()
    {
        currentDeck.Clear();
        foreach (var card in allCardTypes)
        {
            for (int i = 0; i < 5; i++) currentDeck.Add(card);
        }
        ShuffleDeck();
    }

    // 合并为一个RefillHand方法
    public void RefillHand()
    {
        int canDraw = Mathf.Min(
         cardsPerTurn,
         maxHandSize - currentHand.Count,
         currentDeck.Count
     );

        for (int i = 0; i < canDraw; i++) DrawCard();
    }

    private void ShuffleDeck()
    {
        // Fisher-Yates洗牌算法
        for (int i = currentDeck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            CardData temp = currentDeck[i];
            currentDeck[i] = currentDeck[j];
            currentDeck[j] = temp;
        }
    }

    void DrawInitialHand()
    {
        Debug.Log($"正在抽初始手牌，牌库剩余:{currentDeck.Count}");
        for (int i = 0; i < initialHandSize; i++)
        {
            if (currentDeck.Count == 0)
            {
                Debug.LogError("牌库为空！");
                break;
            }
            DrawCard();
        }
    }

    private void DrawCard()
    {
        // 安全校验
        if (currentDeck == null || currentDeck.Count == 0)
        {
            Debug.LogWarning("牌库为空");
            return;
        }

        if (currentHand == null)
        {
            Debug.LogWarning("手牌列表未初始化，正在修复...");
            currentHand = new List<CardData>();
        }

        // 抽卡逻辑
        CardData drawnCard = currentDeck[0];
        currentDeck.RemoveAt(0);
        currentHand.Add(drawnCard);

        // 更新UI（添加null检查）
        if (CardUIManager.Instance != null)
        {
            CardUIManager.Instance.UpdateHandUI(currentHand);
        }
        else
        {
            Debug.LogError("CardUIManager 实例丢失！");
        }

        Debug.Log($"抽卡: {drawnCard.type} (剩余:{currentDeck.Count})");
    }

    public bool PlayCard(CardData card, GameObject cardUIInstance = null)
    {
        if (!currentHand.Remove(card)) return false;

        // 销毁卡牌UI
        if (cardUIInstance != null)
        {
            Destroy(cardUIInstance);
        }

        // 更新手牌UI
        CardUIManager.Instance?.UpdateHandUI(currentHand);

        // 检查手牌是否为空
        if (currentHand.Count == 0)
        {
            Debug.Log("手牌已用完，自动结束回合");
            TurnManager.Instance.EndPlayerTurn();
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