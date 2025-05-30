using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("�ؿ��б�")]
    public List<LevelData> allLevels = new List<LevelData>();

    [Header("��ǰ�ؿ�")]
    public LevelData currentLevelData;       // ��ǰ���еĹؿ�����

    private void Awake()
    {
        // ����ģʽ��ʼ��
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLevels();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ��ʼ���ؿ�����״̬
    private void InitializeLevels()
    {
        // Ĭ�Ͻ�����һ��
        if (allLevels.Count > 0)
        {
            allLevels[0].isUnlocked = true;
            allLevels[0].SaveUnlockState();
        }

        // ���������ؿ��Ľ���״̬
        foreach (var level in allLevels)
        {
            level.LoadUnlockState();
        }
    }

    /// <summary>
    /// ����ָ���ؿ�
    /// </summary>
    public void LoadLevel(int targetLevelID)
    {
        LevelData targetLevel = allLevels.Find(x => x.levelID == targetLevelID);
        if (targetLevel == null)
        {
            Debug.LogError($"�Ҳ���IDΪ{targetLevelID}�Ĺؿ���");
            return;
        }

        currentLevelData = targetLevel;
        SceneManager.LoadScene(targetLevel.sceneName);

        // ȷ�����ƹ�����Ӧ���¹ؿ�����
        if (CardManager.Instance != null)
        {
            CardManager.Instance.ApplyLevelSettings(targetLevel);
        }
    }

    /// <summary>
    /// ������������һ��
    /// </summary>
    public void UnlockAndLoadNextLevel()
    {
        if (currentLevelData == null) return;

        int nextLevelID = currentLevelData.levelID + 1;
        LevelData nextLevel = allLevels.Find(x => x.levelID == nextLevelID);

        if (nextLevel != null)
        {
            nextLevel.isUnlocked = true;
            nextLevel.SaveUnlockState();
            LoadLevel(nextLevelID);
        }
        else
        {
            Debug.Log("�Ѿ������һ�أ��������˵�");
            SceneManager.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// ��ȡ��һ��δ�����Ĺؿ�
    /// </summary>
    public LevelData GetNextUnlockedLevel()
    {
        return allLevels.Find(x => !x.isUnlocked);
    }

    public bool HasNextLevel()
    {
        if (currentLevelData == null || allLevels == null || allLevels.Count == 0)
            return false;

        return currentLevelData.levelID < allLevels.Count;
    }
    // �� LevelManager.cs �����
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentLevelData != null && CardManager.Instance != null)
        {
            CardManager.Instance.ApplyLevelSettings(currentLevelData);
        }
    }
}
