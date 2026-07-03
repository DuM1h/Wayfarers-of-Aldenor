using UnityEngine;

[CreateAssetMenu(fileName = "NewConsumable", menuName = "Aldenor/Items/Consumables")]
public class ConsumableConfig : ItemConfig
{
    [Header("Consumable Stats")]
    [SerializeField] private int _healAmount;

    public override ItemType Type => ItemType.Consumable;
    public int HealAmount => _healAmount;
}
