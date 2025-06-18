using UnityEngine;
using UnityEngine.InputSystem;

public class Freeze : MonoBehaviour
{
    [Header("Freeze Settings")]
    public int requiredPresses = 6;
    public float freezeInterval = 10f;
    public bool debugMessages = true;

    [Header("Visual Feedback")]
    public Color frozenColor = new Color(0.6f, 0.9f, 1f, 1f); // Icy blue
    public float colorLerpSpeed = 5f;

    [Header("UI (Optional)")]
    public UnityEngine.UI.Text pressCounterText;

    // Components
    private int currentPressCount = 0;
    private Rigidbody rb;
    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Color originalColor;
    private bool isFrozen = false;
    private float timer = 0f;
    private PlayerInput playerInput; // Input System reference
    private InputActionAsset originalInputActions; // Store original inputs

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb2D = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>(); // Get the Input System component

        // Store original input actions
        if (playerInput != null)
        {
            originalInputActions = playerInput.actions;
        }

        // Visual setup
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        else if (TryGetComponent<Renderer>(out var renderer))
        {
            originalMaterial = renderer.material;
            originalColor = originalMaterial.color;
        }

        FreezePlayer(true); // Start frozen
    }

    private void Update()
    {
        // Auto-freeze timer
        timer += Time.deltaTime;
        if (timer >= freezeInterval && !isFrozen)
        {
            FreezePlayer(true);
            if (debugMessages) Debug.Log("Auto-freeze triggered!");
            timer = 0f;
        }

        // Unfreeze on key presses - works even when input is disabled
        if (isFrozen && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            currentPressCount++;
            UpdateCounterUI();
            if (currentPressCount >= requiredPresses) FreezePlayer(false);
        }

        UpdateVisualFeedback();
        
        // Keep velocity at zero while frozen
        if (isFrozen)
        {
            if (rb != null) rb.linearVelocity = Vector3.zero;
            if (rb2D != null) rb2D.linearVelocity = Vector2.zero;
        }
    }

    private void FreezePlayer(bool freeze)
    {
        isFrozen = freeze;
        
        if (freeze)
        {
            currentPressCount = 0;
            timer = 0f;
            
            // Disable all input actions
            if (playerInput != null)
            {
                playerInput.actions = null; // This completely disables input
            }
        }
        else
        {
            // Re-enable input actions
            if (playerInput != null && originalInputActions != null)
            {
                playerInput.actions = originalInputActions;
            }
        }
        
        UpdateCounterUI();
    }

    private void UpdateVisualFeedback()
    {
        Color targetColor = isFrozen ? frozenColor : originalColor;
        if (spriteRenderer != null)
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, Time.deltaTime * colorLerpSpeed);
        else if (originalMaterial != null)
            originalMaterial.color = Color.Lerp(originalMaterial.color, targetColor, Time.deltaTime * colorLerpSpeed);
    }

    private void UpdateCounterUI()
    {
        if (pressCounterText != null)
            pressCounterText.text = isFrozen 
                ? $"Press any key: {currentPressCount}/{requiredPresses}" 
                : "Freeze cooldown...";
    }

    private void OnDestroy()
    {
        if (originalMaterial != null) 
            originalMaterial.color = originalColor;
            
        // Restore input actions if destroyed while frozen
        if (isFrozen && playerInput != null && originalInputActions != null)
        {
            playerInput.actions = originalInputActions;
        }
    }
}