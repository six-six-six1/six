using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public int cardsPerTurn = 2; // ÿ�غϲ�������
    public int maxCardsPerTurn = 7; // ��Ϊ����ֵ����Ϊ�ֶ������غ�

    [Header("Audio Settings")]
    public AudioClip turnEndSound; // �غϽ�����Ч
    [Range(0, 1)] public float turnEndVolume = 0.8f;
    private AudioSource audioSource;

    // �����غ��¼�
    public UnityEvent onTurnStarted;
    public UnityEvent onTurnEnded;

    private int currentTurn;
    public bool isPlayerTurn;
    public int CurrentTurn { get; private set; } = 1;
    public int CardsPlayedThisTurn { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }

        // ��ʼ����Ƶ���
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
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

    public void StartPlayerTurn()
    {
        CurrentTurn++;
        CardsPlayedThisTurn = 0; // ���ñ��غϳ��Ƽ���
        isPlayerTurn = true;

        Debug.Log($"�غ� {CurrentTurn} ��ʼ");
        // �����غϿ�ʼ�¼�
        onTurnStarted?.Invoke();
        // ÿ�غϿ�ʼ���俨��
        CardManager.Instance.RefillHand();
    }

    public void EndPlayerTurn()
    {

        Debug.Log($"���ĹػغϽ���ǰ - CurrentTurn: {CurrentTurn}, isPlayerTurn: {isPlayerTurn}");
        isPlayerTurn = false;
        Debug.Log("�غϽ���");

        // ���ŻغϽ�����Ч
        PlaySound(turnEndSound, turnEndVolume);

        // �����غϽ����¼�
        onTurnEnded?.Invoke();
        // ֱ�ӿ�ʼ�»غϣ��Ƴ����˻غ��ӳ٣�
        StartPlayerTurn();
        Debug.Log($"���ĹػغϽ����� - CurrentTurn: {CurrentTurn}, isPlayerTurn: {isPlayerTurn}");
    }

    public void RegisterCardPlayed()
    {
        CardsPlayedThisTurn++;

        // �Զ������غ��߼�����ѡ��
        if (CardsPlayedThisTurn >= maxCardsPerTurn)
        {
            EndPlayerTurn();
        }
    }

    public bool CanPlayCard()
    {
        return isPlayerTurn; // ֻҪ����һغϾͿ��Գ���
    }
    // ��TurnManager.cs��������÷���
public void ResetTurnManager()
    {
        CurrentTurn = 1;
        CardsPlayedThisTurn = 0;
        isPlayerTurn = true;

        // ��������¼�����
        onTurnStarted.RemoveAllListeners();
        onTurnEnded.RemoveAllListeners();

        // ���³�ʼ��
        StartPlayerTurn();
    }
}