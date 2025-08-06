using UnityEngine;

[RequireComponent(typeof(Collider2D)), RequireComponent(typeof(Rigidbody2D))]
public class SnowballCollision : MonoBehaviour
{
    [Header("Impact Settings")]
    public float minGlanceForce = 3f;
    public float maxGlanceForce = 8f;
    public float upwardBounceForce = 5f;

    [Header("Angle Thresholds")]
    [Range(0, 90)] public float headAngleThreshold = 30f; // Degrees from vertical to count as "head hit"

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            HandlePlayerCollision(collision);
        }
    }

    private void HandlePlayerCollision(Collision2D collision)
    {
        // Calculate collision angle (0 = direct top, 90 = direct side)
        float collisionAngle = Vector2.Angle(collision.contacts[0].normal, Vector2.up);

        // If hitting the player's head (from above)
        if (collisionAngle < headAngleThreshold)
        {
            GlanceOffPlayer(collision);
        }
        else // Standard side hit
        {
            ApplyKnockback(collision.collider, collision.contacts[0].point);
        }
    }

    private void GlanceOffPlayer(Collision2D collision)
    {
        // Get random left/right direction
        float direction = Random.value > 0.5f ? 1f : -1f;
        
        // Apply angled force
        Vector2 glanceForce = new Vector2(
            direction * Random.Range(minGlanceForce, maxGlanceForce),
            upwardBounceForce
        );

        GetComponent<Rigidbody2D>().AddForce(glanceForce, ForceMode2D.Impulse);
        
        // Optional: Make snowball non-collidable briefly
        StartCoroutine(DisableCollisionBriefly(0.2f));
        
       
    }

    private void ApplyKnockback(Collider2D player, Vector2 contactPoint)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            Vector2 knockbackDir = ((Vector2)player.transform.position - contactPoint).normalized;
            playerRb.AddForce(knockbackDir * minGlanceForce, ForceMode2D.Impulse);
        }
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator DisableCollisionBriefly(float time)
    {
        var collider = GetComponent<Collider2D>();
        collider.enabled = false;
        yield return new WaitForSeconds(time);
        collider.enabled = true;
    }
}