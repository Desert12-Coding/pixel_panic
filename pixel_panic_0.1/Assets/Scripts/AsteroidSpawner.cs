using UnityEngine;

public class StaticPlatformSpawner : MonoBehaviour
{
    [Header("Spawn Area Settings")]
    [SerializeField] private Vector2 spawnAreaCenter = Vector2.zero;
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 5f);
    [SerializeField] private Color gizmoColor = new Color(0, 1, 0, 0.3f);
    [SerializeField] private bool showSpawnArea = true;

    [Header("Platform Settings")]
    [SerializeField] private GameObject[] platformPrefabs;
    [SerializeField] private int numberOfPlatforms = 10;
    [SerializeField] private float minDistanceBetween = 1f;

    private void Start()
    {
        SpawnAllPlatforms();
    }

    private void SpawnAllPlatforms()
    {
        if (platformPrefabs.Length == 0)
        {
            Debug.LogError("No platform prefabs assigned!");
            return;
        }

        Vector2 halfSize = spawnAreaSize * 0.5f;
        Vector2 minBounds = spawnAreaCenter - halfSize;
        Vector2 maxBounds = spawnAreaCenter + halfSize;

        for (int i = 0; i < numberOfPlatforms; i++)
        {
            Vector2 spawnPos;
            bool positionValid;
            int attempts = 0;
            const int maxAttempts = 100;

            // Try to find a valid position
            do
            {
                spawnPos = new Vector2(
                    Random.Range(minBounds.x, maxBounds.x),
                    Random.Range(minBounds.y, maxBounds.y)
                );

                positionValid = IsPositionValid(spawnPos);
                attempts++;
            } 
            while (!positionValid && attempts < maxAttempts);

            if (positionValid)
            {
                GameObject platform = Instantiate(
                    platformPrefabs[Random.Range(0, platformPrefabs.Length)],
                    spawnPos,
                    Quaternion.identity
                );

                // Optional random rotation
                if (platform.TryGetComponent<Rigidbody2D>(out var rb))
                {
                    rb.angularVelocity = Random.Range(-5f, 5f);
                }
            }
            else
            {
                Debug.LogWarning($"Failed to find valid position for platform {i}");
            }
        }
    }

    private bool IsPositionValid(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, minDistanceBetween);
        return colliders.Length == 0; // Position is valid if no colliders nearby
    }

    private void OnDrawGizmos()
    {
        if (!showSpawnArea) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
        Gizmos.DrawCube(spawnAreaCenter, spawnAreaSize);
    }
}