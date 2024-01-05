using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[DisallowMultipleComponent]
public class PhotonSpawnPlayer : MonoBehaviour
{
    [SerializeField] private GameObject rosaPrefab;
    [SerializeField] private GameObject teoPrefab;
    [SerializeField] private GameObject ignacioPrefab;
    [SerializeField] private GameObject mairaPrefab;
    private PhotonSpawnPlayer instance;
    [SerializeField] private string playerPrefabPath;
    [SerializeField] private GameObject player;
    [SerializeField] private PhotonMatchManager photonMatchManager;


    public PhotonSpawnPlayer Instance
    {
        get
        {
            if(instance == null)
            {
            instance = this;
            }
            return instance;
        }
    }

    void Start() {
        StartCoroutine(InitialSpawn());
    }

    private IEnumerator InitialSpawn()
    {
        yield return new WaitForSeconds(1f);
        if (PhotonNetwork.IsConnected &&
             FindObjectOfType<PhotonMatchManager>().gameState != PhotonMatchManager.GameStates.EndGame &&
             player == null)
        {
            Debug.Log("NOW >:C");
            photonMatchManager.NewPlayerSent(PhotonNetwork.NickName);
        }
    }

    public void SpawnPlayer(int ID, Transform spawnPos)
    {
        if(ID == 0)
        {
            playerPrefabPath = "Heroes/" + mairaPrefab.name;
            
            player = PhotonNetwork.Instantiate(playerPrefabPath, spawnPos.position, Quaternion.Euler(spawnPos.rotation.x, spawnPos.rotation.y, spawnPos.rotation.z));
            player.name = "Maira";
        }
        else if(ID == 1)
        {
            playerPrefabPath = "Heroes/" + teoPrefab.name;
            
            player = PhotonNetwork.Instantiate(playerPrefabPath, spawnPos.position, Quaternion.Euler(spawnPos.rotation.x, spawnPos.rotation.y, spawnPos.rotation.z));
            player.name = "Teo";
        }
        if(ID == 2)
        {
            playerPrefabPath = "Heroes/" + ignacioPrefab.name;
            
            player = PhotonNetwork.Instantiate(playerPrefabPath, spawnPos.position, Quaternion.Euler(spawnPos.rotation.x, spawnPos.rotation.y, spawnPos.rotation.z));
            player.name = "Ignacio";
        }
        if(ID == 3)
        {
            playerPrefabPath = "Heroes/" + rosaPrefab.name;
            
            player = PhotonNetwork.Instantiate(playerPrefabPath, spawnPos.position, Quaternion.Euler(spawnPos.rotation.x, spawnPos.rotation.y, spawnPos.rotation.z));
            player.name = "Rosa";

        }
    }




    // [SerializeField] private GameObject playerPrefab;
    // [SerializeField] private GameObject playerDeath;
    // private PhotonSpawnPlayer instance;
    // [SerializeField] private string playerPrefabPath;
    // [SerializeField] private GameObject player;
    // private HealthSystem _playerHealth;
    // [SerializeField] private PhotonMatchManager photonMatchManager;

    // public PhotonSpawnPlayer Instance
    // {
    //     get
    //     {
    //         if(instance == null)
    //         {
    //             instance = this;
    //         }
    //         return instance;
    //     }
    // }

    // public void Start() 
    // {
    //     StartCoroutine(DelayedSpawn());
    // }

    // private IEnumerator DelayedSpawn()
    // {
    //     yield return new WaitForSeconds(1f);
    //     if (PhotonNetwork.IsConnected &&
    //         FindObjectOfType<PhotonMatchManager>().gameState != PhotonMatchManager.GameStates.EndGame &&
    //         player == null)
    //     {
    //         SpawnPlayer();
    //         photonMatchManager.NewPlayerSent(PhotonNetwork.NickName);
    //         // UIController.Instance.leaderboardManager.HideEndScreen();
    //     }
    // }

    // private void SpawnPlayer()
    // {

    //     playerPrefabPath = "Students/" + playerPrefab.name;
        
    //     player = PhotonNetwork.Instantiate(playerPrefabPath, new Vector3(0,0,0), Quaternion.identity);
    //     player.tag = "Player";

    //     _playerHealth = player.GetComponent<HealthSystem>();
    //     // _playerHealth.RestoreHealtAfterRespawn();
    //     // _playerHealth.onPlayerDeath += DestroyPLayer;
    // }

    // private void DestroyPLayer(string killerName)
    // {
    //     Debug.Log($"You have been killed by {killerName}");
    //     // UIController.Instance.ShowDeathScreen(killerName);
    //     playerPrefabPath = "Students/" + playerDeath.name;
    //     Vector3 deathEffectPosition = player.transform.position;
    //     PhotonNetwork.Instantiate(playerPrefabPath, deathEffectPosition, Quaternion.identity);
    //     // _playerHealth.onPlayerDeath -= DestroyPLayer;
        
    //     PhotonNetwork.Destroy(player);

    //     //StartCoroutine(DelayedSpawn());
            
    //     // ID de quien me mato
    //     int actorID = photonMatchManager.GetActorID(killerName);
    //     // Actualizar estatus de quien me mato
    //     photonMatchManager.ChangeStatSent(actorID, 0, 1);
    //     photonMatchManager.ChangeStatSent(actorID, 2, 100);
            
    //     // Actualizar mi conteo de muerte
    //     photonMatchManager.ChangeStatSent(PhotonNetwork.LocalPlayer.ActorNumber, 1, 1);
    //     player = null;
    // }

    // public void ReSpawn()
    // {
    //     // UIController.Instance.HideDeathScreen();
    //     StartCoroutine(DelayedSpawn());
    // }
}
