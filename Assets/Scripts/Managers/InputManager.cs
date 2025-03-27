using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    [HideInInspector] public Controls controls;

    // Event that other scripts can subscribe to
    public event System.Action OnButtonPressed;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        controls = new Controls();
        
        // Subscribe to the button press event
        controls.Player.Interact.performed += ctx => ButtonPressed();
    }

    private void ButtonPressed()
    {
        // Invoke the event when button is pressed
        OnButtonPressed?.Invoke();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}