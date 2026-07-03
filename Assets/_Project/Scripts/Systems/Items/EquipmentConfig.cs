using UnityEngine;

[CreateAssetMenu(fileName = "NewEquipment", menuName = "Aldenor/Items/Equipment")]
public class EquipmentConfig : ItemConfig
{
    [Header("Equipment Settings")]
    [SerializeField] private EquipmentType _slotType;
    [SerializeField] private int _armorBonus;
    [SerializeField] private int _damageBonus;

    public override ItemType Type => ItemType.Equipment;

    public EquipmentType SlotType => _slotType;
    public int ArmorBonus => _armorBonus;
    public int DamageBonus => _damageBonus;
}