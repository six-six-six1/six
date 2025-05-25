using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUIManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handContainer;
    public static CardUIManager Instance;

    private GameObject draggedCard;
    private CardData draggedCardData;
    private Canvas canvas;
    private bool isSelectingMoveTarget;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        canvas = GetComponentInParent<Canvas>();
    }

    public void UpdateHandUI(List<CardData> hand)
    {
        Debug.Log($"更新手牌UI，手牌数量:{hand?.Count}");

        // 清空现有手牌
        foreach (Transform child in handContainer)
        {
            Destroy(child.gameObject);
        }

        if (hand == null || hand.Count == 0)
        {
            Debug.LogWarning("手牌数据为空！");
            return;
        }

        // 创建新卡牌UI
        foreach (var card in hand)
        {
            Debug.Log($"创建卡牌:{card?.type}");
            CreateCardUI(card);
        }
    }

    private void CreateCardUI(CardData card)
    {
        GameObject cardObj = Instantiate(cardPrefab, handContainer);
        cardObj.GetComponent<Image>().sprite = card.icon;

        // 设置卡牌数据
        var display = cardObj.GetComponent<CardDisplay>();
        if (display != null) display.cardData = card;
        {
            Debug.Log($"创建卡牌UI: {card.type}");
            display.SetCardData(card); // 确保调用
        }
        // 添加拖拽事件
        AddDragEvents(cardObj);
    }

    private void AddDragEvents(GameObject cardObj)
    {
        Debug.Log($"正在为 {cardObj.name} 添加拖拽事件"); // 检查是否执行
        EventTrigger trigger = cardObj.GetComponent<EventTrigger>() ?? cardObj.AddComponent<EventTrigger>();
        // 检查 EventTrigger 是否成功添加
        if (trigger == null)
        {
            Debug.LogError("EventTrigger 添加失败！");
            return;
        }
        // 开始拖拽
        EventTrigger.Entry beginDrag = new EventTrigger.Entry();
        beginDrag.eventID = EventTriggerType.BeginDrag;
        beginDrag.callback.AddListener((data) => OnBeginDrag(cardObj, (PointerEventData)data));
        trigger.triggers.Add(beginDrag);

        // 拖拽中
        EventTrigger.Entry drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener((data) => OnDrag((PointerEventData)data));
        trigger.triggers.Add(drag);

        // 结束拖拽
        EventTrigger.Entry endDrag = new EventTrigger.Entry();
        endDrag.eventID = EventTriggerType.EndDrag;
        endDrag.callback.AddListener((data) => OnEndDrag((PointerEventData)data));
        trigger.triggers.Add(endDrag);
    }

    private void OnBeginDrag(GameObject card, PointerEventData eventData)
    {
        // 获取卡牌显示组件
        var display = card.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.SetDraggingStatus(true); // 开始拖拽时更新状态
        }

        // 确保获取卡牌数据
        draggedCardData = display?.cardData; // 使用 null 条件运算符避免异常
        if (draggedCardData == null)
        {
            Debug.LogError("拖拽卡牌数据丢失！");
            return;
        }


        // 创建拖拽副本
        draggedCard = Instantiate(card, canvas.transform);
        draggedCard.transform.SetAsLastSibling();

        var img = draggedCard.GetComponent<Image>();
        img.raycastTarget = false;
        img.color = new Color(1, 1, 1, 0.7f);

        draggedCardData = card.GetComponent<CardDisplay>().cardData;
       
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (draggedCard != null)
        {
            draggedCard.transform.position = eventData.position;
        }
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        if (draggedCard == null) return;
        bool used = IsOverPlayArea(eventData);

        // 恢复原卡牌状态
        var originalCard = eventData.pointerDrag; // 获取被拖拽的原始卡牌
        if (originalCard != null)
        {
            var display = originalCard.GetComponent<CardDisplay>();
            if (display != null)
            {
                display.SetDraggingStatus(false); // 结束拖拽时恢复状态
            }
        }
        Destroy(draggedCard);
      
        if (used && draggedCardData != null)
        {
            // 立即消耗卡牌
            bool success = CardManager.Instance.PlayCard(draggedCardData);

            if (!success)
            {
                Debug.LogError("卡牌使用失败！");
                return;
            }

            // 根据卡牌类型进入目标选择模式
            switch (draggedCardData.type)
            {
                case CardData.CardType.Move:
                    StartMoveTargetSelection();
                    break;
                case CardData.CardType.Shock:
                    StartShockDirectionSelection();
                    break;
                default:
                    // 非目标选择类卡牌直接清除数据
                    draggedCardData = null;
                    break;
            }
        }
        else
        {
            // 未使用卡牌时清除数据
            draggedCardData = null;
        }
    }

    private void StartMoveTargetSelection()
    {
        isSelectingMoveTarget = true;
        HexGridSystem.Instance.ClearAllHighlights();

        if (HexGridSystem.Instance == null || PlayerController.Instance == null)
        {
            Debug.LogError("HexGridSystem或PlayerController实例未初始化！");
            return;
        }

        // 获取玩家当前位置
        Vector3Int playerHex = HexGridSystem.Instance.WorldToCell(PlayerController.Instance.transform.position);

        // 检查playerHex是否为有效坐标
        if (!HexGridSystem.Instance.IsHexValid(playerHex))
        {
            Debug.LogError("玩家位置无效！");
            return;
        }

        // 高亮相邻可行走格子
        var neighbors = HexGridSystem.Instance.GetWalkableNeighbors(playerHex);
        foreach (var pos in neighbors)
        {
            HexGridSystem.Instance.HighlightHex(pos, true);
        }

        // 注册点击事件
        TilemapClickHandler.OnHexClicked += HandleHexClick;
    }

    private void HandleHexClick(Vector3Int hexPosition)
    {
        Debug.Log($"点击高亮格子: {hexPosition}isSelectingMoveTarget{isSelectingMoveTarget}");
        if (!isSelectingMoveTarget)
        {
            Debug.LogWarning("当前未在选择移动目标状态！");
            return;
        }

        // 检查卡牌数据是否有效
        if (draggedCardData == null)
        {
            Debug.LogError("卡牌数据丢失，无法执行移动！");
            return;
        }

        Debug.Log("准备移动玩家...");
        // 移动玩家
        PlayerController.Instance.MoveToHex(hexPosition);
        // 实际消耗卡牌
        CardManager.Instance.PlayCard(draggedCardData);
        // 结束选择
        EndMoveTargetSelection();

        
    }

    public void EndMoveTargetSelection()
    {
        isSelectingMoveTarget = false;
        HexGridSystem.Instance.ClearAllHighlights();
        TilemapClickHandler.OnHexClicked -= HandleHexClick;
        Debug.Log("已取消高亮并解除点击事件绑定");

        draggedCardData = null;
        Debug.Log("移动目标选择结束，卡牌数据已清除");
    }

    private bool IsOverPlayArea(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            
            if (result.gameObject.CompareTag("PlayArea"))
            {

                return true;
            }
        }

        return false;
    }

    private void StartShockDirectionSelection()
    {
        HexGridSystem.Instance.ClearAllHighlights();
        Vector3Int playerPos = HexGridSystem.Instance.WorldToCell(PlayerController.Instance.transform.position);

        // 调用新方法获取方向数组
        Vector3Int[] directions = HexGridSystem.Instance.GetNeighborDirectionsForPosition(playerPos);

        // 高亮所有可能方向的首个格子
        foreach (var dir in directions)
        {
            Vector3Int neighborPos = playerPos + dir;
            // 仅检查格子是否存在Tile，不限制是否为DarkHexTile
            if (HexGridSystem.Instance.IsHexValid(neighborPos))
            {
                HexGridSystem.Instance.HighlightHex(neighborPos, true);
            }
        }
        TilemapClickHandler.OnHexClicked += HandleShockDirectionSelected;
    }

    private void HandleShockDirectionSelected(Vector3Int selectedPos)
    {
        // 空引用检查
        if (HexGridSystem.Instance == null)
        {
            Debug.LogError("HexGridSystem 实例未找到！");
            return;
        }
        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController 实例未找到！");
            return;
        }
        if (draggedCardData == null)
        {
            Debug.LogError("卡牌数据丢失！");
            return;
        }

        Vector3Int playerPos = HexGridSystem.Instance.WorldToCell(PlayerController.Instance.transform.position);
        Vector3Int direction = selectedPos - playerPos;

        // 获取该方向上最多3格
        List<Vector3Int> tilesToClear = HexGridSystem.Instance.GetTilesInDirection(
            playerPos, direction, draggedCardData.maxClearDistance
        );

        // 清除DarkHexTile
        foreach (var pos in tilesToClear)
        {
            if (HexGridSystem.Instance.IsDarkHexTile(pos))
            {
                Debug.Log($"尝试清除DarkHexTile：{pos}");
                DarkTileSystem.Instance.ClearDarkTile(pos);
            }
        }

        EndShockSelection();
    }

    private void EndShockSelection()
    {
        HexGridSystem.Instance.ClearAllHighlights();
        TilemapClickHandler.OnHexClicked -= HandleShockDirectionSelected;

        // 操作完成后清除卡牌数据
        draggedCardData = null;
        Debug.Log("Shock方向选择结束，卡牌数据已清除");
    }

}