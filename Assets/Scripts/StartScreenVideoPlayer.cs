using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class StartScreenVideoPlayer : MonoBehaviour
{
    [Header("视频配置")]
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;
   
    [Header("UI元素")]
    public GameObject startButton; // Start按钮
   

    private RenderTexture videoTexture;

    void Start()
    {
        // 初始化视频播放器
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;

        // 设置视频完成回调
        videoPlayer.loopPointReached += OnVideoFinished;

        // 创建RenderTexture
        videoTexture = new RenderTexture((int)videoDisplay.rectTransform.rect.width,
                                        (int)videoDisplay.rectTransform.rect.height, 24);
        videoPlayer.targetTexture = videoTexture;
        videoDisplay.texture = videoTexture;

        // 默认隐藏视频显示
        videoDisplay.gameObject.SetActive(false);
     
    }

    // Start按钮点击事件
    public void OnStartButtonClicked()
    {
        // 隐藏Start按钮
        startButton.SetActive(false);

        // 显示视频
        videoDisplay.gameObject.SetActive(true);
     

        // 准备并播放视频
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        // 视频准备完成后开始播放
        videoPlayer.Play();
    
        videoPlayer.prepareCompleted -= OnVideoPrepared;
    }

    // 视频播放完成回调
    private void OnVideoFinished(VideoPlayer source)
    {
        LoadNextScene();
    }

    // 跳过按钮点击事件
    public void OnSkipButtonClicked()
    {
        videoPlayer.Stop();
        LoadNextScene();
    }

    // 加载下一场景
    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void OnDestroy()
    {
        // 清理资源
        if (videoTexture != null)
        {
            videoTexture.Release();
        }
    }
}