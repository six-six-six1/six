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
        }
        else
        {
            Destroy(gameObject);
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

        // ͬ�����õ����ƹ�����
        if (CardManager.Instance != null)
        {
            CardManager.Instance.ApplyLevelSettings(targetLevel);
        }
    }

    /// <summary>
    /// ��ȡ��һ��δ�����Ĺؿ�
    /// </summary>
    public LevelData GetNextUnlockedLevel()
    {
        // ʵ����Ŀ�пɽ��PlayerPrefs�洢����״̬
        return allLevels.Find(x => !x.isUnlocked);
    }
}
