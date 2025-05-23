// HexTile.using UnityEngine;

public class HexTile : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    public Vector2Int Coordinates { get; private set; }

    public void Init(Vector2Int coord)
    {
        Coordinates = coord;
        highlight.SetActive(false);
    }

    public void SetHighlight(bool state)
    {
        highlight.SetActive(state);
    }

    private void OnMouseEnter()
    {
        if (CardDragHandler.IsDraggingMoveCard)
            SetHighlight(true);
    }

    private void OnMouseExit()
    {
        SetHighlight(false);
    }
}