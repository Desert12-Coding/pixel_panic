using UnityEngine;
using System.Collections;

public class LaserSpawner : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private float minSpawnDelay = 1f;
    [SerializeField] private float maxSpawnDelay = 3f;
    [SerializeField] private Vector2 spawnAreaMin;
    [SerializeField] private Vector2 spawnAreaMax;

    private void Start()
    {
        StartCoroutine(SpawnLasersRoutine());
    }

    private IEnumerator SpawnLasersRoutine()
    {
        while (true)
        {
            // Wait for random time between min and max delay
            float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(waitTime);

            // Generate random position within spawn area
            Vector2 spawnPosition = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            // Instantiate the laser
            Instantiate(laserPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Optional: Draw the spawn area in the editor for visualization
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