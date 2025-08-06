using UnityEngine;

[RequireComponent(typeof(Collider2D)), RequireComponent(typeof(Rigidbody2D))] // 2D components
public class KnockbackOnContact : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForce = 10f;
    public float upwardForce = 2f; 
    public bool destroyOnContact = false;

    private void OnCollisionEnter2D(Collision2D collision) // Only need this for 2D
    {
        TryKnockback(collision.collider, collision.contacts[0].point);
    }

    private void TryKnockback(Collider2D other, Vector2 contactPoint) // Changed to Vector2
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>(); // Get Rigidbody2D
            if (playerRb != null)
            {
                // Calculate direction (convert contactPoint to Vector2 for 2D)
                Vector2 knockbackDir = ((Vector2)other.transform.position - contactPoint).normalized;
                knockbackDir.y += upwardForce; // Add upward force

                // Apply force (ForceMode2D.Impulse for instant force)
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

                if (destroyOnContact)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}