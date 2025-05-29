using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Hex Escape/Card Data")]
public class CardData : ScriptableObject
{
    public enum CardType { Move, EnergyStone, Replenish, Shock, Teleport }

    public CardType type;
    public Sprite icon;
    public string description;

    [Header("抽卡概率"), Tooltip("设置此卡牌的抽取概率百分比")]
    [Range(0.1f, 100)]
    public float drawProbability = 20f;

    [Header("Move Card")]
    public int moveDistance = 1;

    [Header("Shock Card")]
    public int maxClearDistance = 3;
}
