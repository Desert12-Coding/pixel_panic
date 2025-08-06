using UnityEngine;

public class AdvancedPrefabSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnablePrefab
    {
        public GameObject prefab;
        [Range(0, 100)] 
        public float spawnWeight = 20f;
        public Color previewColor = Color.white;
    }

    [Header("Spawn Settings")]
    public SpawnablePrefab[] prefabs = new SpawnablePrefab[5];
    public float spawnRadius = 5f;
    public bool use2DSpace = false;

    [Header("Timed Spawning")]
    public bool spawnRepeatedly = true;
    public float initialDelay = 0f;
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;
    public int maxSpawnCount = 10; // 0 = infinite

    [Header("Collision Handling")]
    public bool avoidOverlaps = true;
    public float objectRadius = 0.5f;
    public int maxSpawnAttempts = 10;

    [Header("Debug")]
    public bool showSpawnRadius = true;
    public bool showPreview = true;
    private int spawnedCount = 0;

    void Start()
    {
        if (initialDelay > 0)
        {
            Invoke("StartSpawning", initialDelay);
        }
        else
        {
            StartSpawning();
        }
    }

    void StartSpawning()
    {
        if (spawnRepeatedly)
        {
            InvokeRepeating("SpawnWithWeights", 0f, GetNextInterval());
        }
        else
        {
            SpawnWithWeights();
        }
    }

    float GetNextInterval()
    {
        return Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    public void SpawnWithWeights()
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("No prefabs assigned!", this);
            return;
        }

        // Check spawn limit
        if (maxSpawnCount > 0 && spawnedCount >= maxSpawnCount)
        {
            CancelInvoke();
            return;
        }

        // Calculate total weight
        float totalWeight = 0;
        foreach (var item in prefabs)
        {
            if (item.prefab != null) totalWeight += item.spawnWeight;
        }

        if (totalWeight <= 0)
        {
            Debug.LogError("Total spawn weight must be > 0!", this);
            return;
        }

        // Weighted random selection
        float randomPoint = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        for (int i = 0; i < prefabs.Length; i++)
        {
            if (prefabs[i].prefab == null) continue;

            currentWeight += prefabs[i].spawnWeight;
            if (randomPoint <= currentWeight)
            {
                TrySpawnPrefab(prefabs[i].prefab);
                break;
            }
        }

        spawnedCount++;
        
        // Update interval if repeating
        if (spawnRepeatedly)
        {
            CancelInvoke();
            InvokeRepeating("SpawnWithWeights", GetNextInterval(), GetNextInterval());
        }
    }

    void TrySpawnPrefab(GameObject prefabToSpawn)
    {
        Vector3 spawnPosition = Vector3.zero;
        Quaternion spawnRotation = use2DSpace ? 
            Quaternion.Euler(0, 0, Random.Range(0f, 360f)) : 
            Random.rotation;

        bool positionFound = !avoidOverlaps; // Skip check if not avoiding overlaps

        for (int i = 0; i < maxSpawnAttempts && !positionFound; i++)
        {
            spawnPosition = GetRandomSpawnPosition();
            positionFound = !Physics.CheckSphere(spawnPosition, objectRadius);
        }

        if (positionFound)
        {
            Instantiate(prefabToSpawn, spawnPosition, spawnRotation);
        }
        else if (avoidOverlaps)
        {
            Debug.LogWarning("Failed to find valid position after " + maxSpawnAttempts + " attempts");
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (use2DSpace)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            return transform.position + new Vector3(randomCircle.x, randomCircle.y, 0);
        }
        return transform.position + Random.insideUnitSphere * spawnRadius;
    }

    void OnDrawGizmosSelected()
    {
        if (!showSpawnRadius) return;

        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        if (showPreview && prefabs != null)
        {
            foreach (var item in prefabs)
            {
                if (item.prefab == null) continue;
                
                Gizmos.color = item.previewColor;
                Gizmos.DrawWireCube(transform.position, Vector3.one * 0.3f);
            }
        }

        if (avoidOverlaps)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, objectRadius);
        }
    }

    [ContextMenu("Spawn Now")]
    public void SpawnNow()
    {
        SpawnWithWeights();
    }

    [ContextMenu("Stop Spawning")]
    public void StopSpawning()
    {
        CancelInvoke();
    }
}