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

    private Rigidbody rb;

    private Vector2 movementXY;
    private Vector2 rotationXY;
    [SerializeField] private bool isControlling;

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

    private void UpdateParameters()
    {

    }

    private Vector2 currentRotation; // накопленные углы поворота

    private void RotationLogic()
    {
        // Накопление углов поворота от геймпада
        currentRotation.x += rotationXY.x * rotationSpeed * Time.deltaTime;
        currentRotation.y += rotationXY.y * rotationSpeed * Time.deltaTime;

        // Ограничим вертикальный угол, чтобы не перевернуться
        currentRotation.y = Mathf.Clamp(currentRotation.y, -80f, 80f);

        // Применим поворот через кватернион — стабильно и без дрейфа
        Quaternion targetRotation = Quaternion.Euler(-currentRotation.y, currentRotation.x, 0);

        // Прямое присвоение или плавная интерполяция
        transform.rotation = targetRotation;

        // Или плавный поворот (раскомментируй, если нужно сглаживание):
        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }



    private void MovementLogic()
    {
        transform.Translate(speed * Time.deltaTime);
    }
}
