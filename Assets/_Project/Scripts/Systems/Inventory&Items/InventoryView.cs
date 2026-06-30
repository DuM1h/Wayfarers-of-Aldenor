using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private Transform _gridContainer;
    [SerializeField] private InventorySlot _slotPrefab;
    private Canvas _canvas;

    private Inventory _logicalInventory;
    private List<InventorySlot> _spawnedSlots = new List<InventorySlot>();

    public event Action<bool> OnInventoryToggled;
    public event Action<ItemConfig> OnItemClicked;

    public void Initialize(Inventory logicalInventory)
    {
        _logicalInventory = logicalInventory;
        _logicalInventory.OnInventoryChanged += HandleInventoryChanged;

        _canvas = GetComponent<Canvas>();

        RefreshView();
    }

    private void OnDestroy()
    {
        if (_logicalInventory != null)
        {
            _logicalInventory.OnInventoryChanged -= HandleInventoryChanged;
        }
    }

    private void HandleInventoryChanged(ItemConfig item, bool wasAdded)
    {
        // Для MVP можна просто повністю перемальовувати інвентар при будь-якій зміні
        RefreshView();
    }

    private void RefreshView()
    {
        if (_logicalInventory == null) return;

        foreach (var slot in _spawnedSlots)
        {
            slot.OnSlotClicked -= OnItemClicked;
            Destroy(slot.gameObject);
        }
        _spawnedSlots.Clear();

        foreach (var slot in _logicalInventory.AllItems)
        {
            ItemConfig item = slot.Key;
            int amount = slot.Value;

            InventorySlot newSlot = Instantiate(_slotPrefab, _gridContainer);
            newSlot.OnSlotClicked += OnItemClicked;

            newSlot.Setup(item, amount);

            _spawnedSlots.Add(newSlot);
        }
    }

    public void ToggleInventory()
    {
        bool isActive = !_canvas.enabled;
        _canvas.enabled = isActive;

        OnInventoryToggled?.Invoke(isActive);

        if (isActive)
        {
            RefreshView();
        }
    }
}