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
        Debug.Log($"��������UI����������:{hand?.Count}");

        // �����������
        foreach (Transform child in handContainer)
        {
            Destroy(child.gameObject);
        }

        if (hand == null || hand.Count == 0)
        {
            Debug.LogWarning("��������Ϊ�գ�");
            return;
        }

        // �����¿���UI
        foreach (var card in hand)
        {
            Debug.Log($"��������:{card?.type}");
            CreateCardUI(card);
        }
    }

    private void CreateCardUI(CardData card)
    {
        GameObject cardObj = Instantiate(cardPrefab, handContainer);
        cardObj.GetComponent<Image>().sprite = card.icon;

        // ���ÿ�������
        var display = cardObj.GetComponent<CardDisplay>();
        if (display != null) display.cardData = card;

        // �����ק�¼�
        AddDragEvents(cardObj);
    }

    private void AddDragEvents(GameObject cardObj)
    {
        EventTrigger trigger = cardObj.GetComponent<EventTrigger>() ?? cardObj.AddComponent<EventTrigger>();

        // ��ʼ��ק
        EventTrigger.Entry beginDrag = new EventTrigger.Entry();
        beginDrag.eventID = EventTriggerType.BeginDrag;
        beginDrag.callback.AddListener((data) => OnBeginDrag(cardObj, (PointerEventData)data));
        trigger.triggers.Add(beginDrag);

        // ��ק��
        EventTrigger.Entry drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener((data) => OnDrag((PointerEventData)data));
        trigger.triggers.Add(drag);

        // ������ק
        EventTrigger.Entry endDrag = new EventTrigger.Entry();
        endDrag.eventID = EventTriggerType.EndDrag;
        endDrag.callback.AddListener((data) => OnEndDrag((PointerEventData)data));
        trigger.triggers.Add(endDrag);
    }

    private void OnBeginDrag(GameObject card, PointerEventData eventData)
    {
        // ��ȡ������ʾ���
        var display = card.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.SetDraggingStatus(true); // ��ʼ��קʱ����״̬
        }

        // ������ק����
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
        // �ָ�ԭ����״̬
        var originalCard = eventData.pointerDrag; // ��ȡ����ק��ԭʼ����
        if (originalCard != null)
        {
            var display = originalCard.GetComponent<CardDisplay>();
            if (display != null)
            {
                display.SetDraggingStatus(false); // ������קʱ�ָ�״̬
            }
        }
        Destroy(draggedCard);

        if (used && draggedCardData != null)
        {
            if (draggedCardData.type == CardData.CardType.Move)
            {
                StartMoveTargetSelection();
            }
            else
            {
                CardManager.Instance.PlayCard(draggedCardData);
            }
        }

        draggedCard = null;
        draggedCardData = null;
    }

    private void StartMoveTargetSelection()
    {
        isSelectingMoveTarget = true;
        HexGridSystem.Instance.ClearAllHighlights();

        if (HexGridSystem.Instance == null)
        {
            Debug.LogError("HexGridSystem ʵ��δ�ҵ���");
            return;
        }

        // ��ȡ��ҵ�ǰλ��
        Vector3Int playerHex = HexGridSystem.Instance.WorldToCell(PlayerController.Instance.transform.position);

        // �������ڿ����߸���
        var neighbors = HexGridSystem.Instance.GetWalkableNeighbors(playerHex);
        foreach (var pos in neighbors)
        {
            HexGridSystem.Instance.HighlightHex(pos, true);
        }

        // ע�����¼�
        TilemapClickHandler.OnHexClicked += HandleHexClick;
    }

    private void HandleHexClick(Vector3Int hexPosition)
    {
        if (!isSelectingMoveTarget) return;

        // �ƶ����
        PlayerController.Instance.MoveToHex(hexPosition);

        // ����ѡ��
        EndMoveTargetSelection();

        // ʵ�����Ŀ���
        CardManager.Instance.PlayCard(draggedCardData);
    }

    private void EndMoveTargetSelection()
    {
        isSelectingMoveTarget = false;
        HexGridSystem.Instance.ClearAllHighlights();
        TilemapClickHandler.OnHexClicked -= HandleHexClick;
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
}