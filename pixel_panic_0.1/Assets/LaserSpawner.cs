using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserSpawner : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    [SerializeField] private Vector2 spawnAreaMin;
    [SerializeField] private Vector2 spawnAreaMax;
    [SerializeField] private float minDistanceBetweenLasers = 2f;
    [SerializeField] private int maxAttempts = 10;

    private List<GameObject> activeLasers = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnLasersRoutine());
    }

    private IEnumerator SpawnLasersRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

            Vector2 spawnPosition = GetBestAvailableSpawnPosition();
            GameObject newLaser = Instantiate(laserPrefab, spawnPosition, Quaternion.identity);
            activeLasers.Add(newLaser);
            
            // Clean up destroyed lasers
            activeLasers.RemoveAll(laser => laser == null);
        }
    }

    private Vector2 GetBestAvailableSpawnPosition()
    {
        Vector2 bestPosition = Vector2.zero;
        float bestDistance = 0f;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 testPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y));

            float currentMinDistance = GetMinimumDistanceToLasers(testPosition);

            // If we find a position that meets our requirements, use it immediately
            if (currentMinDistance >= minDistanceBetweenLasers)
            {
                return testPosition;
            }

            // Otherwise keep track of the best position we found
            if (currentMinDistance > bestDistance)
            {
                bestDistance = currentMinDistance;
                bestPosition = testPosition;
            }
        }

        // If we didn't find a perfect position, return the best one we found
        return bestPosition;
    }

    private float GetMinimumDistanceToLasers(Vector2 position)
    {
        if (activeLasers.Count == 0) return float.MaxValue;

        float minDistance = float.MaxValue;
        foreach (GameObject laser in activeLasers)
        {
            if (laser != null)
            {
                float distance = Vector2.Distance(position, laser.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
        }
        return minDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = (spawnAreaMin + spawnAreaMax) / 2;
        Vector3 size = new Vector3(
            spawnAreaMax.x - spawnAreaMin.x,
            spawnAreaMax.y - spawnAreaMin.y,
            0.1f
        );
        Gizmos.DrawWireCube(center, size);
    }
}