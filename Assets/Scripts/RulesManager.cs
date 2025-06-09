using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulesManager : MonoBehaviour
{
    [SerializeField] private Button rulesButton;
    [SerializeField] private GameObject rulesPanel;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        // 绑定按钮事件
        rulesButton.onClick.AddListener(ShowRules);
        closeButton.onClick.AddListener(HideRules);

        // 初始隐藏规则面板
        rulesPanel.SetActive(false);
    }

    private void ShowRules()
    {
        rulesPanel.SetActive(true);
    }

    private void HideRules()
    {
        rulesPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // 移除事件监听
        rulesButton.onClick.RemoveListener(ShowRules);
        closeButton.onClick.RemoveListener(HideRules);
    }
}
