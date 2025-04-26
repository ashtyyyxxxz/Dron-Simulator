using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference leftJoystickInput;
    [SerializeField] private InputActionReference rightJoystickInput;
    [SerializeField] private InputActionReference toggleArmInput;
    [SerializeField] private InputActionReference toggleModeInput;
    [SerializeField] private InputActionReference changeCameraModeInput;

    [Header("Other Properties")]
    [SerializeField] private float percent = 100;
    [SerializeField] private float shownPing = 30;
    [SerializeField] private Transform tablet;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField] private TextMeshProUGUI pingText;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform[] cameraPositions;
    private float realPing;

    [Header("Inventory")]
    [SerializeField] private GameObject holdingItem;

    [Header("Drone Settings")]
    [SerializeField] private float throttlePower = 10f;
    [SerializeField] private float movePower = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float tiltSpeed = 50f;

    private Vector2 leftInput = Vector2.zero;
    private Vector2 rightInput = Vector2.zero;

    private bool isArmed = false;

    public enum StabilizationMode { Acro, Horizon, Hover }
    public StabilizationMode currentMode = StabilizationMode.Horizon;

    private Rigidbody rb;
    private Vector3 hoverTargetPosition;
    private bool isThirdPerson = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnEnable()
    {
        leftJoystickInput.action.performed += ctx => leftInput = ctx.ReadValue<Vector2>();
        leftJoystickInput.action.canceled += ctx => leftInput = Vector2.zero;

        rightJoystickInput.action.performed += ctx => rightInput = ctx.ReadValue<Vector2>();
        rightJoystickInput.action.canceled += ctx => rightInput = Vector2.zero;

        toggleArmInput.action.performed += ToggleArm;
        toggleModeInput.action.performed += ctx => ToggleStabilizationMode();

        changeCameraModeInput.action.performed += ChangeCameraMode;
    }

    private void OnDisable()
    {
        toggleArmInput.action.performed -= ToggleArm;
        toggleModeInput.action.performed -= ctx => ToggleStabilizationMode();
    }

    void FixedUpdate()
    {
        realPing = Vector3.Distance(transform.position,tablet.transform.position)*2 + 30;
        shownPing = UnityEngine.Random.Range(realPing-2,realPing+2);
        if (!isArmed && percent > 0 && shownPing > 500) return;

        percent -= 0.1f * Time.deltaTime;
        percentText.text = $"Заряд батареи: {Mathf.Round(percent)}%";
        pingText.text = $"Задержка: {Mathf.Round(shownPing)} мс";

        HandleThrottle();
        HandleYaw();
        HandleMovement();
        HandleStabilization();
    }

    #region DroneControlling
    private void ChangeCameraMode(InputAction.CallbackContext context)
    {
        isThirdPerson = !isThirdPerson;
        cam.parent = isThirdPerson ? cameraPositions[1] : cameraPositions[0];
        cam.localPosition = Vector3.zero;
    }

    private void ToggleArm(InputAction.CallbackContext context)
    {
        isArmed = !isArmed;

        if (isArmed)
        {
            hoverTargetPosition = transform.position;
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void ToggleStabilizationMode()
    {
        currentMode = currentMode switch
        {
            StabilizationMode.Acro => StabilizationMode.Horizon,
            StabilizationMode.Horizon => StabilizationMode.Hover,
            _ => StabilizationMode.Acro
        };
    }

    private void HandleThrottle()
    {
        float throttle = Mathf.Clamp01(leftInput.y);
        Vector3 upForce = Vector3.up * throttle * throttlePower;
        rb.AddForce(upForce);
    }

    private void HandleYaw()
    {
        float yaw = leftInput.x;
        rb.AddTorque(Vector3.up * yaw * rotationSpeed);
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.forward * rightInput.y;
        Vector3 right = transform.right * rightInput.x;
        Vector3 movement = (forward + right) * movePower;

        rb.AddForce(movement);
    }

    private void HandleStabilization()
    {
        if (currentMode == StabilizationMode.Acro) return;

        Quaternion levelRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        Quaternion targetRot = Quaternion.RotateTowards(transform.rotation, levelRot, tiltSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(targetRot);

        if (currentMode == StabilizationMode.Hover)
        {
            Vector3 posError = hoverTargetPosition - transform.position;
            Vector3 correction = new Vector3(posError.x, 0, posError.z) * 5f;
            rb.AddForce(correction, ForceMode.Acceleration);
        }
    }
    #endregion

    private void ReleaseItemsFromInventory()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ItemForDrone") && holdingItem == null)
        {
            holdingItem = other.gameObject;
            //Destroy(other.gameObject);
        }
    }
}
