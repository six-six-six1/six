using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Settings")]
    public List<CardData> allCardTypes;
    public int initialHandSize = 7; // ��Ϊ��ʼ7��
    public int maxHandSize = 7;
    public int cardsPerTurn = 2; // ÿ�غϲ���2��

    public List<CardData> currentDeck = new List<CardData>();
    public List<CardData> currentHand = new List<CardData>();
    public static CardManager Instance;
    // ��ӹ���������
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

    // �ϲ�Ϊһ��RefillHand����
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
        // Fisher-Yatesϴ���㷨
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
        Debug.Log($"���ڳ��ʼ���ƣ��ƿ�ʣ��:{currentDeck.Count}");
        for (int i = 0; i < initialHandSize; i++)
        {
            if (currentDeck.Count == 0)
            {
                Debug.LogError("�ƿ�Ϊ�գ�");
                break;
            }
            DrawCard();
        }
    }

    private void DrawCard()
    {
        // ��ȫУ��
        if (currentDeck == null || currentDeck.Count == 0)
        {
            Debug.LogWarning("�ƿ�Ϊ��");
            return;
        }

        if (currentHand == null)
        {
            Debug.LogWarning("�����б�δ��ʼ���������޸�...");
            currentHand = new List<CardData>();
        }

        // �鿨�߼�
        CardData drawnCard = currentDeck[0];
        currentDeck.RemoveAt(0);
        currentHand.Add(drawnCard);

        // ����UI�����null��飩
        if (CardUIManager.Instance != null)
        {
            CardUIManager.Instance.UpdateHandUI(currentHand);
        }
        else
        {
            Debug.LogError("CardUIManager ʵ����ʧ��");
        }

        Debug.Log($"�鿨: {drawnCard.type} (ʣ��:{currentDeck.Count})");
    }

    public bool PlayCard(CardData card, GameObject cardUIInstance = null)
    {
        if (!currentHand.Remove(card)) return false;

        // ���ٿ���UI
        if (cardUIInstance != null)
        {
            Destroy(cardUIInstance);
        }

        // ��������UI
        CardUIManager.Instance?.UpdateHandUI(currentHand);

        // ��������Ƿ�Ϊ��
        if (currentHand.Count == 0)
        {
            Debug.Log("���������꣬�Զ������غ�");
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