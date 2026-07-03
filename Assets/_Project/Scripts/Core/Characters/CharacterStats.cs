using System;
using UnityEngine;
public class CharacterStats
{
    public event Action<int, int> OnHealthChanged;
    public event Action OnDamageTaken;
    public event Action OnDied;

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int TotalArmor => _baseArmor + _equippedArmorBonus;
    public int TotalDamage => _baseDamage + _equippedDamageBonus;
    public float MaxWeightCapacity { get; private set; }
    public bool IsDead => CurrentHealth <= 0;

    private int _baseArmor;
    private int _baseDamage;

    private int _equippedArmorBonus = 0;
    private int _equippedDamageBonus = 0;

    public CharacterStats(int maxHealth, int currentHealth, int armor, int baseDamage, float maxWeight)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        _baseArmor = armor;
        _baseDamage = baseDamage;
        MaxWeightCapacity = maxWeight;
    }

    public void TakeDamage(int amount, GameGrid grid)
    {
        if (IsDead)
            return;

        ChangeHealth(-amount + TotalArmor);
        OnDamageTaken?.Invoke();
        Debug.Log("Character took damage");

        if (IsDead)
            OnDied?.Invoke();
    }

    public void ChangeHealth(int amount)
    {
        if (IsDead)
            return;
        CurrentHealth = Math.Clamp(CurrentHealth + amount, 0, MaxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void RecalculateStats() 
    {
        
    }

    public void UpdateEquipmentModifiers(int armorBonus, int damageBonus)
    {
        _equippedArmorBonus = armorBonus;
        _equippedDamageBonus = damageBonus;
    }
}
