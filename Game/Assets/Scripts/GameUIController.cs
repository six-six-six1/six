using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class GameUIController : MonoBehaviour
{
    // UI Ԫ������
    public Text turnInfoText;          // ��ʾ�غ���Ϣ���ı����
    public Button endTurnButton;       // �����غϰ�ť
    public static GameUIController Instance;  // ����ʵ��
    public Text endTurnButtonText; // ������ť�ı�����
    // Awake �ڶ����ʼ��ʱ����
    private void Awake()
    {
        // ����ģʽʵ��
        if (Instance == null)
        {
            Instance = this;  // ���������ʵ�������õ�ǰ����Ϊʵ��
        }
        else
        {
            Destroy(gameObject);  // ����Ѵ���ʵ���������ظ�����
        }
    }

    private void Start()
    {
        // ��ʼ������
        TurnManager.Instance.onTurnStarted.AddListener(UpdateUI);
        TurnManager.Instance.onTurnEnded.AddListener(UpdateUI);

        endTurnButton.onClick.AddListener(() => {
            TurnManager.Instance.EndPlayerTurn();
            UpdateUI(); // ��������
        });

        UpdateUI(); // ��ʼ����
    }

    public void UpdateTurnInfo()
    {
        turnInfoText.text = $"�غ�: {TurnManager.Instance.CurrentTurn}\n" +
                         $"����: {CardManager.Instance.CurrentHand.Count}";
    }
    private void OnEndTurnButtonClick()
    {
        if (CardManager.Instance.currentHand.Count > 0)
        {
            TurnManager.Instance.EndPlayerTurn();
        }
        else
        {
            Debug.Log("�����������п��Ʋ��ܽ����غϣ�");
        }
        // ���»غ���Ϣ��ʾ
        UpdateTurnInfo();
    }
    // �ڿ�����ק�ͷ�ʱ����
    void OnCardDropped(CardData card)
    {
        if (CardManager.Instance.PlayCard(card))
        {
            // ����ʹ�óɹ������Ч
            //Destroy(card.gameObject);
        }
    }
    public void UpdateUI()
    {
        // ���»غ���Ϣ
        turnInfoText.text = $"�غ�: {TurnManager.Instance.CurrentTurn}\n" +
                         $"����: {CardManager.Instance.CurrentHand.Count}/7";

        // ���°�ť�ı�
        endTurnButtonText.text = $"�����غ�\n(��ǰ�غ�:{TurnManager.Instance.CurrentTurn})";

        // ����״̬������ť��ɫ
        endTurnButton.image.color = CardManager.Instance.CurrentHand.Count == 0 ?
            Color.green : // ����Ϊ��ʱ��ɫ
            Color.white;  // ����״̬��ɫ
    }
}
