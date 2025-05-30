using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("��ť����")]
    public Button startButton;  // ��ʼ/��һ�ذ�ť
    public Button retryButton;  // ���Ե�ǰ�ذ�ť
    public Button menuButton;   // ���ز˵���ť

    [Header("��������")]
    [Tooltip("�˵�������buildIndex")]
    public int menuSceneIndex = 0;

    private void Start()
    {
        // �󶨰�ť�¼�
        if (startButton != null)
        {
            startButton.onClick.AddListener(LoadNextLevel);
        }
        else
        {
            Debug.LogWarning("δ����Start��ť");
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(ReloadCurrentLevel);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(ReturnToMenu);
        }
    }

    /// <summary>
    /// ������һ�أ���ǰ����buildIndex + 1��
    /// </summary>
    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // ����Ƿ񳬳���������
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("�Ѿ������һ�أ����ز˵�");
            SceneManager.LoadScene(menuSceneIndex);
        }
    }

    /// <summary>
    /// ���¼��ص�ǰ�ؿ�
    /// </summary>
    public void ReloadCurrentLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    /// <summary>
    /// �������˵�
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneIndex);
    }

    /// <summary>
    /// ����ָ�������ĳ���
    /// </summary>
    public void LoadSpecificLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogError($"��Ч�ĳ�������: {levelIndex}");
        }
    }
}