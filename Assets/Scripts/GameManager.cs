using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int initialCardCount = 5;
    public int cardsPerTurn = 2;
    public int maxHandSize = 7;

    [Header("References")]
    public Transform playerPrefab;
    public Transform exitPoint;
    public Transform PlayerSpawn;

    // �����ֶ�
    [Header("UI References")]
    public GameObject winUI;
    public GameObject loseUI;
    public Button nextLevelButton;
    public Button retryButton;


    public TurnManager turnManager;
    public CardManager cardManager;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        // ��ʼ����ť�¼�
        nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        retryButton.onClick.AddListener(ReloadCurrentLevel);

        // ����UI
        winUI.SetActive(false);
        loseUI.SetActive(false);


        SpawnPlayer();

        // ȷ��TurnManager���ڲ���ʼ��
        if (turnManager == null)
        {
            turnManager = FindObjectOfType<TurnManager>();
        }

        // ȷ��DarkTileSystem�ѳ�ʼ��
        if (DarkTileSystem.Instance == null)
        {
            var darkTileSystem = FindObjectOfType<DarkTileSystem>();
            if (darkTileSystem != null) darkTileSystem.Init();
        }

        // �������󶨻غϽ����¼�
        turnManager.onTurnEnded.AddListener(OnTurnEnded);
        turnManager.StartPlayerTurn();


        BlockPillarSystem.Instance.Init();
    }
    private void OnDestroy()
    {
        // ����¼���������ֹ�ڴ�й©
        if (turnManager != null)
        {
            turnManager.onTurnEnded.RemoveListener(OnTurnEnded);
        }
    }
    private void OnTurnEnded()
    {
        // ����ÿ�غϸ�Ⱦ1-2���ؿ�
        int expandCount = Random.Range(1, 3);
        DarkTileSystem.Instance.ExpandDarkTiles(expandCount);
    }
    private void SpawnPlayer()
    {
        Instantiate(playerPrefab, GetStartPosition(), Quaternion.identity);
    }

    private Vector3 GetStartPosition()
    {
        // ��ʵ�� - ʵ��Ӧ�ø��ݵ�ͼ���ȷ��
       Vector3 playeralive = new Vector3(PlayerSpawn.position.x, PlayerSpawn.transform.position.y, PlayerSpawn.transform.position.z);

    return playeralive;
    }
    public void GameOver(bool win)
    {
        if (win)
        {
            ShowWinUI();
        }
        else
        {
            ShowLoseUI();
        }

        // ��ͣ��Ϸ�߼�
        Time.timeScale = 0;
    }
    private void ShowWinUI()
    {
        if (winUI == null)
        {
            Debug.LogError("winUIδ��Inspector�и�ֵ!");
            return;
        }

        winUI.SetActive(true);

        bool hasNextLevel = LevelManager.Instance != null && LevelManager.Instance.HasNextLevel();

        if (nextLevelButton != null)
        {
            nextLevelButton.interactable = hasNextLevel;
        }
        else
        {
            Debug.LogError("nextLevelButtonδ��Inspector�и�ֵ!");
        }
    }

    private void ShowLoseUI()
    {
        loseUI.SetActive(true);
    }

    private void OnNextLevelClicked()
    {
        Time.timeScale = 1; // �ָ���Ϸʱ��
        LevelManager.Instance.UnlockAndLoadNextLevel();
    }



    public void ReloadCurrentLevel()
    {
        Time.timeScale = 1;
        // ��ȡ�������¼�����
        if (turnManager != null)
        {
            turnManager.onTurnEnded.RemoveAllListeners();
        }
        // ���س���
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
