using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    [SerializeField] private Transform[] chestSpawnPoints;
    [SerializeField] private GameObject chest;
    [SerializeField] private int chestAmount;
    [SerializeField] private float radioSpawn;
    private Vector3 prevChest = new Vector3(0,0,0);
    private Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
       foreach(Transform spawnPoint in chestSpawnPoints)
        {
            for (int i = 0; i < chestAmount; i++)
            {
                Vector3 randomPosition = Random.insideUnitSphere * radioSpawn;
                while((Mathf.Abs(randomPosition.x - prevChest.x) < 5) && (Mathf.Abs(randomPosition.y - prevChest.y) < 5))
                {
                    randomPosition = Random.insideUnitSphere * radioSpawn;
                }
                randomPosition.y = 0f;
                direction = spawnPoint.position - randomPosition;
                Instantiate(chest, spawnPoint.position + randomPosition, Quaternion.LookRotation(direction));
                prevChest = randomPosition;
            }
        } 
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        for(int i = 0; i < chestSpawnPoints.Length; i++)
        {
            Gizmos.DrawWireSphere(chestSpawnPoints[i].position, radioSpawn);
        }
    }

}
