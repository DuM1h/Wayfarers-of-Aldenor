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

    public virtual void Initialize(Character character, TurnManager turnManager, GridManager gridManager)
    {
        _logicalCharacter = character;
        _turnManager = turnManager;
        _gridManager = gridManager;

        gridManager.UpdateNodeOccupancy(_logicalCharacter.Position, _logicalCharacter);

        transform.position = _gridManager.GetCellCenterWorld(_logicalCharacter.Position);

        _targetGlobalPosition = transform.position;
        _logicalCharacter.OnMoved += ProcessStep;
        _logicalCharacter.OnDamageTaken += HurtAnimation;
        _logicalCharacter.OnDied += HandleDeath;
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
            ProcessStep(_logicalCharacter.Position ,nextStep);
            return;
        }
    }

    public virtual void ProcessStep(Vector2Int oldPos, Vector2Int newPos)
    {
        GameGrid grid = _gridManager.GetGameGrid();

        Vector2Int v = newPos - oldPos;

        if (v == Vector2Int.right) _currentFacingDirection = CharacterFacingDirection.Right;
        else if (v == Vector2Int.left) _currentFacingDirection = CharacterFacingDirection.Left;
        else if (v == Vector2Int.up) _currentFacingDirection = CharacterFacingDirection.Up;
        else if (v == Vector2Int.down) _currentFacingDirection = CharacterFacingDirection.Down;

        Rotate();

        _currentAnimationState = CharacterAnimationState.Walking;

        _targetGlobalPosition = _gridManager.GetCellCenterWorld(newPos);
        _isMovingSmoothly = true;
        _logicalCharacter.IsMovingVisually = true;
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

    protected void HandleAnimation()
    {
        if (_animator == null) return;
        _animator.SetBool("IsFacingSide", _currentFacingDirection == CharacterFacingDirection.Left || _currentFacingDirection == CharacterFacingDirection.Right);
        _animator.SetBool("IsFacingUp", _currentFacingDirection == CharacterFacingDirection.Up);
        _animator.SetBool("IsFacingDown", _currentFacingDirection == CharacterFacingDirection.Down);
        _animator.SetBool("Walking", _currentAnimationState == CharacterAnimationState.Walking);
        _animator.SetBool("Idle", _currentAnimationState == CharacterAnimationState.Idle);
    }

    protected void HurtAnimation()
    {
        if (_animator == null) return;
        _animator.SetTrigger("DamageTaken");
    }

    protected void HandleDeath()
    {
        if (_animator == null) return;
        _animator.SetBool("Dead", true);
    }

    private void OnDestroy()
    {
        if (_logicalCharacter != null)
        {
            _logicalCharacter.OnMoved -= ProcessStep;
            _logicalCharacter.OnDamageTaken -= HurtAnimation;
            _logicalCharacter.OnDied -= HandleDeath;
        }
    }
}