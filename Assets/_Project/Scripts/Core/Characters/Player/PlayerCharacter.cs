using System;
using UnityEngine;

public class PlayerCharacter : Character
{
    public PlayerCharacter(string name, Vector2Int initialPosition, CharacterStats stats, int maxActionPoints, int maxBonusActions, int maxMovementPoints) 
        : base(name, initialPosition, stats, maxActionPoints, maxBonusActions, maxMovementPoints)
    {
        Equipment = new EquipmentManager();

        Equipment.OnEquipmentChanged += HandleEquipmentChanged;
    }

    public EquipmentManager Equipment { get; private set; }

    private void HandleEquipmentChanged(EquipmentType slot, EquipmentConfig oldItem, EquipmentConfig newItem)
    {
        int totalArmorBonus = 0;
        int totalDamageBonus = 0;

        foreach (EquipmentType equipSlot in Enum.GetValues(typeof(EquipmentType)))
        {
            var item = Equipment.GetItemInSlot(equipSlot);
            if (item != null)
            {
                totalArmorBonus += item.ArmorBonus;
                totalDamageBonus += item.DamageBonus;
            }
        }

        CharacterInventory.EquipmentWeight = Equipment.TotalWeight;
        Stats.UpdateEquipmentModifiers(totalArmorBonus, totalDamageBonus);
    }

    public void UseOrEquipItem(ItemConfig item)
    {
        switch (item.Type)
        {
            case ItemType.Consumable:
                TryConsumeItem(item);
                break;

            case ItemType.Equipment:
                var equipItem = (EquipmentConfig)item;

                CharacterInventory.RemoveItem(equipItem); 

                var oldItem = Equipment.EquipItem(equipItem);

                if (oldItem != null)
                {
                    CharacterInventory.TryAddItem(oldItem, 1);
                }
                break;
        }
    }

    public void Dispose()
    {
        if (Equipment != null)
            Equipment.OnEquipmentChanged -= HandleEquipmentChanged;
    }
}
