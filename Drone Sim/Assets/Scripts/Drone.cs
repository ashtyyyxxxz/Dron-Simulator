using UnityEngine;
using UnityEngine.InputSystem;

public class Drone : MonoBehaviour
{
    [SerializeField] private float percentage;
    [SerializeField] private float ping;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private InputActionReference movingXYInput;
    [SerializeField] private InputActionReference rotationXYInput;

    private Vector2 movementXY;
    private Vector2 rotationXY;

    private void OnEnable()
    {
        movingXYInput.action.performed += ReadMovementValues;
        movingXYInput.action.canceled += (InputAction.CallbackContext obj) => movementXY = Vector2.zero;

        rotationXYInput.action.performed += ReadRotationValues;
        rotationXYInput.action.canceled += (InputAction.CallbackContext obj) => rotationXY = Vector2.zero;
    }

    private void ReadRotationValues(InputAction.CallbackContext obj)
    {
        rotationXY = obj.action.ReadValue<Vector2>();
    }

    private void ReadMovementValues(InputAction.CallbackContext obj)
    {
        movementXY = obj.action.ReadValue<Vector2>();
    }
}
