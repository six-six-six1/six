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
    public AudioClip drawCardSound;      // 抽卡音效
    public AudioClip playCardSound;      // 使用卡牌音效
    [Range(0, 1)] public float drawVolume = 0.8f;
    [Range(0, 1)] public float playVolume = 0.8f;

    private AudioSource audioSource;
    public List<CardData> currentHand = new List<CardData>();
    public static CardManager Instance;
    public List<CardData> CurrentHand => currentHand;

    [Header("关卡设置")]
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
        DrawInitialHand(silent: true); // 初始抽牌不播放音效
    }

    // 播放音效的辅助方法
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
        Debug.Log($"切换到关卡{currentLevel}");
        ValidateProbabilities();
    }

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

        if (Mathf.Abs(totalProb - 100f) > 0.1f)
        {
            Debug.LogWarning($"关卡{currentLevel}概率总和为{totalProb}%（应为100%）");
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
    /// 抽取初始手牌
    /// </summary>
    /// <param name="silent">是否静默模式（不播放音效）</param>
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
            Debug.LogWarning("手牌已满！");
            return;
        }

        float totalProb = 0f;
        foreach (var card in allCardTypes)
        {
            totalProb += card.GetProbabilityForLevel(currentLevel);
        }

        // 随机选择卡牌
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
        Debug.Log($"抽取卡牌: {drawnCard.type} (关卡{currentLevel}概率: {drawnCard.GetProbabilityForLevel(currentLevel)}%)");

        // 只在非静默模式下播放音效
        if (!silent)
        {
            PlaySound(drawCardSound, drawVolume, Random.Range(0.9f, 1.1f));
        }

        CardUIManager.Instance?.UpdateHandUI(currentHand);
    }

    public bool PlayCard(CardData card, GameObject cardUIInstance = null)
    {
        if (!currentHand.Remove(card)) return false;

        // 播放使用卡牌音效
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
        Debug.Log($"已应用关卡设置: {levelData.levelName}");
        Debug.Log($"当前关卡ID: {currentLevel}");

        ValidateProbabilities();
        DrawInitialHand(silent: true); // 关卡切换时初始抽牌也不播放音效
    }

    public void ResetForNewLevel()
    {
        currentHand.Clear();
        ValidateProbabilities();
        DrawInitialHand(silent: true); // 新关卡初始抽牌不播放音效
    }
}