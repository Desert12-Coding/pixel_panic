using UnityEngine;

public class DeathBox : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(other.gameObject);
    }
}