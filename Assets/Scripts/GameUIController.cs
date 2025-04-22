using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public Text turnInfoText;
    public Button endTurnButton;
    public static GameUIController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        endTurnButton.onClick.AddListener(OnEndTurnButtonClick);
        UpdateTurnInfo();
    }

    public void UpdateTurnInfo()
    {
        turnInfoText.text = $"Turn: {TurnManager.Instance.CurrentTurn} | " +
                          $"Cards Played: {TurnManager.Instance.CardsPlayedThisTurn}/3";
    }

    private void OnEndTurnButtonClick()
    {
        TurnManager.Instance.EndPlayerTurn();
        UpdateTurnInfo();
    }
}
