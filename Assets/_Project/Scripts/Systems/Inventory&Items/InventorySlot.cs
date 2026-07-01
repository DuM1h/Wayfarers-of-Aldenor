using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _amountText;

    private ItemConfig _currentItem;
    private int _amount;
    public event Action<ItemConfig> OnSlotClicked;
    public event Action<ItemConfig, int> OnHoverEnter;
    public event Action OnHoverExit;

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
        _amount = amount;

        _iconImage.sprite = item.sprite;
        _amountText.text = amount > 1 ? amount.ToString() : "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItem != null)
        {
            OnHoverEnter?.Invoke(_currentItem, _amount);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit?.Invoke();
    }
}