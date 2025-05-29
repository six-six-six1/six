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
   
    [Header("�ؿ�����")]
    [SerializeField] private int currentLevel = 1;  // ��ǰ�ؿ�
    public int CurrentLevel => currentLevel;        // ��ǰ�ؿ�����


    private void Awake()
    {
        // ����ģʽ��ʼ��
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �糡��������
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ValidateProbabilities(); // ��֤��������
        DrawInitialHand();
    }

    /// <summary>
    /// ���õ�ǰ�ؿ�
    /// </summary>
    /// <param name="level">Ŀ��ؿ�</param>
    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, 5); // ���ƹؿ���Χ1-5
        Debug.Log($"�л����ؿ�{currentLevel}");
        ValidateProbabilities(); // ������֤����
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
            totalProb += card.GetProbabilityForLevel(currentLevel);
        }

        // �������ܺ��Ƿ�Ϊ100%
        if (Mathf.Abs(totalProb - 100f) > 0.1f)
        {
            Debug.LogWarning($"�ؿ�{currentLevel}�����ܺ�Ϊ{totalProb}%��ӦΪ100%��");
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
        // ����ʵ�ʿɳ�ȡ�������������������ޣ�
        int canDraw = Mathf.Min(count, maxHandSize - currentHand.Count);
        for (int i = 0; i < canDraw; i++)
        {
            DrawCard(); // ��ȡ���ſ���
        }
    }
   
    /// <summary>
    /// ���ĳ鿨�߼�
    /// </summary>
    private void DrawCard()
    {
        // ��������Ƿ�����
        if (currentHand.Count >= maxHandSize)
        {
            Debug.LogWarning("����������");
            return;
        }

        // 1. ���㵱ǰ�ؿ��ĸ��ʷֲ�
        float[] probabilities = new float[allCardTypes.Count];
        float totalProb = 0f;

        // ��ȡÿ�ֿ����ڵ�ǰ�ؿ��ĸ���
        for (int i = 0; i < allCardTypes.Count; i++)
        {
            probabilities[i] = allCardTypes[i].GetProbabilityForLevel(currentLevel);
            totalProb += probabilities[i];
        }

        // 2. ���ʹ�һ����ȷ���ܺ�Ϊ100%��
        if (Mathf.Abs(totalProb - 100f) > 0.1f)
        {
            for (int i = 0; i < probabilities.Length; i++)
            {
                probabilities[i] = (probabilities[i] / totalProb) * 100f;
            }
        }

        // 3. ���������ѡ����
        float randomPoint = Random.Range(0f, 100f); // �����
        float cumulative = 0f;                     // �ۼƸ���
        CardData drawnCard = allCardTypes[0];      // Ĭ�Ͽ���

        // ���ݸ��ʷֲ�ѡ����
        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulative += probabilities[i];
            if (randomPoint <= cumulative)
            {
                drawnCard = allCardTypes[i];
                break;
            }
        }

        // 4. ��ӿ��Ƶ�����
        currentHand.Add(drawnCard);
        Debug.Log($"��ȡ����: {drawnCard.type} (�ؿ�{currentLevel}����: {drawnCard.GetProbabilityForLevel(currentLevel)}%)");

        // 5. ����UI��ʾ
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


    /// <summary>
    /// ��ȡ��ʼ����
    /// </summary>
    void DrawInitialHand()
    {
        currentHand.Clear(); // ��յ�ǰ����
        for (int i = 0; i < initialHandSize; i++)
        {
            DrawCard(); // ��ȡ��ʼ����
        }
    }

    
    public bool PlayCard(CardData card, GameObject cardUIInstance = null)
    {
        // �������Ƴ�����
        if (!currentHand.Remove(card)) return false;

        // ���ٿ���UI����
        if (cardUIInstance != null)
        {
            Destroy(cardUIInstance);
        }

        // ����UI��ʾ
        CardUIManager.Instance?.UpdateHandUI(currentHand);

        // �������Ϊ�գ������غ�
        if (currentHand.Count == 0)
        {
            TurnManager.Instance?.EndPlayerTurn();
        }

        return true;
    }
    public void ApplyLevelSettings(LevelData levelData)
    {
        // ������������
        initialHandSize = levelData.initialHandSize;
        maxHandSize = levelData.maxHandSize;
        cardsPerTurn = levelData.cardsPerTurn;

        // ���õ�ǰ�ؿ�ID
        currentLevel = levelData.levelID;

        // Ӧ��ȫ�ָ��ʵ�������ѡ��
        foreach (var card in allCardTypes)
        {
            card.ApplyLevelMultiplier(levelData.probabilityMultiplier);
        }

        Debug.Log($"��Ӧ�ùؿ����ã�{levelData.levelName}");
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