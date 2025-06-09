using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject endGamePanel;
    public Button returnToMenuButton;
    public Button quitGameButton;

    public static EndGameManager Instance { get; private set; }

    private void Awake()
    {
        // ʵ�ֵ���ģʽ
        if (Instance == null)
        {
            Instance = this;
            // ����ҪDontDestroyOnLoad����Ϊ����ֻ�ڵ����ʹ��
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // ȷ������ʼʱ�����ص�
        endGamePanel.SetActive(false);

        // �󶨰�ť�¼�
        returnToMenuButton.onClick.AddListener(ReturnToMenu);
        quitGameButton.onClick.AddListener(QuitGame);
    }

    // ��ʾͨ�ؽ���
    public void ShowEndGameScreen()
    {
        endGamePanel.SetActive(true);
        Time.timeScale = 0; // ��ͣ��Ϸ�߼�
    }

    // �������˵�
    private void ReturnToMenu()
    {
        Time.timeScale = 1; // �ָ���Ϸʱ��
        SceneManager.LoadScene(0); // ���ؿ�ʼ����
    }

    // �˳���Ϸ
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        // �Ƴ��¼�����
        if (returnToMenuButton != null)
            returnToMenuButton.onClick.RemoveListener(ReturnToMenu);

        if (quitGameButton != null)
            quitGameButton.onClick.RemoveListener(QuitGame);
    }
}
