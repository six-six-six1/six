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
        // 实现单例模式
        if (Instance == null)
        {
            Instance = this;
            // 不需要DontDestroyOnLoad，因为我们只在第五关使用
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 确保面板初始时是隐藏的
        endGamePanel.SetActive(false);

        // 绑定按钮事件
        returnToMenuButton.onClick.AddListener(ReturnToMenu);
        quitGameButton.onClick.AddListener(QuitGame);
    }

    // 显示通关界面
    public void ShowEndGameScreen()
    {
        endGamePanel.SetActive(true);
        Time.timeScale = 0; // 暂停游戏逻辑
    }

    // 返回主菜单
    private void ReturnToMenu()
    {
        Time.timeScale = 1; // 恢复游戏时间
        SceneManager.LoadScene(0); // 加载开始场景
    }

    // 退出游戏
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
        // 移除事件监听
        if (returnToMenuButton != null)
            returnToMenuButton.onClick.RemoveListener(ReturnToMenu);

        if (quitGameButton != null)
            quitGameButton.onClick.RemoveListener(QuitGame);
    }
}
