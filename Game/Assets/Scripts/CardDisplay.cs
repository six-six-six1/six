using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CardDisplay : MonoBehaviour
{
    public CardData cardData; // 关联的卡牌数据
    //public TextMeshProUGUI statusText; // 拖拽时显示状态的文本
    //public TextMeshProUGUI typeText;   // 显示卡牌类型的文本
    // 可以在初始化时设置
    public void SetCardData(CardData data)
    {
        Debug.Log($"设置卡牌数据: {data?.type}"); // 检查数据是否有效
        cardData = data;
        // 这里可以更新UI显示
        GetComponent<Image>().sprite = data.icon;
    }
    public void SetDraggingStatus(bool isDragging)
    {
        //statusText.text = isDragging ? "使用中..." : "";
        //statusText.color = isDragging ? Color.yellow : Color.white;

        //// 也可以调整其他视觉效果
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
