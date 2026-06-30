using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private Dictionary<ItemConfig, int> _allItems = new Dictionary<ItemConfig, int>();

    public IReadOnlyDictionary<ItemConfig, int> AllItems => _allItems;

    public float CurrentWeight { get; private set; } = 0;
    public float MaxWeight { get; private set; }

    public event Action<ItemConfig, bool> OnInventoryChanged;

    public Inventory(float maxWeight)
    {
        MaxWeight = maxWeight;
    }

    public bool TryAddItem (ItemConfig item)
    {
        if (item == null) return false;
        if (CurrentWeight + item.weight > MaxWeight) return false;

        if (_allItems.ContainsKey(item) && item.isStackable)
            _allItems[item]++;
        else
            _allItems.Add(item, 1);

        CurrentWeight += item.weight;
        OnInventoryChanged?.Invoke(item, true);
        Debug.Log($"Предмет {item.name} додано");
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

        CurrentWeight = Mathf.Max(0, CurrentWeight - item.weight);
        OnInventoryChanged?.Invoke(item, false);
    }
}
