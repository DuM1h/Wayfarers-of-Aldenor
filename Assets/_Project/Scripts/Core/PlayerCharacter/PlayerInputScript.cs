using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PlayerControllerView _controllerView;
    [SerializeField] private PlayerInput _unityInputSystem;

    private bool _isInputBlocked = false;

    private Vector2Int _lastHoveredGridPos = new Vector2Int(-999, -999);

    void Awake()
    {
        _controllerView ??= GetComponent<PlayerControllerView>();
        _unityInputSystem ??= GetComponent<PlayerInput>();
        _unityInputSystem.enabled = true;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (_isInputBlocked) return;

        if (context.phase == InputActionPhase.Performed)
        {
            _controllerView.ClearPathPreview();
            Vector2 inputDirection = context.ReadValue<Vector2>();
            Vector2Int direction = new Vector2Int(
                Mathf.RoundToInt(inputDirection.x),
                Mathf.RoundToInt(inputDirection.y)
            );
            if (direction != Vector2Int.zero)
            {
                _controllerView.TryToMove(direction);
            }
        }
    }

    public void SetInputBlocked(bool isInputBlocked)
    {
        _isInputBlocked = isInputBlocked;
    }

    void Update()
    {
        if (_isInputBlocked) return;

        HandleInput();
    }

    private void HandleInput()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector3Int unityCoords = new Vector3Int(
            Mathf.FloorToInt(worldPosition.x),
            Mathf.FloorToInt(worldPosition.y),
            0
        );
        Vector2Int targetGridPos = gridManager.UnityToLogicalCoords(unityCoords);

        if (targetGridPos != _lastHoveredGridPos)
        {
            _lastHoveredGridPos = targetGridPos;
            _controllerView.UpdatePathPreview(targetGridPos);
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var targetNode = gridManager.GameGrid.GetNode(targetGridPos);
            if (targetNode == null)
                return;
            if (targetNode.OccupyingCharacter != null && targetNode.OccupyingCharacter != _controllerView.GetPlayerCharacter())
            {
                _controllerView.HandleAttackInput(targetNode.OccupyingCharacter);
            }
            else
            {
                _controllerView.ClearPathPreview();
                _controllerView.SetPath(targetGridPos);
            }
        }
    }
}
