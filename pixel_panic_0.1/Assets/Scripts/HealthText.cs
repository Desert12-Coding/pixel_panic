using UnityEngine;
using TMPro;

[RequireComponent(typeof(Health))] // Ensures Health component exists
public class HealthText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text healthText; // Assign in Inspector
    private Health healthScript;

    void Start()
    {
        // Get the Health script on the same GameObject
        healthScript = GetComponent<Health>();
        
        if (healthText == null)
        {
            Debug.LogError("HealthText: No TMP_Text assigned!");
            return;
        }

        // Initialize with current health
        UpdateHealthDisplay(healthScript.GetCurrentHealth() / healthScript.maxHealth);

        // Subscribe to health changes
        healthScript.onHealthChanged.AddListener(UpdateHealthDisplay);
    }

    private void UpdateHealthDisplay(float healthRatio)
    {
        // Update text (e.g., "75%" or "50/100 HP")
        healthText.text = $"{Mathf.RoundToInt(healthRatio * 100)}%";
    }

    void OnDestroy()
    {
        // Safely unsubscribe
        if (healthScript != null)
            healthScript.onHealthChanged.RemoveListener(UpdateHealthDisplay);
    }
}