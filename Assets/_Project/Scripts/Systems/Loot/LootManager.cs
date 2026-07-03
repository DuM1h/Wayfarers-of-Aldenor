using System;
using System.Collections.Generic;
using UnityEngine;

public class LootManager
{
    private Dictionary<Vector2Int, GroundLoot> _worldLoot = new Dictionary<Vector2Int, GroundLoot>();

    public event Action<Vector2Int> OnLootRemoved;
    public event Action<GroundLoot> OnLootSpawned;

    public void SpawnLoot(Vector2Int coords, ItemConfig item, int amount)
    {
        if (_worldLoot.ContainsKey(coords)) return;

        GroundLoot loot = new GroundLoot(coords, item, amount);
        _worldLoot.Add(coords, loot);

        OnLootSpawned?.Invoke(loot);
    }

    public bool TryPickupLoot(Vector2Int coords, Inventory playerInventory)
    {
        if (!_worldLoot.ContainsKey(coords)) return false;

        GroundLoot loot = _worldLoot[coords];
        
        if (!playerInventory.TryAddItem(loot.Item, loot.Amount))
            return false;

        _worldLoot.Remove(coords);
        OnLootRemoved?.Invoke(coords);

        return true;
    }
}