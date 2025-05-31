using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Settings")]
    public List<CardData> allCardTypes;
    public int initialHandSize = 7;
    public int maxHandSize = 7;
    public int cardsPerTurn = 2;

    [Header("Audio Settings")]
    public AudioClip drawCardSound;      // �鿨��Ч
    public AudioClip playCardSound;      // ʹ�ÿ�����Ч
    [Range(0, 1)] public float drawVolume = 0.8f;
    [Range(0, 1)] public float playVolume = 0.8f;

    private AudioSource audioSource;
    public List<CardData> currentHand = new List<CardData>();
    public static CardManager Instance;
    public List<CardData> CurrentHand => currentHand;

    [Header("�ؿ�����")]
    [SerializeField] private int currentLevel = 1;
    public int CurrentLevel => currentLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        ValidateProbabilities();
        DrawInitialHand(silent: true); // ��ʼ���Ʋ�������Ч
    }

    // ������Ч�ĸ�������
    private void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 1, 5);
        Debug.Log($"�л����ؿ�{currentLevel}");
        ValidateProbabilities();
    }

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

        if (Mathf.Abs(totalProb - 100f) > 0.1f)
        {
            Debug.LogWarning($"�ؿ�{currentLevel}�����ܺ�Ϊ{totalProb}%��ӦΪ100%��");
        }
    }

    public void RefillHand()
    {
        AddCard(cardsPerTurn);
    }

    public void AddCard(int count)
    {
        int canDraw = Mathf.Min(count, maxHandSize - currentHand.Count);
        for (int i = 0; i < canDraw; i++)
        {
            DrawCard();
        }
    }

    /// <summary>
    /// ��ȡ��ʼ����
    /// </summary>
    /// <param name="silent">�Ƿ�Ĭģʽ����������Ч��</param>
    public void DrawInitialHand(bool silent = false)
    {
        currentHand.Clear();
        for (int i = 0; i < initialHandSize; i++)
        {
            DrawCard(silent);
        }
    }

    private void DrawCard(bool silent = false)
    {
        if (currentHand.Count >= maxHandSize)
        {
            Debug.LogWarning("����������");
            return;
        }

        float totalProb = 0f;
        foreach (var card in allCardTypes)
        {
            totalProb += card.GetProbabilityForLevel(currentLevel);
        }

        // ���ѡ����
        float randomPoint = Random.Range(0f, totalProb);
        float cumulative = 0f;
        CardData drawnCard = allCardTypes[0];

        foreach (var card in allCardTypes)
        {
            cumulative += card.GetProbabilityForLevel(currentLevel);
            if (randomPoint <= cumulative)
            {
                drawnCard = card;
                break;
            }
        }

        currentHand.Add(drawnCard);
        Debug.Log($"��ȡ����: {drawnCard.type} (�ؿ�{currentLevel}����: {drawnCard.GetProbabilityForLevel(currentLevel)}%)");

        // ֻ�ڷǾ�Ĭģʽ�²�����Ч
        if (!silent)
        {
            PlaySound(drawCardSound, drawVolume, Random.Range(0.9f, 1.1f));
        }

        CardUIManager.Instance?.UpdateHandUI(currentHand);
    }

    public bool PlayCard(CardData card, GameObject cardUIInstance = null)
    {
        if (!currentHand.Remove(card)) return false;

        // ����ʹ�ÿ�����Ч
        PlaySound(playCardSound, playVolume);

        if (cardUIInstance != null)
        {
            Destroy(cardUIInstance);
        }

        CardUIManager.Instance?.UpdateHandUI(currentHand);

        if (currentHand.Count == 0)
        {
            TurnManager.Instance?.EndPlayerTurn();
        }

        return true;
    }

    public void ApplyLevelSettings(LevelData levelData)
    {
        initialHandSize = levelData.initialHandSize;
        maxHandSize = levelData.maxHandSize;
        cardsPerTurn = levelData.cardsPerTurn;
        currentLevel = levelData.levelID;
        Debug.Log($"��Ӧ�ùؿ�����: {levelData.levelName}");
        Debug.Log($"��ǰ�ؿ�ID: {currentLevel}");

        ValidateProbabilities();
        DrawInitialHand(silent: true); // �ؿ��л�ʱ��ʼ����Ҳ��������Ч
    }

    public void ResetForNewLevel()
    {
        currentHand.Clear();
        ValidateProbabilities();
        DrawInitialHand(silent: true); // �¹ؿ���ʼ���Ʋ�������Ч
    }
}