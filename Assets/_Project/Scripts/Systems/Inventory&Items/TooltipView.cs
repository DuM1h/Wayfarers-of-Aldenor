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

        _nameText.text = item.itemName;
        _descriptionText.text = item.description;
        _weightText.text = $"w: {item.weight * amount}";

        if (item.type == ItemType.Consumable && item.healAmount > 0)
        {
            _statsText.text = $"Відновлює: <color=green>{item.healAmount} HP</color>";
            _statsText.gameObject.SetActive(true);
        }
        else
        {
            _statsText.gameObject.SetActive(false);
        }

        transform.position = Mouse.current.position.ReadValue() + offset;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}