using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class LavaBlock2D : MonoBehaviour
{
    [Header("Lava Physics")]
    [SerializeField] private float viscosity = 0.7f; // Higher = thicker lava
    [SerializeField] private float flowSpeed = 2f;
    [SerializeField] private float fallThreshold = 0.5f; // Slope angle to start falling
    [SerializeField] private float burnDamage = 10f;
    [SerializeField] private float burnInterval = 0.5f;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem lavaBubbles;
    [SerializeField] private ParticleSystem lavaSplash;
    [SerializeField] private Color glowColor = new Color(1f, 0.5f, 0.2f, 1f);
    [SerializeField] private float glowIntensity = 2f;

    private Rigidbody2D rb;
    private Collider2D col;
    private ContactFilter2D contactFilter;
    private RaycastHit2D[] groundHits = new RaycastHit2D[1];
    private float nextBurnTime;
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Material glowMaterial;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Configure physics
        rb.gravityScale = 1f;
        rb.linearDamping = viscosity * 5f;
        rb.angularDamping = viscosity * 10f;
        
        // Setup contact filter
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Ground"));
        contactFilter.useLayerMask = true;
        
        // Setup visual effects
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
            glowMaterial = new Material(Shader.Find("Sprites/Default"));
            glowMaterial.color = glowColor;
            UpdateGlowEffect(true);
        }
    }

    void Update()
    {
        CheckSliding();
        UpdateVisualEffects();
    }

    void FixedUpdate()
    {
        ApplyLavaPhysics();
    }

    void ApplyLavaPhysics()
    {
        // Make lava flow downward with viscosity
        if (rb.linearVelocity.magnitude < flowSpeed)
        {
            rb.AddForce(Vector2.down * (flowSpeed * 0.1f), ForceMode2D.Impulse);
        }
        
        // Limit maximum speed based on viscosity
        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, flowSpeed / viscosity);
    }

    void CheckSliding()
    {
        // Check if we're on a slope steep enough to slide
        int hitCount = col.Cast(Vector2.down, contactFilter, groundHits, 0.1f);
        
        if (hitCount > 0)
        {
            float slopeAngle = Vector2.Angle(groundHits[0].normal, Vector2.up);
            
            if (slopeAngle > fallThreshold)
            {
                // Apply sliding force based on slope angle
                Vector2 slideDirection = new Vector2(-groundHits[0].normal.y, groundHits[0].normal.x);
                rb.AddForce(slideDirection * (slopeAngle * 0.1f), ForceMode2D.Force);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Create splash effect
        if (lavaSplash != null)
        {
            ContactPoint2D contact = collision.contacts[0];
            Instantiate(lavaSplash, contact.point, Quaternion.identity);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Damage objects in contact with lava
        if (Time.time >= nextBurnTime)
        {
            Damageable damageable = other.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(burnDamage);
                nextBurnTime = Time.time + burnInterval;
            }
        }
    }

    void UpdateVisualEffects()
    {
        // Control bubble effects based on velocity
        if (lavaBubbles != null)
        {
            var emission = lavaBubbles.emission;
            emission.rateOverTime = rb.linearVelocity.magnitude * 5f;
            
            if (rb.linearVelocity.magnitude > 0.1f && !lavaBubbles.isPlaying)
            {
                lavaBubbles.Play();
            }
            else if (rb.linearVelocity.magnitude <= 0.1f && lavaBubbles.isPlaying)
            {
                lavaBubbles.Stop();
            }
        }
        
        // Update glow intensity based on movement
        if (spriteRenderer != null)
        {
            float glow = Mathf.Lerp(glowIntensity * 0.5f, glowIntensity, rb.linearVelocity.magnitude / flowSpeed);
            glowMaterial.SetFloat("_Glow", glow);
        }
    }

    void UpdateGlowEffect(bool enable)
    {
        if (spriteRenderer == null) return;
        
        spriteRenderer.material = enable ? glowMaterial : originalMaterial;
    }

    void OnDestroy()
    {
        if (spriteRenderer != null && glowMaterial != null)
        {
            Destroy(glowMaterial);
        }
    }
}