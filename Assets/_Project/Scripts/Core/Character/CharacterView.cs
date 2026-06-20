using System.Collections.Generic;
using UnityEngine;

public enum CharacterFacingDirection
{
    Up,
    Down,
    Left,
    Right
}

public enum CharacterAnimationState
{
    Idle,
    Walking,
    Attacking,
    Dying
}



public class CharacterView : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 5f;

    protected Character _logicalCharacter;
    protected TurnManager _turnManager;
    protected GridManager _gridManager;

    [Header("UI References")]
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Animator _animator;

    protected Vector3 _targetGlobalPosition;
    protected bool _isMovingSmoothly = false;

    protected CharacterAnimationState _currentAnimationState = CharacterAnimationState.Idle;
    protected CharacterFacingDirection _currentFacingDirection = CharacterFacingDirection.Down;

    protected Queue<Vector2Int> _currentPath = new Queue<Vector2Int>();

    public void Initialize(Character character, TurnManager turnManager, GridManager gridManager)
    {
        _logicalCharacter = character;
        _turnManager = turnManager;
        _gridManager = gridManager;

        gridManager.UpdateNodeOccupancy(_logicalCharacter.Position, _logicalCharacter);

        transform.position = _gridManager.GetCellCenterWorld(_logicalCharacter.Position);

        _targetGlobalPosition = transform.position;
    }

    protected virtual void Update()
    {
        HandleAnimation();

        if (_logicalCharacter == null) return;

        HandleVisualMovement();

        if (_isMovingSmoothly) return;

        if (_currentPath.Count > 0)
        {
            Vector2Int nextStep = _currentPath.Dequeue();
            ProcessStep(nextStep);
            return;
        }
    }

    public virtual void ProcessStep(Vector2Int targetGridPos)
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
        }
        else
        {
            _currentPath.Clear();
        }
    }

    protected void HandleVisualMovement()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetGlobalPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetGlobalPosition) < 0.001f)
        {
            transform.position = _targetGlobalPosition;
            _isMovingSmoothly = false;
            _logicalCharacter.IsMovingVisually = false;
            _currentAnimationState = CharacterAnimationState.Idle;
        }
    }

    protected void Rotate()
    {
        _spriteRenderer.flipX = _currentFacingDirection == CharacterFacingDirection.Left;
    }


    public void TryToMove(Vector2Int direction)
    {
        if (_isMovingSmoothly) return;

        _currentPath.Clear();
        Vector2Int targetGridPos = _logicalCharacter.Position + direction;
        ProcessStep(targetGridPos);
    }

    protected void HandleAnimation()
    {
        if (_animator == null) return;
        _animator.SetBool("IsFacingSide", _currentFacingDirection == CharacterFacingDirection.Left || _currentFacingDirection == CharacterFacingDirection.Right);
        _animator.SetBool("IsFacingUp", _currentFacingDirection == CharacterFacingDirection.Up);
        _animator.SetBool("IsFacingDown", _currentFacingDirection == CharacterFacingDirection.Down);
        _animator.SetBool("Walking", _currentAnimationState == CharacterAnimationState.Walking);
        _animator.SetBool("Idle", _currentAnimationState == CharacterAnimationState.Idle);
    }
}