using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DamageDealer : MonoBehaviour
{
    [Header("Basic Damage Settings")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private bool destroyOnHit = false;
    [SerializeField] private bool damageOnTrigger = true;
    [SerializeField] private bool damageOnCollision = false;

    [Header("Damage Over Time (DPS)")]
    [SerializeField] private bool applyDPS = false; // Enable/disable DPS
    [SerializeField] private float dpsAmount = 5f; // Damage per second
    [SerializeField] private float dpsDuration = 3f; // How long DPS lasts
    [SerializeField] private float dpsInterval = 0.5f; // Time between damage ticks

    [Header("Target Settings")]
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private bool damagePlayer = true;
    [SerializeField] private bool damageEnemies = true;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damageOnTrigger)
            TryDealDamage(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (damageOnCollision)
            TryDealDamage(collision.gameObject);
    }

    private void TryDealDamage(GameObject target)
    {
        if (!IsInTargetLayer(target) || !ShouldDamageTarget(target))
            return;

        Health health = target.GetComponent<Health>();
        if (health != null)
        {
            // Apply initial damage
            health.TakeDamage(damageAmount);

            // Apply DPS if enabled
            if (applyDPS)
            {
                StartCoroutine(ApplyDamageOverTime(health));
            }

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            if (destroyOnHit)
                Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator ApplyDamageOverTime(Health health)
    {
        float timer = 0f;
        float nextDamageTime = 0f;

        while (timer < dpsDuration)
        {
            if (Time.time >= nextDamageTime)
            {
                health.TakeDamage(dpsAmount * dpsInterval); // Scale damage by interval
                nextDamageTime = Time.time + dpsInterval;
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

    private bool IsInTargetLayer(GameObject target)
    {
        return (targetLayers.value & (1 << target.layer)) != 0;
    }

    private bool ShouldDamageTarget(GameObject target)
    {
        bool isPlayer = target.CompareTag("Player");
        bool isEnemy = target.CompareTag("Enemy");

        if (isPlayer && !damagePlayer) return false;
        if (isEnemy && !damageEnemies) return false;

        return true;
    }
}