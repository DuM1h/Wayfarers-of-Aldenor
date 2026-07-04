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
    private ItemConfig _hoveredItem;

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

        int newAmount = 0;
        bool shouldKeepTooltip = !wasAdded && item == _hoveredItem && item.isStackable
            && _logicalInventory.AllItems.TryGetValue(item, out newAmount);

        RebuildSlots(!shouldKeepTooltip);

        if (shouldKeepTooltip)
        {
            _tooltipView.ShowTooltip(item, newAmount);
        }
    }

    private void UpdateWeightText()
    {
        if (_logicalInventory == null) return;
        _inventoryWeightText.text = $"Weight: {_logicalInventory.TotalWeight:F1}/{_logicalInventory.MaxWeight:F1}";
    }

    private void RebuildSlots(bool hideTooltip = true)
    {
        if (_logicalInventory == null) return;

        if (hideTooltip)
        {
            _tooltipView.HideTooltip();
            _hoveredItem = null;
        }

        foreach (var slot in _spawnedSlots)
        {
            UnsubscribeSlot(slot);
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
                    SubscribeSlot(newSlot);
                    newSlot.Setup(item, 1);
                    _spawnedSlots.Add(newSlot);
                }
            }
            else
            {
                InventorySlot newSlot = Instantiate(_slotPrefab, _gridContainer);
                SubscribeSlot(newSlot);
                newSlot.Setup(item, amount);
                _spawnedSlots.Add(newSlot);
            }
        }
    }

    private void SubscribeSlot(InventorySlot slot)
    {
        slot.OnSlotClicked += HandleSlotClicked;
        slot.OnHoverEnter += HandleHoverEnter;
        slot.OnHoverExit += HandleHoverExit;
    }

    private void UnsubscribeSlot(InventorySlot slot)
    {
        slot.OnSlotClicked -= HandleSlotClicked;
        slot.OnHoverEnter -= HandleHoverEnter;
        slot.OnHoverExit -= HandleHoverExit;
    }

    private void HandleSlotClicked(ItemConfig item)
    {
        OnItemClicked?.Invoke(item);
    }

    private void HandleHoverEnter(ItemConfig item, int amount)
    {
        _hoveredItem = item;
        _tooltipView.ShowTooltip(item, amount);
    }

    private void HandleHoverExit()
    {
        _hoveredItem = null;
        _tooltipView.HideTooltip();
    }

    public void ToggleInventory()
    {
        bool isActive = !_canvas.enabled;
        _canvas.enabled = isActive;
        _tooltipView.HideTooltip();
        _hoveredItem = null;

        OnInventoryToggled?.Invoke(isActive);

        if (isActive)
        {
            UpdateWeightText();
            RebuildSlots();
        }
    }
}
