using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class deathscript : MonoBehaviour
{
    // Called when another collider enters this trigger zone
    private void OnTriggerEnter(Collider other) // Use Collider2D for 2D games
    {
        Debug.Log("Triggered by: " + other.name);
        // Add your trigger logic here (e.g., destroy, play sound, etc.)
        if (other.CompareTag("Player"))
        {
            Destroy(other.gameObject);
        }
    }
}