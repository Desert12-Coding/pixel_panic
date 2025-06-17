using UnityEngine;
using UnityEngine.InputSystem; // Required for new Input System (optional)

public class Freeze : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How many key presses are needed to unfreeze")]
    public int requiredPresses = 6;

    [Tooltip("Show debug messages")]
    public bool debugMessages = true;

    [Header("UI (Optional)")]
    [Tooltip("Drag a TextMeshProUGUI or Text UI element here to show count")]
    public UnityEngine.UI.Text pressCounterText;

    private int currentPressCount = 0;
    private Rigidbody rb; // For 3D
    private Rigidbody2D rb2D; // For 2D
    private bool isFrozen = true;

    private void Start()
    {
        // Get Rigidbody (3D or 2D)
        rb = GetComponent<Rigidbody>();
        rb2D = GetComponent<Rigidbody2D>();

        FreezePlayer(true); // Freeze at start
    }

    private void Update()
    {
        if (isFrozen && Keyboard.current.anyKey.wasPressedThisFrame) // Works with any key
        {
            currentPressCount++;
            UpdateCounterUI();

            if (debugMessages)
                Debug.Log($"Key pressed! ({currentPressCount}/{requiredPresses})");

            if (currentPressCount >= requiredPresses)
            {
                FreezePlayer(false); // Unfreeze
                if (debugMessages)
                    Debug.Log("Player unfrozen!");
            }
        }
    }

    private void FreezePlayer(bool freeze)
    {
        isFrozen = freeze;

        // Freeze/unfreeze Rigidbody (3D)
        if (rb != null)
        {
            rb.isKinematic = freeze;
            rb.linearVelocity = Vector3.zero;
        }

        // Freeze/unfreeze Rigidbody2D (2D)
        if (rb2D != null)
        {
            rb2D.simulated = !freeze;
            rb2D.linearVelocity = Vector2.zero;
        }
    }

    private void UpdateCounterUI()
    {
        if (pressCounterText != null)
            pressCounterText.text = $"Press any key: {currentPressCount}/{requiredPresses}";
    }
}