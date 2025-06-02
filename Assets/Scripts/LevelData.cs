using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewLevel", menuName = "�ؿ�/�ؿ�����")]
public class LevelData : ScriptableObject
{
    [Header("������Ϣ")]
    public int levelID = 1;                  // Ψһ�ؿ���ţ���101=��1�µ�1�أ�
    public string levelName = "Ĭ�Ϲؿ�";     // �ؿ���ʾ����
    [TextArea] public string description;    // �ؿ�����
    public Sprite thumbnail;                 // �ؿ�ѡ����������ͼ

    [Header("��������")]
    public string sceneName;                 // �����ĳ�������
    // ��ʹ��SceneAsset����UnityEditor�����ռ䣩
    // public SceneAsset sceneAsset;         // ����ȫ�ĳ������÷�ʽ

    [Header("��������")]
    public int initialHandSize = 3;          // ���ؿ���ʼ������
    public int maxHandSize = 7;              // ���ؿ���������
    public int cardsPerTurn = 2;             // ���ؿ�ÿ�غϳ�����

    [Header("��������")]
    public bool limitDarkTileExpansion = true; // �Ƿ����ƺ���Ⱦ
    public int maxExpansionPerTurn = 2;       // ÿ�غ����Ⱦ����


    [Header("�ؿ�״̬")]
    public bool isUnlocked;                  // �Ƿ��ѽ���

    // �洢����״̬��PlayerPrefs
    public void SaveUnlockState()
    {
        PlayerPrefs.SetInt($"Level_{levelID}_Unlocked", isUnlocked ? 1 : 0);
    }

    // ��PlayerPrefs���ؽ���״̬
    public void LoadUnlockState()
    {
        isUnlocked = PlayerPrefs.GetInt($"Level_{levelID}_Unlocked", 0) == 1;
    }
}
