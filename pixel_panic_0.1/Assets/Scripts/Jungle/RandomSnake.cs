using UnityEngine;
using System.Collections.Generic;

public class RandomSnake : MonoBehaviour
{
    [Header("Snake Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 90f;
    public float directionChangeInterval = 2f;
    public int segmentCount = 5;
    public float segmentDistance = 0.5f;
    public GameObject segmentPrefab;

    [Header("Boundary Settings")]
    public Vector2 minBoundary = new Vector2(-5, -5);
    public Vector2 maxBoundary = new Vector2(5, 5);
    public float borderPadding = 0.5f;

    private List<Transform> segments = new List<Transform>();
    private Vector2 currentDirection;
    private float timeSinceLastDirectionChange;

    void Start()
    {
        // Initialize snake head
        segments.Add(transform);
        
        // Create initial body segments
        for (int i = 1; i < segmentCount; i++)
        {
            AddSegment();
        }

        // Set random initial direction
        currentDirection = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        UpdateDirection();
        MoveSnake();
        UpdateSegments();
        ConstrainToBoundary();
    }

    void UpdateDirection()
    {
        timeSinceLastDirectionChange += Time.deltaTime;
        
        Vector2 pos = transform.position;
        bool nearBorder = 
            pos.x <= minBoundary.x + borderPadding || 
            pos.x >= maxBoundary.x - borderPadding ||
            pos.y <= minBoundary.y + borderPadding || 
            pos.y >= maxBoundary.y - borderPadding;
        
        if (timeSinceLastDirectionChange >= directionChangeInterval || nearBorder)
        {
            if (nearBorder)
            {
                // Bounce away from border
                Vector2 newDirection = currentDirection;
                
                if (pos.x <= minBoundary.x + borderPadding || pos.x >= maxBoundary.x - borderPadding)
                    newDirection.x *= -1;
                if (pos.y <= minBoundary.y + borderPadding || pos.y >= maxBoundary.y - borderPadding)
                    newDirection.y *= -1;
                    
                currentDirection = newDirection.normalized;
            }
            else
            {
                // Random direction change
                float angleChange = Random.Range(-45f, 45f);
                currentDirection = Quaternion.Euler(0, 0, angleChange) * currentDirection;
            }
            
            timeSinceLastDirectionChange = 0f;
        }
    }

    void ConstrainToBoundary()
    {
        // Keep snake within bounds
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBoundary.x, maxBoundary.x),
            Mathf.Clamp(transform.position.y, minBoundary.y, maxBoundary.y),
            transform.position.z
        );
    }

    void MoveSnake()
    {
        transform.position += (Vector3)currentDirection * moveSpeed * Time.deltaTime;
        
        float targetAngle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(0, 0, targetAngle),
            rotationSpeed * Time.deltaTime
        );
    }

    void UpdateSegments()
    {
        for (int i = 1; i < segments.Count; i++)
        {
            Transform previous = segments[i - 1];
            Transform current = segments[i];
            
            Vector2 targetPosition = previous.position - (Vector3)previous.right * segmentDistance;
            
            current.position = Vector2.Lerp(
                current.position,
                targetPosition,
                Time.deltaTime * moveSpeed * 2f
            );
            
            current.rotation = Quaternion.Slerp(
                current.rotation,
                previous.rotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    void AddSegment()
    {
        if (segmentPrefab != null && segments.Count > 0)
        {
            Transform lastSegment = segments[segments.Count - 1];
            GameObject newSegment = Instantiate(
                segmentPrefab,
                lastSegment.position - (Vector3)lastSegment.right * segmentDistance,
                lastSegment.rotation
            );
            
            segments.Add(newSegment.transform);
        }
    }

    // Visualize boundaries in editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = (minBoundary + maxBoundary) / 2f;
        Vector3 size = maxBoundary - minBoundary;
        Gizmos.DrawWireCube(center, size);
    }
}