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

    public event Action<Vector2Int, Vector2Int> OnGazeDirectionChanged;

    public EnemyBrain(string name, Vector2Int initialPosition, CharacterStats stats, int maxActionPoints, int maxBonusActions, int maxMovementPoints, int aggroRadius, Character playerCharacter)
        : base(name, initialPosition, stats, maxActionPoints, maxBonusActions, maxMovementPoints)
    {
        AggroRadius = aggroRadius;
        _playerCharacter = playerCharacter;
    }

    public void ProcessTurn(GameGrid grid)
    {
        if (Stats.IsDead || !IsAggroed || IsMovingVisually) return;

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
            OnGazeDirectionChanged?.Invoke(Position, _playerCharacter.Position);
            Attack(_playerCharacter, grid);
            AvailableMovementPoints = 0;
            AvailableBonusActions = 0;
        }
    }

    public bool TryDetectPlayer(GameGrid grid)
    {
        if (_playerCharacter.Stats.IsDead || Stats.IsDead)
        {
            IsAggroed = false;
            return false;
        }
        int distanceToPlayer = grid.GetDistance(grid.GetNode(Position), grid.GetNode(_playerCharacter.Position));

        if (distanceToPlayer <= AggroRadius || (IsAggroed && distanceToPlayer <= AggroRadius + 5))
        {
            IsAggroed = true;
            return true;
        }

        IsAggroed = false;
        return false;
    }

    public override void ResetTurn()
    {
        base.ResetTurn();

        if (!Stats.IsDead)
        {
            _currentPath?.Clear();
        }
    }
}
