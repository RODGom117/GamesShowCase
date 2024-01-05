using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager instance;
    [SerializeField] public Transform[] spawns;
    private Transform playerSpawn;
    [SerializeField] private GameObject playerPrefab;
    private GameObject playerToChange;
    [SerializeField] PhotonSpawnPlayer photonSpawn;

    void Awake() {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public Transform SetSpawnPoint(int HeroID)
    {
        Transform newSpawn = gameObject.transform;

        if(HeroID == 0)
        {
            Debug.Log("Your character is Maira");
            newSpawn = spawns[0];
        }
        if(HeroID == 1)
        {
            Debug.Log("Your character is Teo");
            newSpawn = spawns[1];
        }
        if(HeroID == 2)
        {
            Debug.Log("Your character is Ignacio");
            newSpawn = spawns[2];
        }
        if(HeroID == 3)
        {
            Debug.Log("Your character is Rosa");
            newSpawn = spawns[3];
        }

        return newSpawn;
    }

    public void SpawnPlayer(int ID)
    {
        Debug.Log("SPAWN THIS");
        playerSpawn = SetSpawnPoint(ID);
        photonSpawn.SpawnPlayer(ID, playerSpawn);
        //Instantiate(playerPrefab, playerSpawn.position, Quaternion.Euler(playerSpawn.rotation.x, playerSpawn.rotation.y, playerSpawn.rotation.z));
        //playerToChange = GameObject.Find(player);
        //playerToChange.transform.position = playerSpawn.position;
        //playerToChange.transform.rotation = Quaternion.Euler(playerSpawn.rotation.x, playerSpawn.rotation.y, playerSpawn.rotation.z);
    }


}
