using UnityEngine;
using UnityEngine.UI;
using System;

public class EquipmentSlotView : MonoBehaviour
{
    [SerializeField] private EquipmentType _slotType;
    [SerializeField] private Image _slotIcon;
    [SerializeField] private Sprite _emptySlotIcon;
    [SerializeField] private Sprite _filledSlotIcon;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Button _slotButton;

    public EquipmentType SlotType => _slotType;

    public event Action<EquipmentType> OnSlotClicked;

    private void Awake()
    {
        _slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(_slotType));
    }

    public void SetItem(EquipmentConfig item)
    {
        if (item != null)
        {
            _slotIcon.sprite = _filledSlotIcon;
            _itemIcon.sprite = item.Icon;
            _itemIcon.enabled = true;
        }
        else
        {
            _slotIcon.sprite = _emptySlotIcon;
            _itemIcon.sprite = null;
            _itemIcon.enabled = false;
        }
    }
}