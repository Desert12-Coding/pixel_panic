using UnityEngine;

public class OneTimeBreakableIce : MonoBehaviour
{
    public ParticleSystem breakEffect;
    public AudioClip breakSound;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BreakPlatform();
        }
    }

    private void BreakPlatform()
    {
        // Play effects
        if (breakEffect != null) Instantiate(breakEffect, transform.position, Quaternion.identity);
        if (breakSound != null) AudioSource.PlayClipAtPoint(breakSound, transform.position);
        
        // Disable platform
        gameObject.SetActive(false);
        
        // Alternatively, destroy completely:
        // Destroy(gameObject);
    }
}