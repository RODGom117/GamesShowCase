using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemigos;
    [SerializeField] private int cantidadEnemigos = 5;
    [SerializeField] private float radioSpawn = 3f;
    [SerializeField] private float radioTrigger = 4f;
    [SerializeField] private bool active = true;
    [SerializeField] private bool playerInsideTrigger = false;
    [SerializeField] private PhotonView photonview;

    private SphereCollider trigger;

    void Start()
    {
        trigger = GetComponent<SphereCollider>();
        trigger.radius = radioTrigger;
        trigger.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(photonview.IsMine)
        {
            if (playerInsideTrigger && active)
            {
                SpawnEnemies();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Hero"))
        {
            playerInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Hero"))
        {
            playerInsideTrigger = false;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioSpawn);
        Gizmos.color = new Color(0.8f,0.5f,0,1);
        Gizmos.DrawWireSphere(transform.position, radioTrigger);
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < cantidadEnemigos; i++)
        {
            // Calculate a random position within the spawn radius
            Vector3 randomPosition = Random.insideUnitSphere * radioSpawn;
            randomPosition.y = 1f; // Spawn level

            // Instantiate a random enemy from the array at the calculated position
            int randomEnemyIndex = Random.Range(0, enemigos.Length);
            GameObject enemyPrefab = enemigos[randomEnemyIndex];
            string enemyPath = "Enemies/" + enemyPrefab.name;
            GameObject instance = PhotonNetwork.Instantiate(enemyPath, transform.position + randomPosition,Quaternion.identity) as GameObject;
            
            //Instantiate(enemyPrefab, transform.position + randomPosition, Quaternion.identity);
            
        }
        active = false;
    }
}
