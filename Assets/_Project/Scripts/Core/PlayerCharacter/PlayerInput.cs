using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerControllerView _controllerView;
    [SerializeField] private PlayerInput input;

    void Awake()
    {
        _controllerView = GetComponent<PlayerControllerView>();
        input.enabled = true;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
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
}
