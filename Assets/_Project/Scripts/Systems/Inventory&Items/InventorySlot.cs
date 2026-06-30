using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;

    private ItemConfig _currentItem;
    public event Action<ItemConfig> OnSlotClicked;

    public void OnButtonClick()
    {
        if (_currentItem != null)
        {
            OnSlotClicked?.Invoke(_currentItem);
        }
    }

    public void Setup(ItemConfig item, int amount)
    {
        if (item == null) 
        {_iconImage.enabled = false; _amountText.enabled = false; return; }

        _currentItem = item;

        _iconImage.sprite = item.sprite;
        _amountText.text = amount > 1 ? amount.ToString() : "";
    }
}