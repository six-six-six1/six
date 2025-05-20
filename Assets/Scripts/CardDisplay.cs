using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CardDisplay : MonoBehaviour
{
    public CardData cardData; // �����Ŀ�������
    //public TextMeshProUGUI statusText; // ��קʱ��ʾ״̬���ı�
    //public TextMeshProUGUI typeText;   // ��ʾ�������͵��ı�
    // �����ڳ�ʼ��ʱ����
    public void SetCardData(CardData data)
    {

        cardData = data;
        // ������Ը���UI��ʾ
        GetComponent<Image>().sprite = data.icon;
    }
    public void SetDraggingStatus(bool isDragging)
    {
        //statusText.text = isDragging ? "ʹ����..." : "";
        //statusText.color = isDragging ? Color.yellow : Color.white;

        //// Ҳ���Ե��������Ӿ�Ч��
        //if (isDragging)
        //{
        //    typeText.fontStyle = FontStyles.Bold;
        //}
        //else
        //{
        //    typeText.fontStyle = FontStyles.Normal;
        //}
    }
}
