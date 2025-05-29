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

    [Header("Level Settings")]
    public List<string> levelScenes = new List<string>(); // ��˳���Ÿ��ؿ�������
    private int currentLevelIndex = 0;

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
        nextLevelButton.onClick.AddListener(LoadNextLevel);
        retryButton.onClick.AddListener(ReloadCurrentLevel);

        // ����UI
        winUI.SetActive(false);
        loseUI.SetActive(false);


        SpawnPlayer();
        // �������󶨻غϽ����¼�
        turnManager.onTurnEnded.AddListener(OnTurnEnded);
        turnManager.StartPlayerTurn();

        BlockPillarSystem.Instance.Init();
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
        winUI.SetActive(true);
        // ��������һ�أ�����"��һ��"��ť
        nextLevelButton.interactable = currentLevelIndex < levelScenes.Count - 1;
    }

    private void ShowLoseUI()
    {
        loseUI.SetActive(true);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1; // �ָ�ʱ��
        currentLevelIndex++;

        if (currentLevelIndex < levelScenes.Count)
        {
            SceneManager.LoadScene(levelScenes[currentLevelIndex]);
        }
        else
        {
            // ���йؿ����
            SceneManager.LoadScene("MainMenu"); // �������˵�
        }
    }

    public void ReloadCurrentLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
