using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerView : CharacterView
{
    [SerializeField] protected LineRenderer _pathLineRenderer;
    private LootManager _lootManager;

    public void Initialize(Character character, TurnManager turnManager, GridManager gridManager, LootManager lootManager)
    {
        base.Initialize(character, turnManager, gridManager);
        _lootManager = lootManager;
    }

    public void SetPath(Vector2Int targetGridPos)
    {
        _currentPath = Pathfinder.FindPath(_logicalCharacter.Position, targetGridPos, _gridManager.GetGameGrid(), _logicalCharacter);
    }

    public void UpdatePathPreview(Vector2Int targetGridPos)
    {
        if (_logicalCharacter == null || _logicalCharacter.IsDead || _isMovingSmoothly || _currentPath.Count > 0)
        {
            ClearPathPreview();
            return;
        }

        if (targetGridPos == _logicalCharacter.Position)
        {
            ClearPathPreview();
            return;
        }

        Queue<Vector2Int> path = Pathfinder.FindPath(_logicalCharacter.Position, targetGridPos, _gridManager.GetGameGrid(), _logicalCharacter);

        if (path.Count == 0)
        {
            ClearPathPreview();
            return;
        }

        _pathLineRenderer.positionCount = path.Count + 1;

        _pathLineRenderer.SetPosition(0, GridManager.GetCellCenterWorld(_logicalCharacter.Position));

        int index = 1;
        foreach (var step in path)
        {
            _pathLineRenderer.SetPosition(index, GridManager.GetCellCenterWorld(step));
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

    public void ProcessPathStep(Vector2Int targetGridPos)
    {
        GameGrid grid = _gridManager.GetGameGrid();

        // 1. ЛОГІКА: Пробуємо зробити крок
        // (Якщо вийде, Character.cs сам викличе OnMoved, і базовий CharacterView запустить анімацію!)
        if (_logicalCharacter.TryMove(targetGridPos, grid))
        {
            _lootManager.TryPickupLoot(_logicalCharacter.Position, _logicalCharacter.CharacterInventory);
            // 2. МЕНЕДЖМЕНТ ГРИ: Оновлюємо стани (TurnManager)
            switch (TurnManager.CurrentState)
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
            // Якщо TryMove повернув false (немає MP або зайнято)
            _currentPath.Clear();
            Debug.Log($"Шлях заблоковано або недостатньо Очок Руху в бою!");
        }
    }

    protected override void Update()
    {
        HandleAnimation();

        if (_logicalCharacter == null || _logicalCharacter.IsDead) return;

        HandleVisualMovement();

        if (_isMovingSmoothly) return;

        if (_currentPath.Count > 0)
        {
            Vector2Int nextStep = _currentPath.Dequeue();
            ProcessPathStep(nextStep);
            return;
        }
    }

    public void TryToMove(Vector2Int direction)
    {
        if (_isMovingSmoothly) return;

        _currentPath.Clear();
        Vector2Int targetGridPos = _logicalCharacter.Position + direction;
        ProcessPathStep(targetGridPos);
    }

    public Character GetPlayerCharacter()
    {
        return _logicalCharacter;
    }

    public void HandleAttackInput(Character character)
    {
        GameGrid grid = _gridManager.GetGameGrid();
        int distance = grid.GetDistance(grid.GetNode(_logicalCharacter.Position), grid.GetNode(character.Position));

        if (distance > 1)
            Debug.Log("Ціль занадто далеко!");
        if (distance == 1)
        {
            SetFacingDirection(_logicalCharacter.Position, character.Position);
            _logicalCharacter.Attack(character, grid);
        }
    }
}