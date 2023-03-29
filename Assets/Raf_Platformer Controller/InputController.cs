using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private bool _controlsEnabled;
    private MovementController _movementController;

    private void Start()
    {
        _movementController = GetComponent<MovementController>();
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
}
