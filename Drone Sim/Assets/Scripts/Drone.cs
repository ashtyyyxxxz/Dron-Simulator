using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drone : MonoBehaviour
{
    [SerializeField] private float percentage;
    [SerializeField] private float ping;
    [SerializeField] private Vector3 speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private InputActionReference movingXYInput;
    [SerializeField] private InputActionReference rotationXYInput;
    [SerializeField] private InputActionReference toggleControlInput;

    private Rigidbody rb;

    private Vector2 movementXY;
    private Vector2 rotationXY;
    [SerializeField] private bool isControlling = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        movingXYInput.action.performed += ReadMovementValues;
        movingXYInput.action.canceled += (InputAction.CallbackContext obj) => movementXY = Vector2.zero;

        rotationXYInput.action.performed += ReadRotationValues;
        rotationXYInput.action.canceled += (InputAction.CallbackContext obj) => rotationXY = Vector2.zero;

        toggleControlInput.action.performed += ToggleControllingMode;
    }

    private void ReadRotationValues(InputAction.CallbackContext obj)
    {
        rotationXY = obj.action.ReadValue<Vector2>();
    }

    private void ReadMovementValues(InputAction.CallbackContext obj)
    {
        movementXY = obj.action.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (isControlling)
        {
            rb.useGravity = false;
            MovementLogic();
            RotationLogic();
            UpdateParameters();
        }
        else
        {
            rb.useGravity = true;
        }
    }

    private void ToggleControllingMode(InputAction.CallbackContext obj)
    {
        if (percentage <= 0) return;
        isControlling = !isControlling;
    }

    private void UpdateParameters()
    {

    }

    private Vector2 currentRotation;

    private void RotationLogic()
    {
        currentRotation.x += rotationXY.x * rotationSpeed * Time.deltaTime;
        currentRotation.y += rotationXY.y * rotationSpeed * Time.deltaTime;

        currentRotation.y = Mathf.Clamp(currentRotation.y, -80f, 80f);

        Quaternion targetRotation = Quaternion.Euler(-currentRotation.y, currentRotation.x, 0);

        transform.rotation = targetRotation;

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }



    private void MovementLogic()
    {
        transform.Translate(speed * Time.deltaTime);
    }
}
