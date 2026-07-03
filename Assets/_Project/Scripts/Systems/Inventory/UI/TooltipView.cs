using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TooltipView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _weightText;
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private Vector2 offset;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.position = Vector3.Lerp(transform.position, (Vector3)Mouse.current.position.ReadValue() + (Vector3)offset, Time.deltaTime * 10f);
        }
    }

    public void ShowTooltip(ItemConfig item, int amount)
    {
        if (item == null) return;

        gameObject.SetActive(true);

        _nameText.text = item.ItemName;
        _descriptionText.text = item.Description;
        _weightText.text = $"w: {item.Weight * amount}";

        switch(item.Type)
        {
            case ItemType.Consumable:
                var consumable = (ConsumableConfig)item;
                _statsText.text = $"Відновлює: <color=green>{consumable.HealAmount} HP</color>";
                break;
            case ItemType.Equipment:
                var equipment = (EquipmentConfig)item;
                switch (equipment.SlotType)
                {
                    case EquipmentType.Weapon:
                        _statsText.text = $"Збільшує: <color=green>{equipment.DamageBonus} DMG</color>";
                        break;
                    default:
                        _statsText.text = $"Збільшує: <color=green>{equipment.ArmorBonus} ARMOR</color>";
                        break;
                }
                break;
            default:
                _statsText.text = string.Empty;
                break;
        }
        transform.position = Mouse.current.position.ReadValue() + offset;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}