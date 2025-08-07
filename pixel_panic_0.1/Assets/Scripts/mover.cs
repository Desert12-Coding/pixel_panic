using UnityEngine;

public class mover : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 1f;
    [SerializeField] private float startDelay = 0f; // Delay in seconds before movement starts
    
    private float delayTimer = 0f;
    private bool isMoving = false;

    void Update()
    {
        // Handle the delay timer
        if (!isMoving)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer >= startDelay)
            {
                isMoving = true;
            }
            return;
        }
        
        // Movement code
        transform.position += (Vector3)(new Vector2(1f, 0) * MoveSpeed * Time.deltaTime);
    }
}