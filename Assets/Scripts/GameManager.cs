using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
// ��Ϸ������������ȫ����Ϸ�߼�����
public class GameManager : MonoBehaviour
{
    // ����ʵ��
    public static GameManager Instance;

    // ��Ϸ���ò���
    [Header("Game Settings")]
    public int initialCardCount = 5; // ��ʼ��������
    public int cardsPerTurn = 2;      // ÿ�غϳ�������
    public int maxHandSize = 7;       // �����������

    // ���ò���
    [Header("References")]
    public Transform playerPrefab;    // ���Ԥ����
    public Transform exitPoint;       // ���ڵ�
    public Transform PlayerSpawn;     // ������ɵ�

    // UI���ò���
    [Header("UI References")]
    public GameObject winUI;         // ʤ��UI
    public GameObject loseUI;        // ʧ��UI
    public Button nextLevelButton;   // ��һ�ذ�ť
    public Button retryButton;       // ���԰�ť

    // ����������
    public TurnManager turnManager;  // �غϹ�����
    public CardManager cardManager;  // ���ƹ�����

    // Awake�ڶ����ʼ��ʱ����
    private void Awake()
    {
        // ʵ�ֵ���ģʽ
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // ����Ѵ���ʵ����������ʵ��
    }

    // Start��Awake֮�󣬵�һ֡����ǰ����
    private void Start()
    {
        // ��ʼ����ť�¼�
        nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        retryButton.onClick.AddListener(ReloadCurrentLevel);

        // ��ʼ����UI
        winUI.SetActive(false);
        loseUI.SetActive(false);

        // �������
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

        // �󶨻غϽ����¼�
        turnManager.onTurnEnded.AddListener(OnTurnEnded);
        turnManager.StartPlayerTurn(); // ��ʼ��һغ�

        // ��ʼ���赲��ϵͳ
        BlockPillarSystem.Instance.Init();
    }

    // ��������ʱ����
    private void OnDestroy()
    {
        // ����¼���������ֹ�ڴ�й©
        if (turnManager != null)
        {
            turnManager.onTurnEnded.RemoveListener(OnTurnEnded);
        }
    }

    // �غϽ���ʱ�Ļص�����
    private void OnTurnEnded()
    {
        // ��ȡ��ǰ�ؿ�����
        LevelData currentLevel = LevelManager.Instance?.currentLevelData;

        if (currentLevel != null && !currentLevel.limitDarkTileExpansion)
        {
            // �����ƴ�Ⱦ
            DarkTileSystem.Instance.ExpandDarkTiles(100, true); // ʹ�ô����ֺ�isUnlimited=true
        }
        else
        {
            // �����ƴ�Ⱦ
            int expandCount = Random.Range(1, currentLevel.maxExpansionPerTurn + 1);
            DarkTileSystem.Instance.ExpandDarkTiles(expandCount);
        }
    }

    // �������
    private void SpawnPlayer()
    {
        Instantiate(playerPrefab, GetStartPosition(), Quaternion.identity);
    }

    // ��ȡ��ʼλ��
    private Vector3 GetStartPosition()
    {
        // ����������ɵ��λ��
        Vector3 playeralive = new Vector3(
            PlayerSpawn.position.x,
            PlayerSpawn.transform.position.y,
            PlayerSpawn.transform.position.z
        );
        return playeralive;
    }

    // ��Ϸ��������
    // ��Ϸ��������
    public void GameOver(bool win)
    {
        // ��ͣ��Ϸ�߼�
        Time.timeScale = 0;

        if (win)
        {
            ShowWinUI(); // ��ʾʤ��UI
        }
        else
        {
            ShowLoseUI(); // ��ʾʧ��UI
        }
    }

    // ��ʾʤ��UI
    private void ShowWinUI()
    {
        if (winUI == null)
        {
            Debug.LogError("winUIδ��Inspector�и�ֵ!");
            return;
        }

        winUI.SetActive(true);

        // ����Ƿ�����һ��
        bool hasNextLevel = LevelManager.Instance != null && LevelManager.Instance.HasNextLevel();

        // ��ȡ��ť����
        Button nextLevelBtn = winUI.transform.Find("NextLevelButton")?.GetComponent<Button>();
        Button menuBtn = winUI.transform.Find("MenuButton")?.GetComponent<Button>();
        Button restartBtn = winUI.transform.Find("RestartButton")?.GetComponent<Button>();

        // ������һ�ذ�ť״̬
        if (nextLevelBtn != null)
        {
            nextLevelBtn.interactable = hasNextLevel;
            nextLevelBtn.onClick.RemoveAllListeners();
            nextLevelBtn.onClick.AddListener(OnNextLevelClicked);
        }

        // ���ò˵���ť
        if (menuBtn != null)
        {
            menuBtn.onClick.RemoveAllListeners();
            menuBtn.onClick.AddListener(() => LoadSceneByIndex(0)); // 0�ǿ�ʼ����������
        }

        // �������¿�ʼ��ť
        if (restartBtn != null)
        {
            restartBtn.onClick.RemoveAllListeners();
            restartBtn.onClick.AddListener(ReloadCurrentLevel);

        }
    }

    // ��ʾʧ��UI
    private void ShowLoseUI()
    {
        if (loseUI == null)
        {
            Debug.LogError("loseUIδ��Inspector�и�ֵ!");
            return;
        }

        loseUI.SetActive(true);

        // ��ȡ��ť����
        Button retryBtn = loseUI.transform.Find("RetryButton")?.GetComponent<Button>();
        Button menuBtn = loseUI.transform.Find("MenuButton")?.GetComponent<Button>();

        // �������԰�ť
        if (retryBtn != null)
        {
            retryBtn.onClick.RemoveAllListeners();
            retryBtn.onClick.AddListener(ReloadCurrentLevel);
        }

        // ���ò˵���ť
        if (menuBtn != null)
        {
            menuBtn.onClick.RemoveAllListeners();
            menuBtn.onClick.AddListener(() => LoadSceneByIndex(0)); // 0�ǿ�ʼ����������
        }
    }

    // �������������س���
    private void LoadSceneByIndex(int sceneIndex)
    {
        Time.timeScale = 1; // �ָ���Ϸʱ��
        SceneManager.LoadScene(sceneIndex);
    }

    // ��һ�ذ�ť����¼�
    private void OnNextLevelClicked()
    {
        Time.timeScale = 1; // �ָ���Ϸʱ��
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1); // ������һ��
    }

    // ���¼��ص�ǰ�ؿ�
    public void ReloadCurrentLevel()
    {
        Time.timeScale = 1; // �ָ���Ϸʱ��

        // ������ƹ���������������
        if (CardManager.Instance != null)
        {
            CardManager.Instance.currentHand.Clear();
        }

        // ���ص�ǰ����
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}