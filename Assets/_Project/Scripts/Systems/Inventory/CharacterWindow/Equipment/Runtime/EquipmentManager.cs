using System;
using System.Collections.Generic;

public class EquipmentManager
{
    private readonly Dictionary<EquipmentType, EquipmentConfig> _equippedItems;

    public event Action<EquipmentType, EquipmentConfig, EquipmentConfig> OnEquipmentChanged;

    public EquipmentManager()
    {
        _equippedItems = new Dictionary<EquipmentType, EquipmentConfig>();

        foreach (EquipmentType slot in Enum.GetValues(typeof(EquipmentType)))
        {
            _equippedItems[slot] = null;
        }
    }

    public EquipmentConfig EquipItem(EquipmentConfig newItem)
    {
        if (newItem == null) return null;

        EquipmentType type = newItem.SlotType;

        var oldItem = _equippedItems[type];

        _equippedItems[type] = newItem;
        OnEquipmentChanged?.Invoke(type, oldItem, newItem);
        return oldItem;
    }

    public EquipmentConfig UnequipItem(EquipmentType slot)
    {
        if (_equippedItems.ContainsKey(slot))
        {
            var removedItem = _equippedItems[slot];
            _equippedItems[slot] = null;
            OnEquipmentChanged?.Invoke(slot, removedItem, null);
            return removedItem;
        }
        else return null;
    }

    public EquipmentConfig GetItemInSlot(EquipmentType slot)
    {
        return _equippedItems[slot];
    }

    public float TotalWeight
    {
        get
        {
            float weight = 0f;
            foreach (var item in _equippedItems.Values)
            {
                if (item != null)
                {
                    weight += item.Weight;
                }
            }
            return weight;
        }
    }
}