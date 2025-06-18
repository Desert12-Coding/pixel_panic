
using UnityEngine;

public class TimedSpriteAppearance : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Assign in inspector
    public float appearAfter = 20f;       // Time until sprite appears
    public float disappearAfter = 5f;     // Time sprite stays visible

    private float timer;
    private bool hasAppeared = false;

    void Start()
    {
        // Make sure sprite is hidden at start
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer not assigned in inspector!");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Make sprite appear after specified time
        if (!hasAppeared && timer >= appearAfter)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                hasAppeared = true;
                timer = 0f; // Reset timer for disappearance
            }
        }

        // Make sprite disappear after visible time
        if (hasAppeared && timer >= disappearAfter)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
                // Optional: disable script after completion if you only want this to happen once
                // this.enabled = false;
            }
        }
    }
}