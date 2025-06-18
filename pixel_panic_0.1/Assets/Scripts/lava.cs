using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class FallingLava : MonoBehaviour
{
    [Header("Lava Properties")]
    [Range(0.1f, 10f)] public float viscosity = 2f; // Higher = thicker lava
    public float flowSpeed = 3f;
    public Color lavaColor = new Color(1f, 0.3f, 0f, 1f);
    public float damagePerSecond = 20f;

    [Header("Collision Settings")]
    public LayerMask groundLayer; // Set in Inspector
    public string groundTag = "Ground"; // Alternative to layers

    [Header("Effects")]
    public ParticleSystem lavaDrips;
    public Light pointLight; // Optional glow effect

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float originalGravityScale;

    void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize lava properties
        originalGravityScale = rb.gravityScale;
        rb.linearDamping = viscosity;
        spriteRenderer.color = lavaColor;

        // Set up visual effects
        if (pointLight != null)
        {
            pointLight.color = lavaColor;
            pointLight.intensity = 1f;
        }

        // Start falling immediately
        StartFalling();
    }

    void Update()
    {
        // Control lava drip particles based on movement
        if (lavaDrips != null)
        {
            var emission = lavaDrips.emission;
            emission.rateOverTime = rb.linearVelocity.magnitude * 10f;
        }
    }

    void FixedUpdate()
    {
        // Make lava flow downward
        if (rb.linearVelocity.magnitude < flowSpeed)
        {
            rb.AddForce(Vector2.down * (flowSpeed * 0.5f));
        }
    }

    public void StartFalling()
    {
        rb.gravityScale = originalGravityScale;
        rb.isKinematic = false;
        
        if (lavaDrips != null)
        {
            lavaDrips.Play();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Only process if colliding with ground
        if (!IsGround(collision.gameObject))
            return;

        // Spread when hitting ground surfaces
        if (collision.relativeVelocity.magnitude > 2f)
        {
            TrySpreadLava(collision.contacts[0].point);
        }

        // Impact effects only on ground
        if (pointLight != null)
        {
            pointLight.intensity = 2f; // Brighten on impact
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Only process if colliding with ground
        if (!IsGround(collision.gameObject))
            return;

        // Damage ground objects over time
        Health health = collision.gameObject.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Damage any objects in trigger contact
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    bool IsGround(GameObject obj)
    {
        // Check if object is on ground layer OR has ground tag
        return (groundLayer == (groundLayer | (1 << obj.layer))) || 
               obj.CompareTag(groundTag);
    }

    void TrySpreadLava(Vector2 contactPoint)
    {
        // Simple spread effect
        if (Random.value > 0.7f) // 0% chance to spread
        {
            Vector2 spreadPos = contactPoint + Random.insideUnitCircle * 0.5f;
            Instantiate(this, spreadPos, Quaternion.identity);
        }
    }
}