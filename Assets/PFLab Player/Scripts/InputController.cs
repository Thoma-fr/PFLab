using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private bool _controlsEnabled;
    public bool ControlsEnabled => _controlsEnabled;
    public bool EnabledControls => _controlsEnabled;
    private MovementController _movementController;
    private PlatformsController _platformsController;

    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
        _platformsController = GetComponent<PlatformsController>();
        _controlsEnabled = true;
    }

    public void DisableControls()
    {
        _controlsEnabled = false;
    }

    public void EnableControls()
    {
        _controlsEnabled = true;
    }

    public void Movement(InputAction.CallbackContext context)
    {
        if (!_controlsEnabled)
        {
            _movementController.MovementDirection = Vector2.zero;
            return;
        }

        _movementController.MovementDirection = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!_controlsEnabled)
        {
            _movementController.HoldingJump = false;
            return;
        }

        if (context.started)
            _movementController.HoldingJump = true;

        if (context.canceled)
            _movementController.HoldingJump = false;
    }

    public void SpawnPlatform(InputAction.CallbackContext context)
    {
        if (!_controlsEnabled)
            return;
        
        if (_platformsController.choosenPlatform == PLATFORM.NONE) return;

        if (context.started)
            _platformsController.SpawnPlatform();

        if (context.performed)
            _platformsController.RotatingPlatform = true;

        if (context.canceled)
        {
            _platformsController.RotatingPlatform = false;
            _platformsController.PlacePlatform();
        }
    }

    public void DisplaySelectionWheel(InputAction.CallbackContext context)
    {
        if (!_controlsEnabled)
            return;

        if (context.performed)
            _platformsController.ToggleSelectionWheel(true);
        
        if (context.canceled)
            _platformsController.ToggleSelectionWheel(false);
    }
}
