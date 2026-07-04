using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class CharacterStatsView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _armorText;
    [SerializeField] private TextMeshProUGUI _damageText;

    private CharacterStats _logicalStats;

    public void Init(CharacterStats stats)
    {
        _logicalStats = stats;

        _logicalStats.OnStatsChanged += UpdateUI;
        _logicalStats.OnHealthChanged += (current, max) => UpdateUI();

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (_logicalStats == null) return;

        _hpText.text = $"HP: {_logicalStats.CurrentHealth} / {_logicalStats.MaxHealth}";

        _armorText.text = $"Armor: {_logicalStats.TotalArmor}";
        if (_logicalStats.EquippedArmorBonus > 0)
            _armorText.text += $" (+{_logicalStats.EquippedArmorBonus})";

        _damageText.text = $"Damage: {_logicalStats.TotalDamage}";
        if (_logicalStats.EquippedDamageBonus > 0)
            _damageText.text += $" (+{_logicalStats.EquippedDamageBonus})";
    }

    private void OnDestroy()
    {
        if (_logicalStats != null)
        {
            _logicalStats.OnStatsChanged -= UpdateUI;
        }
    }
}