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

    public List<CardData> currentHand = new List<CardData>();
    public static CardManager Instance;
    // ��ӹ���������
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
    /// ��֤��������
    /// </summary>
    private void ValidateProbabilities()
    {
        if (allCardTypes.Count != 5)
        {
            Debug.LogError("��������5�ֿ��ƣ�");
            return;
        }

        float totalProb = 0f;
        foreach (var card in allCardTypes)
        {
            totalProb += card.drawProbability;
        }

        // �Զ���һ������
        if (Mathf.Abs(totalProb - 100f) > 0.1f)
        {
            Debug.LogWarning($"�����ܺͷ�100%����ǰ{totalProb}%�������Զ�����");
            foreach (var card in allCardTypes)
            {
                card.drawProbability = (card.drawProbability / totalProb) * 100f;
            }
        }
    }

    // �ϲ�Ϊһ��RefillHand����
    public void RefillHand()
    {
        AddCard(cardsPerTurn);
    }

    // ������������
    public void AddCard(int count)
    {
        int canDraw = Mathf.Min(count, maxHandSize - currentHand.Count);
        for (int i = 0; i < canDraw; i++)
        {
            DrawProbabilityBasedCard();
        }
    }
    /// <summary>
    /// ���ĳ鿨���� - ������ֱ�ӳ�ȡ
    /// </summary>
    private void DrawProbabilityBasedCard()
    {
        // 1. ������ʷֲ�
        float[] probabilities = new float[5];
        for (int i = 0; i < 5; i++)
        {
            probabilities[i] = allCardTypes[i].drawProbability;
        }

        // 2. ���������ѡ��
        int cardIndex = GetRandomCardIndex(probabilities);
        CardData drawnCard = allCardTypes[cardIndex];

        // 3. ��������
        currentHand.Add(drawnCard);
        Debug.Log($"�鵽: {drawnCard.type} (����: {drawnCard.drawProbability}%)");

        // 4. ����UI
        CardUIManager.Instance?.UpdateHandUI(currentHand);
    }

    /// <summary>
    /// ���ݸ��ʷֲ����ѡ��������
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

        return 0; // Ĭ�Ϸ��ص�һ��
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