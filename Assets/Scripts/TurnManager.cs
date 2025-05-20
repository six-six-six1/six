using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public int cardsPerTurn = 2; // ÿ�غϲ�������
    public int maxCardsPerTurn = 7; // ��Ϊ����ֵ����Ϊ�ֶ������غ�
                                    // �����غ��¼�
    public UnityEvent onTurnStarted;
    public UnityEvent onTurnEnded;

    private int currentTurn;
    private bool isPlayerTurn;
    public int CurrentTurn { get; private set; } = 1;
    public int CardsPlayedThisTurn { get; private set; }

   
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartPlayerTurn()
    {
        CurrentTurn++;
        Debug.Log($"�غ� {CurrentTurn} ��ʼ");
        // �����غϿ�ʼ�¼�
        onTurnStarted?.Invoke();
        // ÿ�غϿ�ʼ���俨��
        CardManager.Instance.RefillHand();
    }

    public void EndPlayerTurn()
    {
        Debug.Log("�غϽ���");
        // �����غϽ����¼�
        onTurnEnded?.Invoke();
        // ֱ�ӿ�ʼ�»غϣ��Ƴ����˻غ��ӳ٣�
        StartPlayerTurn();
    }

    
    private void RegisterCardPlayed()
    {
        CardsPlayedThisTurn++;

        // �Զ������غ��߼�����ѡ��
        if (CardsPlayedThisTurn >= maxCardsPerTurn)
        {
            EndPlayerTurn();
        }
    }
    //private IEnumerator EnemyTurn()
    //{
    //    yield return new WaitForSeconds(1f); // ģ�����˼��ʱ��
    //    StartPlayerTurn(); // �ص���һغ�
    //}

    public bool CanPlayCard()
    {
        return isPlayerTurn; // ֻҪ����һغϾͿ��Գ���
    }
}