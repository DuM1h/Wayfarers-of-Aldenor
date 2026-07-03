using UnityEngine;
using System.Collections.Generic;

public class EquipmentPanelView : MonoBehaviour
{
    [SerializeField] private List<EquipmentSlotView> _slots;

    private PlayerCharacter _player;

    public void Init(PlayerCharacter player)
    {
        _player = player;

        _player.Equipment.OnEquipmentChanged += UpdateSlotVisuals;

        foreach (var slot in _slots)
        {
            slot.OnSlotClicked += HandleSlotClicked;
        }

        foreach (var slot in _slots)
            UpdateSlotVisuals(slot.SlotType, null, null);
    }

    private void UpdateSlotVisuals(EquipmentType type, EquipmentConfig oldItem, EquipmentConfig newItem)
    {
        var targetSlot = _slots.Find(s => s.SlotType == type);

        if (targetSlot != null)
        {
            targetSlot.SetItem(newItem);
        }
    }

    private void HandleSlotClicked(EquipmentType clickedType)
    {
        var itemInSlot = _player.Equipment.GetItemInSlot(clickedType);
        if (itemInSlot == null) return;

        var unequippedItem = _player.Equipment.UnequipItem(clickedType);

        if (unequippedItem != null)
        {
            _player.CharacterInventory.TryAddItem(unequippedItem, 1);
        }
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.Equipment.OnEquipmentChanged -= UpdateSlotVisuals;
        }

        foreach (var slot in _slots)
        {
            slot.OnSlotClicked -= HandleSlotClicked;
        }
    }
}