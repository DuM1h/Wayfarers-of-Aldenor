using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerView : CharacterView
{
    [SerializeField] protected LineRenderer _pathLineRenderer;

    public void SetPath(Vector2Int targetGridPos)
    {
        _currentPath = Pathfinder.FindPath(_logicalCharacter.Position, targetGridPos, _gridManager.GetGameGrid());
    }

    public void UpdatePathPreview(Vector2Int targetGridPos)
    {
        if (_logicalCharacter == null || _isMovingSmoothly || _currentPath.Count > 0)
        {
            ClearPathPreview();
            return;
        }

        if (targetGridPos == _logicalCharacter.Position)
        {
            ClearPathPreview();
            return;
        }

        Queue<Vector2Int> path = Pathfinder.FindPath(_logicalCharacter.Position, targetGridPos, _gridManager.GetGameGrid());

        if (path.Count == 0)
        {
            ClearPathPreview();
            return;
        }

        _pathLineRenderer.positionCount = path.Count + 1;

        _pathLineRenderer.SetPosition(0, _gridManager.GetCellCenterWorld(_logicalCharacter.Position));

        int index = 1;
        foreach (var step in path)
        {
            _pathLineRenderer.SetPosition(index, _gridManager.GetCellCenterWorld(step));
            index++;
        }
    }

    public void ClearPathPreview()
    {
        if (_pathLineRenderer != null)
        {
            _pathLineRenderer.positionCount = 0;
        }
    }

    public override void ProcessStep(Vector2Int targetGridPos)
    {
        GameGrid grid = _gridManager.GetGameGrid();
        Vector2Int v = targetGridPos - _logicalCharacter.Position;

        if (v == Vector2Int.right) _currentFacingDirection = CharacterFacingDirection.Right;
        else if (v == Vector2Int.left) _currentFacingDirection = CharacterFacingDirection.Left;
        else if (v == Vector2Int.up) _currentFacingDirection = CharacterFacingDirection.Up;
        else if (v == Vector2Int.down) _currentFacingDirection = CharacterFacingDirection.Down;

        Rotate();

        if (_logicalCharacter.TryMove(targetGridPos, grid))
        {
            _currentAnimationState = CharacterAnimationState.Walking;

            _targetGlobalPosition = _gridManager.GetCellCenterWorld(_logicalCharacter.Position);
            _isMovingSmoothly = true;
            _logicalCharacter.IsMovingVisually = true;

            switch (_turnManager.CurrentState)
            {
                case TurnState.FreeExploration:
                    _turnManager.TickFreeTurn();
                    _turnManager.CheckForCombatTriggers();
                    break;
                case TurnState.Combat:
                    _turnManager.CheckAndAdvanceCombatTurn();
                    break;
            }
        }
        else
        {
            _currentPath.Clear();
            Debug.Log($"Шлях заблоковано або недостатньо Очок Руху в бою!");
        }
    }
}