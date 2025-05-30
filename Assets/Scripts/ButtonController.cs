using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [Header("按钮设置")]
    public Button startButton;  // 开始/下一关按钮
    public Button retryButton;  // 重试当前关按钮
    public Button menuButton;   // 返回菜单按钮

    [Header("场景设置")]
    [Tooltip("菜单场景的buildIndex")]
    public int menuSceneIndex = 0;

    private void Start()
    {
        // 绑定按钮事件
        if (startButton != null)
        {
            startButton.onClick.AddListener(LoadNextLevel);
        }
        else
        {
            Debug.LogWarning("未分配Start按钮");
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
    /// 加载下一关（当前场景buildIndex + 1）
    /// </summary>
    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // 检查是否超出场景总数
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("已经是最后一关，返回菜单");
            SceneManager.LoadScene(menuSceneIndex);
        }
    }

    /// <summary>
    /// 重新加载当前关卡
    /// </summary>
    public void ReloadCurrentLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(menuSceneIndex);
    }

    /// <summary>
    /// 加载指定索引的场景
    /// </summary>
    public void LoadSpecificLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogError($"无效的场景索引: {levelIndex}");
        }
    }
}