using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private Dictionary<ItemConfig, int> _allItems = new Dictionary<ItemConfig, int>();

    public IReadOnlyDictionary<ItemConfig, int> AllItems => _allItems;

    public float CurrentWeight { get; private set; } = 0;
    public float MaxWeight { get; private set; }

    public float EquipmentWeight { get; set; }

    public float TotalWeight => CurrentWeight + EquipmentWeight;

    public event Action<ItemConfig, bool> OnInventoryChanged;

    public Inventory(float maxWeight)
    {
        MaxWeight = maxWeight;
    }

    public bool TryAddItem (ItemConfig item, int amount)
    {
        if (item == null) return false;
        if (TotalWeight + item.Weight > MaxWeight) return false;

        if (amount == 1)
        {
            if (_allItems.ContainsKey(item))
                _allItems[item]++;
            else
                _allItems.Add(item, 1);
            CurrentWeight += item.Weight;
        }
        else if (TotalWeight + item.Weight * amount <= MaxWeight)
        {
            if (_allItems.ContainsKey(item))
                _allItems[item] += amount;
            else
                _allItems.Add(item, amount);
            CurrentWeight += item.Weight * amount;
        }
        else
        {
            int leftAmount = amount;
            do
            {
                if (_allItems.ContainsKey(item))
                    _allItems[item]++;
                else
                    _allItems.Add(item, 1);
                leftAmount--;
                CurrentWeight += item.Weight;
            } while (TotalWeight + item.Weight <= MaxWeight && leftAmount > 0);
        }
        OnInventoryChanged?.Invoke(item, true);
        Debug.Log($"Предмет {item.ItemName} додано");
        return true;
    }

    public void RemoveItem (ItemConfig item)
    {
        if (item == null) return;

        if (!_allItems.ContainsKey(item)) return;

        if (_allItems[item] > 1)
            _allItems[item]--;
        else
            _allItems.Remove(item);

        CurrentWeight = Mathf.Max(0, CurrentWeight - item.Weight);
        OnInventoryChanged?.Invoke(item, false);
    }
}
