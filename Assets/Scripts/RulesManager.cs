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
        // �󶨰�ť�¼�
        rulesButton.onClick.AddListener(ShowRules);
        closeButton.onClick.AddListener(HideRules);

        // ��ʼ���ع������
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
        // �Ƴ��¼�����
        rulesButton.onClick.RemoveListener(ShowRules);
        closeButton.onClick.RemoveListener(HideRules);
    }
}
