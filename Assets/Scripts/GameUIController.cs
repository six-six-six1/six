using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class GameUIController : MonoBehaviour
{
    // UI 元素引用
    public Text turnInfoText;          // 显示回合信息的文本组件
    public Button endTurnButton;       // 结束回合按钮
    public static GameUIController Instance;  // 单例实例
    public Text endTurnButtonText; // 新增按钮文本引用
    // Awake 在对象初始化时调用
    private void Awake()
    {
        // 单例模式实现
        if (Instance == null)
        {
            Instance = this;  // 如果不存在实例，设置当前对象为实例
        }
        else
        {
            Destroy(gameObject);  // 如果已存在实例，销毁重复对象
        }
    }

    private void Start()
    {
        // 初始化监听
        TurnManager.Instance.onTurnStarted.AddListener(UpdateUI);
        TurnManager.Instance.onTurnEnded.AddListener(UpdateUI);

        endTurnButton.onClick.AddListener(() => {
            TurnManager.Instance.EndPlayerTurn();
            UpdateUI(); // 立即更新
        });

        UpdateUI(); // 初始更新
    }

    public void UpdateTurnInfo()
    {
        turnInfoText.text = $"回合: {TurnManager.Instance.CurrentTurn}\n" +
                         $"手牌: {CardManager.Instance.CurrentHand.Count}";
    }
    private void OnEndTurnButtonClick()
    {
        if (CardManager.Instance.currentHand.Count > 0)
        {
            TurnManager.Instance.EndPlayerTurn();
        }
        else
        {
            Debug.Log("必须用完所有卡牌才能结束回合！");
        }
        // 更新回合信息显示
        UpdateTurnInfo();
    }
    // 在卡牌拖拽释放时调用
    void OnCardDropped(CardData card)
    {
        if (CardManager.Instance.PlayCard(card))
        {
            // 卡牌使用成功后的特效
            //Destroy(card.gameObject);
        }
    }
    public void UpdateUI()
    {
        // 更新回合信息
        turnInfoText.text = $"回合: {TurnManager.Instance.CurrentTurn}\n" +
                         $"手牌: {CardManager.Instance.CurrentHand.Count}/7";

        // 更新按钮文本
        endTurnButtonText.text = $"结束回合\n(当前回合:{TurnManager.Instance.CurrentTurn})";

        // 根据状态调整按钮颜色
        endTurnButton.image.color = CardManager.Instance.CurrentHand.Count == 0 ?
            Color.green : // 手牌为空时绿色
            Color.white;  // 正常状态白色
    }
}
