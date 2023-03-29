using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private bool _controlsEnabled;
    private MovementController _movementController;
    private PlatformsController _platformsController;

    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
        _platformsController = GetComponent<PlatformsController>();
    }

    /// <summary> Toggles the controls. Returns the state of the controls at after the toggle.</summary>
    public bool ToggleControls()
    {
        return _controlsEnabled = !_controlsEnabled;
    }

    public void Movement(InputAction.CallbackContext context)
    {
        _movementController.MovementDirection = context.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started)
            _movementController.HoldingJump = true;

        if (context.canceled)
            _movementController.HoldingJump = false;
    }

    public void SpawnPlatform(InputAction.CallbackContext context)
    {
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
        if (context.performed)
            _platformsController.ToggleSelectionWheel(true);
        
        if (context.canceled)
            _platformsController.ToggleSelectionWheel(false);
    }
}
