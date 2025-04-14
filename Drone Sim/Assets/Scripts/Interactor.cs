using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Interactor : MonoBehaviour
{
    [SerializeField] private InteractButton interactButton;
    [SerializeField] private Transform uiButtonSpawnParent;
    public UnityEvent onEnteredZone, onInteracted, onLeftZone;

    private bool playerInZone;

    private enum InteractButton { AButton, BButton }

    private GameObject spawnedButton;

    private InputActionReference aInput, bInput;

    private void BButtonPressed(InputAction.CallbackContext obj)
    {
        if (playerInZone)
            onInteracted?.Invoke();
    }

    private void AButtonPressed(InputAction.CallbackContext obj)
    {
        if (playerInZone)
            onInteracted?.Invoke();
    }

    public void LoadScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            onEnteredZone?.Invoke();
            switch (interactButton)
            {
                case InteractButton.AButton:
                    spawnedButton = Instantiate(UIUXManager.instance.buttonA, uiButtonSpawnParent != null ? uiButtonSpawnParent : transform);
                    aInput = UIUXManager.instance.aInput;
                    aInput.action.performed += AButtonPressed;
                    Debug.Log(aInput);
                    break;

                case InteractButton.BButton:
                    spawnedButton = Instantiate(UIUXManager.instance.buttonB, uiButtonSpawnParent != null ? uiButtonSpawnParent : transform);
                    bInput = UIUXManager.instance.bInput;
                    bInput.action.performed += BButtonPressed;
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (spawnedButton != null)
                Destroy(spawnedButton);

            onLeftZone?.Invoke();

            switch (interactButton)
            {
                case InteractButton.AButton:
                    aInput = null;
                    break;

                case InteractButton.BButton:
                    bInput = null;
                    break;
            }
        }

    }
}
