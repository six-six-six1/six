using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CardUIManager : MonoBehaviour
{
    // 配置参数
    [Header("必需引用")]
    public GameObject cardUIPrefab;
    public Transform handPanel;

    [Header("拖拽设置")]
    public float dragAlpha = 0.7f;
    public Vector2 dragOffset = new Vector2(0, 20f); // 拖拽时鼠标与卡牌的偏移

    // 运行时状态
    private List<GameObject> cardUIInstances = new List<GameObject>();
    private GameObject draggedCard;
    private CardData currentDraggedCard;
    private Canvas rootCanvas;
    private GraphicRaycaster raycaster;  // 新增声明
    private void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        if (handPanel == null) handPanel = transform;
        // 初始化raycaster
        raycaster = GetComponentInParent<GraphicRaycaster>();
        if (raycaster == null)
        {
            raycaster = FindObjectOfType<GraphicRaycaster>();
            if (raycaster == null)
            {
                Debug.LogError("场景中未找到GraphicRaycaster组件！");
            }
        }
    }


    public void UpdateHandUI(List<CardData> hand)
    {
        ClearHandUI();
        foreach (var card in hand)
        {
            var cardUI = Instantiate(cardUIPrefab, handPanel);
            SetupCardUI(cardUI, card);
            cardUIInstances.Add(cardUI);
        }
    }

    private void SetupCardUI(GameObject cardUI, CardData card)
    {
        // 基础设置
        var img = cardUI.GetComponent<Image>();
        img.sprite = card.icon;
        img.preserveAspect = true;

        var text = cardUI.GetComponentInChildren<Text>();
        if (text != null) text.text = card.type.ToString();

        // 事件设置
        var trigger = cardUI.GetComponent<EventTrigger>() ?? cardUI.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        AddTriggerEvent(trigger, EventTriggerType.BeginDrag, (data) => StartDrag(card, data));
        AddTriggerEvent(trigger, EventTriggerType.Drag, (data) => DuringDrag(data));
        AddTriggerEvent(trigger, EventTriggerType.EndDrag, (data) => EndDrag());
    }

    private void AddTriggerEvent(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    private void StartDrag(CardData card, BaseEventData data)
    {
        // 1. 检查必要引用
        Debug.Log("开始拖拽事件触发"); // 确认事件起点
        if (cardUIPrefab == null)
        {
            Debug.LogError("cardUIPrefab 未赋值！");
            return;
        }

        if (rootCanvas == null)
        {
            rootCanvas = GetComponentInParent<Canvas>();
            if (rootCanvas == null)
            {
                Debug.LogError("未找到Canvas！");
                return;
            }
        }

        // 2. 实例化卡牌
        draggedCard = Instantiate(cardUIPrefab, rootCanvas.transform);
        if (draggedCard == null)
        {
            Debug.LogError("卡牌实例化失败！");
            return;
        }

        // 3. 设置卡牌属性
        draggedCard.transform.SetAsLastSibling();

        Image img = draggedCard.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("卡牌预制体缺少Image组件！");
            Destroy(draggedCard);
            return;
        }

        img.sprite = card?.icon; // 安全访问card
        img.raycastTarget = false;
        img.color = new Color(1, 1, 1, dragAlpha);

        // 4. 设置位置
        var pointerData = data as PointerEventData;
        if (pointerData != null)
        {
            Vector3 mousePos = new Vector3(pointerData.position.x, pointerData.position.y, 0);
            Vector3 offset = new Vector3(dragOffset.x, dragOffset.y, 0);
            draggedCard.transform.position = mousePos + offset;
        }

        currentDraggedCard = card;
    }

    private void DuringDrag(BaseEventData data)
    {
        if (draggedCard == null) return;

        var pointerData = (PointerEventData)data;
        Vector3 mousePosition = new Vector3(pointerData.position.x, pointerData.position.y, 0);
        Vector3 offset = new Vector3(dragOffset.x, dragOffset.y, 0);
        draggedCard.transform.position = mousePosition + offset;
    }

    private void EndDrag()
    {
        Debug.Log("结束拖拽事件触发");
        if (draggedCard == null) return;

        bool used = CheckIfUsedOnPlayArea();
        Destroy(draggedCard);

        if (used && currentDraggedCard != null)
        {
            CardManager.Instance.PlayCard(currentDraggedCard);
        }
        currentDraggedCard = null;
    }

    private bool CheckIfUsedOnPlayArea()
    {

        if (EventSystem.current == null || raycaster == null)
        {
            Debug.LogError("事件系统缺失！");
            return false;
        }

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        // 调试所有被射线检测到的对象
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        Debug.Log($"检测到{results.Count}个对象：");
        foreach (var result in results)
        {
            Debug.Log($"{result.gameObject.name} (Tag: {result.gameObject.tag})");

            if (result.gameObject.CompareTag("PlayArea"))
            {
                Debug.Log("有效释放区域");
                return true;
            }
        }

        Debug.Log("未找到有效释放区域");
        return false;
    }

    private void ClearHandUI()
    {
        foreach (var card in cardUIInstances)
        {
            if (card != null) Destroy(card);
        }
        cardUIInstances.Clear();
    }
}
