using System.Collections.Generic;
using UnityEngine;

public enum PlayerFacingDirection
{
    Up,
    Down,
    Left,
    Right
}

public enum PlayerAnimationState
{
    Idle,
    Walking,
    Attacking,
    Dying
}



public class PlayerControllerView : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Character _logicalCharacter;
    private TurnManager _turnManager;
    private GridManager _gridManager;

    [Header("UI References")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private LineRenderer _pathLineRenderer;
    [SerializeField] private Animator _animator;

    private Vector3 _targetGlobalPosition;
    private bool _isMovingSmoothly = false;

    private PlayerAnimationState _currentAnimationState = PlayerAnimationState.Idle;
    private PlayerFacingDirection _currentFacingDirection = PlayerFacingDirection.Down;

    private Queue<Vector2Int> _currentPath = new Queue<Vector2Int>();

    public void Initialize(Character character, TurnManager turnManager, GridManager gridManager)
    {
        _logicalCharacter = character;
        _turnManager = turnManager;
        _gridManager = gridManager;

        transform.position = _gridManager.GetCellCenterWorld(_logicalCharacter.Position);

        _targetGlobalPosition = transform.position;
    }

    private void Update()
    {
        HandleAnimation();

        if (_logicalCharacter == null) return;

        HandleVisualMovement();

        if (_isMovingSmoothly) return;

        if (_currentPath.Count > 0)
        {
            Vector2Int nextStep = _currentPath.Dequeue();
            ProcessPlayerStep(nextStep);
            return;
        }
    }

    private void ProcessPlayerStep(Vector2Int targetGridPos)
    {
        GameGrid grid = _gridManager.GetGameGrid();
        Vector2Int v = targetGridPos - _logicalCharacter.Position;

        if (v == Vector2Int.right) _currentFacingDirection = PlayerFacingDirection.Right;
        else if (v == Vector2Int.left) _currentFacingDirection = PlayerFacingDirection.Left;
        else if (v == Vector2Int.up) _currentFacingDirection = PlayerFacingDirection.Up;
        else if (v == Vector2Int.down) _currentFacingDirection = PlayerFacingDirection.Down;

        Rotate();

        if (_logicalCharacter.TryMove(targetGridPos, grid))
        {
            _currentAnimationState = PlayerAnimationState.Walking;

            _targetGlobalPosition = _gridManager.GetCellCenterWorld(_logicalCharacter.Position);
            _isMovingSmoothly = true;

            switch (_turnManager.CurrentState)
            {
                case TurnState.FreeExploration:
                    _turnManager.TickFreeTurn();
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

    private void HandleVisualMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetGlobalPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetGlobalPosition) < 0.001f)
        {
            transform.position = _targetGlobalPosition;
            _isMovingSmoothly = false;
            _currentAnimationState = PlayerAnimationState.Idle;
        }
    }

    private void Rotate()
    {
        _spriteRenderer.flipX = _currentFacingDirection == PlayerFacingDirection.Left;
    }


    public void TryToMove(Vector2Int direction)
    {
        if (_isMovingSmoothly) return;

        _currentPath.Clear();
        Vector2Int targetGridPos = _logicalCharacter.Position + direction;
        ProcessPlayerStep(targetGridPos);
    }

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

    private void HandleAnimation()
    {
        if (_animator == null) return;
        _animator.SetBool("IsFacingSide", _currentFacingDirection == PlayerFacingDirection.Left || _currentFacingDirection == PlayerFacingDirection.Right);
        _animator.SetBool("IsFacingUp", _currentFacingDirection == PlayerFacingDirection.Up);
        _animator.SetBool("IsFacingDown", _currentFacingDirection == PlayerFacingDirection.Down);
        _animator.SetBool("Walking", _currentAnimationState == PlayerAnimationState.Walking);
        _animator.SetBool("Idle", _currentAnimationState == PlayerAnimationState.Idle);
    }
}