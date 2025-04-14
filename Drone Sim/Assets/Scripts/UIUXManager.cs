using UnityEngine;
using UnityEngine.InputSystem;

public class UIUXManager : MonoBehaviour
{
    public static UIUXManager instance;

    public GameObject buttonA;
    public GameObject buttonB;

    public InputActionReference aInput;
    public InputActionReference bInput;

    private void Awake()
    {
        instance = this;
    }
}
