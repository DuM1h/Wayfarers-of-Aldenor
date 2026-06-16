using UnityEngine;

public class Character
{
    public string Name { get; private set; }

    public Vector2Int Position { get; private set; }

    public int AvailableActionPoints { get; private set; }
    public int AvailableBonusActions { get; private set; }
    public int AvailableMovementPoints { get; private set; }

    public int MaxActionPoints { get; private set; }
    public int MaxBonusActions { get; private set; }
    public int MaxMovementPoints { get; private set; }

    public bool IsDead { get; private set; }

    public Character(string name, Vector2Int initialPosition, int maxActionPoints, int maxBonusActions, int maxMovementPoints)
    {
        Name = name;
        Position = initialPosition;

        MaxActionPoints = maxActionPoints;
        MaxBonusActions = maxBonusActions;
        MaxMovementPoints = maxMovementPoints;

        ResetTurn();
        IsDead = false;
    }

    public bool TryMove(Vector2Int newPosition, GameGrid grid)
    {
        if (AvailableMovementPoints <= 0 || IsDead) return false;

        Node targetNode = grid.GetNode(newPosition);
        if (targetNode == null || !targetNode.IsWalkable) return false;

        int distance = grid.GetDistance(grid.GetNode(Position), grid.GetNode(newPosition));
        if (distance != 1) return false;

        Position = newPosition;
        AvailableMovementPoints -= 1;

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
            
        }
        else
        {
            if (AvailableActionPoints <= 0)
                return false;
            AvailableActionPoints --;
        }
        return true;
    }

    public bool HasExhaustedTurn()
    {
        return AvailableActionPoints <= 0 && AvailableBonusActions <= 0 && AvailableMovementPoints <= 0;
    }

    public void ResetTurn()
    {
        if (!IsDead)
        {
            AvailableActionPoints = MaxActionPoints;
            AvailableBonusActions = MaxBonusActions;
            AvailableMovementPoints = MaxMovementPoints;
        }
    }
}
