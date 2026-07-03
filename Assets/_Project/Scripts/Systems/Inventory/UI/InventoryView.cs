using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private Transform _gridContainer;
    [SerializeField] private InventorySlot _slotPrefab;
    [SerializeField] private TooltipView _tooltipView;
    [SerializeField] private TextMeshProUGUI _inventoryWeightText;
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

        UpdateWeightText();
        RebuildSlots();
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
        if (item.Type != ItemType.Equipment && !wasAdded)
        {
            UpdateWeightText();
        }
        RebuildSlots();
    }

    private void UpdateWeightText()
    {
        if (_logicalInventory == null) return;
        _inventoryWeightText.text = $"Weight: {_logicalInventory.TotalWeight:F1}/{_logicalInventory.MaxWeight:F1}";
    }

    private void RebuildSlots()
    {
        if (_logicalInventory == null) return;

        foreach (var slot in _spawnedSlots)
        {
            slot.OnSlotClicked -= OnItemClicked;
            slot.OnHoverEnter -= _tooltipView.ShowTooltip;
            slot.OnHoverExit -= _tooltipView.HideTooltip;
            Destroy(slot.gameObject);
        }
        _spawnedSlots.Clear();

        foreach (var slot in _logicalInventory.AllItems)
        {
            ItemConfig item = slot.Key;
            int amount = slot.Value;

            if (!item.isStackable)
            {
                for (int i = 0; i < amount; i++)
                {
                    InventorySlot newSlot = Instantiate(_slotPrefab, _gridContainer);
                    newSlot.OnSlotClicked += OnItemClicked;
                    newSlot.OnHoverEnter += _tooltipView.ShowTooltip;
                    newSlot.OnHoverExit += _tooltipView.HideTooltip;
                    newSlot.Setup(item, 1);
                    _spawnedSlots.Add(newSlot);
                }
            }
            else
            {
                InventorySlot newSlot = Instantiate(_slotPrefab, _gridContainer);
                newSlot.OnSlotClicked += OnItemClicked;
                newSlot.OnHoverEnter += _tooltipView.ShowTooltip;
                newSlot.OnHoverExit += _tooltipView.HideTooltip;
                newSlot.Setup(item, amount);
                _spawnedSlots.Add(newSlot);
            }
        }
    }

    public void ToggleInventory()
    {
        bool isActive = !_canvas.enabled;
        _canvas.enabled = isActive;

        OnInventoryToggled?.Invoke(isActive);

        if (isActive)
        {
            UpdateWeightText();
            RebuildSlots();
        }
    }
}