using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Aldenor/Inventory/ItemConfig")]
public class ItemConfig : ScriptableObject
{
    public string itemName;
    public string description;
    public float weight;
    public Sprite sprite;
    public ItemType type;
    public bool isStackable => type != ItemType.Equipment;

    [Header("Consumable Stats")]
    public int healAmount;
}

public enum ItemType
{
    Consumable,
    Equipment,
    Material,
    Treasure,
    Quest
}