using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
   

    public TurnManager turnManager;
    public CardManager cardManager;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
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
        Debug.Log(win ? "You Win!" : "Game Over");
        // ������������Ϸ�����߼�
    }
}
