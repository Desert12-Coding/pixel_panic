using UnityEngine;
using System.Linq; // Required for Sum()

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

    [Header("Difficulty Scaling")]
    public bool scaleOverTime = true;
    public float timeToMaxDifficulty = 300f; // 5 minutes
    public float minIntervalAtMaxDifficulty = 0.3f;
    public AnimationCurve difficultyCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(1, 1)
    );

    [Header("Collision Handling")]
    public bool avoidOverlaps = true;
    public float objectRadius = 0.5f;
    public int maxSpawnAttempts = 10;

    [Header("Debug")]
    public bool showSpawnRadius = true;
    public bool showPreview = true;
    
    private int spawnedCount = 0;
    private float elapsedTime = 0f;
    private float initialMinInterval;
    private float initialMaxInterval;

    void Start()
    {
        initialMinInterval = minSpawnInterval;
        initialMaxInterval = maxSpawnInterval;

        if (initialDelay > 0)
        {
            Invoke("StartSpawning", initialDelay);
        }
        else
        {
            StartSpawning();
        }
    }

    void Update()
    {
        if (scaleOverTime)
        {
            elapsedTime += Time.deltaTime;
            UpdateSpawnIntervals();
        }
    }

    void UpdateSpawnIntervals()
    {
        float progress = Mathf.Clamp01(elapsedTime / timeToMaxDifficulty);
        float curveValue = difficultyCurve.Evaluate(progress);

        minSpawnInterval = Mathf.Lerp(
            initialMinInterval, 
            minIntervalAtMaxDifficulty, 
            curveValue
        );

        maxSpawnInterval = Mathf.Lerp(
            initialMaxInterval,
            minIntervalAtMaxDifficulty * 1.5f,
            curveValue
        );
    }

    void StartSpawning()
    {
        if (spawnRepeatedly)
        {
            SpawnWithWeights(); // Immediate first spawn
            Invoke("ScheduleNextSpawn", GetNextInterval());
        }
        else
        {
            SpawnWithWeights();
        }
    }

    void ScheduleNextSpawn()
    {
        if (spawnRepeatedly && (maxSpawnCount == 0 || spawnedCount < maxSpawnCount))
        {
            SpawnWithWeights();
            Invoke("ScheduleNextSpawn", GetNextInterval());
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
        float totalWeight = prefabs.Sum(item => item.spawnWeight);

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
    }

    // === MISSING METHOD ADDED BELOW ===
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