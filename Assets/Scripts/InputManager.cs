using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public delegate void OnPlayerPressed(Vector2 touchPositon);
    public static event OnPlayerPressed onPlayerPressed;

    public PlayerInput playerInput;

    private Vector2 touchPosition;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Touch.Enable();
    }
    private void OnEnable()
    {
        playerInput.Touch.TouchPress.started += ReadPos;
    }
    
    private void OnDestroy()
    {
        playerInput.Touch.Disable();
        playerInput.Touch.TouchPress.started -= ReadPos;
    }
    private void ReadPos(InputAction.CallbackContext ctx)
    {
        touchPosition = playerInput.Touch.TouchPosition.ReadValue<Vector2>();
        onPlayerPressed?.Invoke(touchPosition);
    }

}
