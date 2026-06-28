using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : Character
{
    public int AggroRadius {  get; private set; }
    public bool IsAggroed { get; private set; }

    private Character _playerCharacter;

    private Queue<Vector2Int> _currentPath = new Queue<Vector2Int>();
    public IReadOnlyCollection<Vector2Int> CurrentPath => _currentPath;
    public EnemyBrain(string name, Vector2Int initialPosition, int maxHealth, int maxActionPoints, int maxBonusActions, int maxMovementPoints, int aggroRadius, Character playerCharacter)
        : base(name, initialPosition, maxHealth, maxActionPoints, maxBonusActions, maxMovementPoints)
    {
        AggroRadius = aggroRadius;
        _playerCharacter = playerCharacter;
    }

    public void ProcessTurn(GameGrid grid)
    {
        if (IsDead || !IsAggroed || IsMovingVisually) return;

        int distanceToPlayer = grid.GetDistance(grid.GetNode(Position), grid.GetNode(_playerCharacter.Position));

        if (distanceToPlayer > 1 && AvailableMovementPoints > 0)
        {
            if (_currentPath == null || _currentPath.Count == 0)
            {
                _currentPath = Pathfinder.FindPath(Position, _playerCharacter.Position, grid, this);
            }

            if (_currentPath.Count > 0)
            {
                Vector2Int nextPosition = _currentPath.Dequeue();
                if (!TryMove(nextPosition, grid))
                {
                    _currentPath.Clear();
                }
            }
        }
        else if (distanceToPlayer > 1 && AvailableMovementPoints <= 0)
        {
            AvailableActionPoints = 0;
            AvailableBonusActions = 0;
        }
        else if (distanceToPlayer == 1)
        {
            Attack(_playerCharacter, grid);
            AvailableMovementPoints = 0;
            AvailableBonusActions = 0;
        }
    }

    public bool TryDetectPlayer(GameGrid grid)
    {
        int distanceToPlayer = grid.GetDistance(grid.GetNode(Position), grid.GetNode(_playerCharacter.Position));

        if (distanceToPlayer <= AggroRadius)
        {
            IsAggroed = true;
            return true;
        }
        return false;
    }

    public override void ResetTurn()
    {
        base.ResetTurn();

        if (!IsDead)
        {
            _currentPath?.Clear();
        }
    }
}
