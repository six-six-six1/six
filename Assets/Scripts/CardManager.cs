using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CardManager : MonoBehaviour
{ // 添加单例模式

    [Header("Settings")]
    public List<CardData> cardDeck;
    public int initialHandSize = 5;
    public int cardsPerTurn = 2;
    public int maxHandSize = 7;

    private List<CardData> currentDeck;
    private List<CardData> currentHand;
    private List<CardData> discardPile = new List<CardData>(); // 新增弃牌堆

    public CardUIManager cardUIManager;
    public static CardManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeDeck();
        DrawInitialHand();
    }

    private void InitializeDeck()
    {
        currentDeck = new List<CardData>(cardDeck);
        ShuffleDeck();
        Debug.Log($"初始牌库: {currentDeck.Count}张卡");
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < currentDeck.Count; i++)
        {
            CardData temp = currentDeck[i];
            int randomIndex = Random.Range(i, currentDeck.Count);
            currentDeck[i] = currentDeck[randomIndex];
            currentDeck[randomIndex] = temp;
        }
    }

    private void DrawInitialHand()
    {
        currentHand = new List<CardData>();
        for (int i = 0; i < initialHandSize; i++)
        {
            DrawCardWithReshuffle();
        }
    }

    public void DrawCardsForTurn()
    {
        for (int i = 0; i < cardsPerTurn; i++)
        {
            if (currentHand.Count < maxHandSize)
            {
                DrawCardWithReshuffle();
            }
            else
            {
                Debug.Log("手牌已达上限，不再抽卡");
                break;
            }
        }
    }

    // 新增：带自动洗牌的抽卡方法
    private void DrawCardWithReshuffle()
    {
        if (currentDeck.Count == 0)
        {
            ReshuffleDiscardPile();
        }

        if (currentDeck.Count > 0)
        {
            DrawCard();
        }
        else
        {
            Debug.LogWarning("牌库和弃牌堆均已空，无法抽卡");
        }
    }

    // 新增：重新洗入弃牌堆
    private void ReshuffleDiscardPile()
    {
        if (discardPile.Count > 0)
        {
            Debug.Log($"将{discardPile.Count}张弃牌洗回牌库");
            currentDeck.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
        }
    }

    private void DrawCard()
    {
        CardData drawnCard = currentDeck[0];
        currentDeck.RemoveAt(0);
        currentHand.Add(drawnCard);
        cardUIManager.UpdateHandUI(currentHand);

        Debug.Log($"抽卡: {drawnCard.type} (剩余:{currentDeck.Count})");
    }

    public bool PlayCard(CardData card)
    {
        if (!TurnManager.Instance.CanPlayCard())
        {
            Debug.Log("当前不能使用卡牌：不在玩家回合或已达出牌上限");
            return false;
        }

        Debug.Log($"尝试使用卡牌：{card.type}");

        // 实际卡牌效果调用（示例：移动卡）
        switch (card.type)
        {
            case CardData.CardType.Move:
                PlayMoveCard(Vector3Int.right); // 示例方向
                break;
                // 其他卡牌类型...
        }

        TurnManager.Instance.RegisterCardPlayed();
        return true;
    }

    // 其他卡牌效果方法保持不变...
    public void PlayMoveCard(Vector3Int direction) { /* 原有实现 */ }
    public void PlayShockCard(Vector3Int direction) { /* 原有实现 */ }
    public void PlayEnergyStoneCard(Vector3Int pillarPosition) { /* 原有实现 */ }
    public void PlayTeleportCard() { /* 原有实现 */ }
}
