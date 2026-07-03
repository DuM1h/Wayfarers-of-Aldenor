using System;
using UnityEngine;

public abstract class Character
{
    public string Name { get; protected set; }

    public Vector2Int Position { get; protected set; }

    public CharacterStats Stats { get; protected set; }

    public int AvailableActionPoints { get; protected set; }
    public int AvailableBonusActions { get; protected set; }
    public int AvailableMovementPoints { get; protected set; }

    public int MaxActionPoints { get; protected set; }
    public int MaxBonusActions { get; protected set; }
    public int MaxMovementPoints { get; protected set; }

    public bool IsMovingVisually { get; set; } = false;

    public Inventory CharacterInventory { get; protected set; }

    public event Action OnResourcesChanged;
    public event Action<Vector2Int, Vector2Int> OnMoved;
    public event Action<Vector2Int> OnCharacterDied;

    public Character(string name, Vector2Int initialPosition, CharacterStats stats, int maxActionPoints, int maxBonusActions, int maxMovementPoints)
    {
        Name = name;
        Position = initialPosition;
        Stats = stats;

        MaxActionPoints = maxActionPoints;
        MaxBonusActions = maxBonusActions;
        MaxMovementPoints = maxMovementPoints;
        MaxActionPoints = maxActionPoints;
        MaxBonusActions = maxBonusActions;
        MaxMovementPoints = maxMovementPoints;

        CharacterInventory = new Inventory(stats.MaxWeightCapacity); 

        Stats.OnDied += HandleDeath;

        ResetTurn();
        OnResourcesChanged?.Invoke();
    }

    public virtual bool TryMove(Vector2Int newPosition, GameGrid grid)
    {
        if (AvailableMovementPoints <= 0 || Stats.IsDead) return false;

        Node targetNode = grid.GetNode(newPosition);
        if (targetNode == null || !targetNode.IsWalkable || targetNode.IsOccupied) return false;

        int distance = grid.GetDistance(grid.GetNode(Position), grid.GetNode(newPosition));
        if (distance != 1) return false;

        var previousNode = grid.GetNode(Position);
        previousNode.IsOccupied = false;
        previousNode.OccupyingCharacter = null;

        Vector2Int oldPosition = Position;

        Position = newPosition;
        targetNode.IsOccupied = true;
        targetNode.OccupyingCharacter = this;

        AvailableMovementPoints -= 1;
        OnResourcesChanged?.Invoke();

        OnMoved?.Invoke(oldPosition, Position);

        return true;
    }

    public virtual bool TryConsumeAction(bool isBonusAction)
    {
        if (Stats.IsDead)
            return false;
        if (isBonusAction)
        {
            if (AvailableBonusActions <= 0)
                return false;
            AvailableBonusActions --;
            OnResourcesChanged?.Invoke();
        }
        else
        {
            if (AvailableActionPoints <= 0)
                return false;
            AvailableActionPoints --;
            OnResourcesChanged?.Invoke();
        }
        return true;
    }

    public bool HasExhaustedTurn()
    {
        return AvailableActionPoints <= 0 && AvailableBonusActions <= 0 && AvailableMovementPoints <= 0;
    }

    public virtual void ResetTurn()
    {
        if (!Stats.IsDead)
        {
            AvailableActionPoints = MaxActionPoints;
            AvailableBonusActions = MaxBonusActions;
            AvailableMovementPoints = MaxMovementPoints;
            OnResourcesChanged?.Invoke();
        }
    }

    public virtual void Attack(Character target, GameGrid grid)
    {
        if (!TryConsumeAction(false))
            return;

        target.Stats.TakeDamage(this.Stats.TotalDamage, grid);
        Debug.Log("Character attacked");
    }

    public virtual bool TryConsumeItem(ItemConfig item)
    {
        if (item.Type != ItemType.Consumable) return false;
        if (Stats.IsDead) return false;

        var consumableItem = item as ConsumableConfig;

        if (TurnManager.CurrentState == TurnState.Combat)
        {
            if (AvailableBonusActions <= 0)
                return false;

            AvailableBonusActions--;
            OnResourcesChanged?.Invoke();
        }

        Stats.ChangeHealth(consumableItem.HealAmount);

        CharacterInventory.RemoveItem(item);

        return true;
    }

    public void HandleDeath()
    {
        OnCharacterDied?.Invoke(Position);
    }
}
