using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    // UI 元素引用
    public Text turnInfoText;
    public Button endTurnButton;
    public static GameUIController Instance;
    public Text endTurnButtonText;
    public GameObject turnTransitionPanel;

    private int cardsRemainingThisTurn;
    private bool isTransitionPlaying = false; // 防止动画重复播放

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
    }

    private void Start()
    {
        cardsRemainingThisTurn = CardManager.Instance.CurrentHand.Count;
        TurnManager.Instance.onTurnStarted.AddListener(OnTurnStarted);
        TurnManager.Instance.onTurnEnded.AddListener(UpdateUI);

        endTurnButton.onClick.AddListener(() => {
            TurnManager.Instance.EndPlayerTurn();
            UpdateUI();
        });

        // 初始状态：面板隐藏且缩放为0
        if (turnTransitionPanel != null)
        {
            turnTransitionPanel.SetActive(false);
            turnTransitionPanel.transform.localScale = Vector3.zero;
        }

        UpdateUI();
    }

    public void OnCardUsed()
    {
        if (cardsRemainingThisTurn > 0)
        {
            cardsRemainingThisTurn--;
        }
        UpdateUI();
    }

    private void OnTurnStarted()
    {
        cardsRemainingThisTurn = CardManager.Instance.CurrentHand.Count;
        UpdateUI();
        ShowTurnTransition();
    }

    // 显示过渡面板（带缩放动画）
    private void ShowTurnTransition()
    {
        if (turnTransitionPanel == null || isTransitionPlaying)
            return;

        StartCoroutine(PlayTransitionAnimation());
    }

    // 协程：播放缩放动画
    private IEnumerator PlayTransitionAnimation()
    {
        isTransitionPlaying = true;
       
        turnTransitionPanel.SetActive(true);

        // 1. 快速放大（0 → 1）
        float duration = 0.2f; // 放大耗时
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(0f, 1f, timer / duration);
            turnTransitionPanel.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        turnTransitionPanel.transform.localScale = Vector3.one;

        // 2. 短暂停留（0.5秒）
        yield return new WaitForSeconds(0.5f);

        // 3. 快速缩小（1 → 0）
        timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 0f, timer / duration);
            turnTransitionPanel.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        turnTransitionPanel.transform.localScale = Vector3.zero;
        turnTransitionPanel.SetActive(false);

        isTransitionPlaying = false;
    }

    public void UpdateUI()
    {
        turnInfoText.text = $"回合: {TurnManager.Instance.CurrentTurn}\n" +
                         $"手牌: {CardManager.Instance.CurrentHand.Count}/7";

        endTurnButtonText.text = $"结束回合\n(当前回合:{TurnManager.Instance.CurrentTurn})\n" ;

        endTurnButton.image.color = (cardsRemainingThisTurn == 0)
            ? Color.green : Color.white;
    }
}