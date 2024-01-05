using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyStats> enemyTypes; // List of different enemy types
    public Transform[] spawnPoints;     // Points where enemies can spawn NOTE: Implement Humberto's spawn point system
    public GameObject enemyPrefab;      // Reference to a generic enemy prefab

    public float spawnInterval = 5f;    // Time interval between enemy spawns NOTE: Implement Humberto's spawn point system
    public int maxEnemies = 10;         // Maximum number of enemies allowed

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float nextSpawnTime = 0f;  // Time of next enemy spawn NOTE: Implement Humberto's spawn point system

    void Update()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        if (Time.time >= nextSpawnTime && activeEnemies.Count < maxEnemies)
        {
            // Choose a random enemy type from the list
            EnemyStats randomEnemyType = enemyTypes[Random.Range(0, enemyTypes.Count)];

            // Choose a random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instantiate the enemy using the prefab from EnemyAttributes
            GameObject newEnemy = Instantiate(randomEnemyType.baseAttributes.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            activeEnemies.Add(newEnemy);

            // Set the next spawn time
            nextSpawnTime = Time.time + spawnInterval;
        }
    }


    // Call this method to remove an enemy from the activeEnemies list
    public void RemoveEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Destroy(enemy);
        }
    }
}
