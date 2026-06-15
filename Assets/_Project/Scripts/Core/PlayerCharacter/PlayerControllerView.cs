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

    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Vector3 _targetGlobalPosition;
    private bool _isMovingSmoothly = false;

    [HideInInspector] public PlayerAnimationState CurrentAnimationState = PlayerAnimationState.Idle;
    [HideInInspector] public PlayerFacingDirection CurrentFacingDirection = PlayerFacingDirection.Down;

    public void Initialize(Character character, TurnManager turnManager, GridManager gridManager)
    {
        _logicalCharacter = character;
        _turnManager = turnManager;
        _gridManager = gridManager;

        transform.position = _gridManager.LogicalToUnityCoords(_logicalCharacter.Position);
        _targetGlobalPosition = transform.position;
    }

    private void Update()
    {
        if (_logicalCharacter == null) return;

        HandleVisualMovement();

        if (_isMovingSmoothly) return;
    }

    private void ProcessPlayerStep(Vector2Int targetGridPos)
    {
        GameGrid grid = _gridManager.GetGameGrid();

        if (_logicalCharacter.TryMove(targetGridPos, grid))
        {
            _targetGlobalPosition = _gridManager.LogicalToUnityCoords(_logicalCharacter.Position);
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
            CurrentAnimationState = PlayerAnimationState.Idle;
        }
    }

    private void Rotate()
    {
        _spriteRenderer.flipX = CurrentFacingDirection == PlayerFacingDirection.Left;
    }


    public void TryToMove(Vector2Int direction)
    {
        Vector2Int targetGridPos = _logicalCharacter.Position + direction;
        if (!_isMovingSmoothly)
            ProcessPlayerStep(targetGridPos);
        if (direction == Vector2Int.left || direction == Vector2Int.right)
            Rotate();

        if (direction == Vector2Int.up)
        {
            CurrentAnimationState = PlayerAnimationState.Walking;
            CurrentFacingDirection = PlayerFacingDirection.Up;
        }
        else if (direction == Vector2Int.down)
        {
            CurrentAnimationState = PlayerAnimationState.Walking;
            CurrentFacingDirection = PlayerFacingDirection.Down;
        }
        else if (direction == Vector2Int.left)
        {
            CurrentAnimationState = PlayerAnimationState.Walking;
            CurrentFacingDirection = PlayerFacingDirection.Left;
        }
        else if (direction == Vector2Int.right)
        {
            CurrentAnimationState = PlayerAnimationState.Walking;
            CurrentFacingDirection = PlayerFacingDirection.Right;
        }
    }
}
