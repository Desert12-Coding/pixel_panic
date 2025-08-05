using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float deathDelay = 0f;

    [Header("Death Settings")]
    [SerializeField] private bool changeSceneOnDeath = false;

    [Header("Events")]
    public UnityEvent onDamageTaken;
    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChanged; // Passes current health ratio (0-1)

    private float currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthEvents();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthEvents();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthEvents();
    }

    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        onDeath.Invoke();

        if (destroyOnDeath)
        {
            Destroy(gameObject, deathDelay);
        }

        if (changeSceneOnDeath)
        {
            SceneManager.LoadScene("StartScreen");
        }
    }

    private void UpdateHealthEvents()
    {
        onHealthChanged.Invoke(currentHealth / maxHealth);
        if (currentHealth < maxHealth)
        {
            onDamageTaken.Invoke();
        }
    }

    // For debugging
    public void DebugTakeDamage(float damage) => TakeDamage(damage);
    public void DebugKill() => TakeDamage(maxHealth);
    public float GetCurrentHealth() => currentHealth;
}