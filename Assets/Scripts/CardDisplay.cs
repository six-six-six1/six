using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// �����Ҫʹ��TextMeshProUGUI��ȡ������ע��
// using TMPro;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData; // �����Ŀ�������

    // �����Ҫʹ��״̬�ı���ȡ������ע��
    // public TextMeshProUGUI statusText; // ��קʱ��ʾ״̬���ı�

    // �����Ҫʹ�������ı���ȡ������ע��
    // public TextMeshProUGUI typeText;   // ��ʾ�������͵��ı�

    // �����ڳ�ʼ��ʱ����
    public void SetCardData(CardData data)
    {
        Debug.Log($"���ÿ�������: {data?.type}"); // ��������Ƿ���Ч
        cardData = data;
        // ������Ը���UI��ʾ
        GetComponent<Image>().sprite = data.icon;
    }

    public void SetDraggingStatus(bool isDragging)
    {
        // �����Ҫ��ק״̬��ʾ��ȡ������ע��
        // statusText.text = isDragging ? "ʹ����..." : "";
        // statusText.color = isDragging ? Color.yellow : Color.white;

        // Ҳ���Ե��������Ӿ�Ч��
        // if (isDragging)
        // {
        //     typeText.fontStyle = FontStyles.Bold;
        // }
        // else
        // {
        //     typeText.fontStyle = FontStyles.Normal;
        // }
    }
}
