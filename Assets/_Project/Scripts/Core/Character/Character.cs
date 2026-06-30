using System;
using UnityEngine;

public class Character
{
    public string Name { get; protected set; }

    public Vector2Int Position { get; protected set; }

    public int MaxHealth { get; protected set; }
    public int CurrentHealth { get; protected set; }

    public int AvailableActionPoints { get; protected set; }
    public int AvailableBonusActions { get; protected set; }
    public int AvailableMovementPoints { get; protected set; }

    public int MaxActionPoints { get; protected set; }
    public int MaxBonusActions { get; protected set; }
    public int MaxMovementPoints { get; protected set; }

    public bool IsDead => CurrentHealth <= 0;

    public bool IsMovingVisually { get; set; } = false;

    public Inventory CharacterInventory { get; protected set; }

    public event Action<int, int> OnHealthChanged;
    public event Action OnDamageTaken;
    public event Action OnDied;
    public event Action OnResourcesChanged;

    public event Action<Vector2Int, Vector2Int> OnMoved;

    public Character(string name, Vector2Int initialPosition, int maxHealth, int maxActionPoints, int maxBonusActions, int maxMovementPoints, float maxInventoryWeight)
    {
        Name = name;
        Position = initialPosition;

        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;

        MaxActionPoints = maxActionPoints;
        MaxBonusActions = maxBonusActions;
        MaxMovementPoints = maxMovementPoints;

        CharacterInventory = new Inventory(maxInventoryWeight); 

        ResetTurn();
        OnResourcesChanged?.Invoke();
    }

    public bool TryMove(Vector2Int newPosition, GameGrid grid)
    {
        if (AvailableMovementPoints <= 0 || IsDead) return false;

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

    public bool TryConsumeAction(bool isBonusAction)
    {
        if (IsDead)
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
        if (!IsDead)
        {
            AvailableActionPoints = MaxActionPoints;
            AvailableBonusActions = MaxBonusActions;
            AvailableMovementPoints = MaxMovementPoints;
            OnResourcesChanged?.Invoke();
        }
    }

    public void TakeDamage(int amount, GameGrid grid)
    {
        if (IsDead)
            return; 

        CurrentHealth = Math.Max(CurrentHealth - amount, 0);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnDamageTaken?.Invoke();
        Debug.Log("Character took damage");

        if (IsDead)
        {
            OnDied?.Invoke();
            var Node = grid.GetNode(Position);
            Node.IsOccupied = false;
            Node.OccupyingCharacter = null;
        }
    }

    public void Attack(Character target, GameGrid grid)
    {
        if (!TryConsumeAction(false))
            return;

        target.TakeDamage(50, grid);
        Debug.Log("Character attacked");
    }

    public bool TryConsumeItem(ItemConfig item)
    {
        if (item.type != ItemType.Consumable) return false;
        if (IsDead) return false;

        if (TurnManager.CurrentState == TurnState.Combat)
        {
            if (AvailableBonusActions <= 0)
                return false;

            AvailableBonusActions--;
            OnResourcesChanged?.Invoke();
        }

        CurrentHealth = Math.Min(MaxHealth, CurrentHealth + item.healAmount);
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        CharacterInventory.RemoveItem(item);

        return true;
    }
}
