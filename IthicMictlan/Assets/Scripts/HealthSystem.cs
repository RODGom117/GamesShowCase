using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UI;

public class HealthSystem : MonoBehaviourPunCallbacks, IPunObservable
{
    public float maxHealth = 100f; // Default max health
    [SerializeField] public float currentHealth;

    public UIManager uiManager;
    public CharacterType characterType;

    //TODO: make this a new script or give it to chanequeenemy.
    // Object pooling variables
    private static Queue<GameObject> enemyPool = new Queue<GameObject>();
    private const int PoolSize = 10;
    // Photon variables
    [SerializeField] private PhotonView photonView;
    [SerializeField] private PhotonMatchManager photonMatchManager;

    private void Start()
    {
        photonMatchManager = GameObject.FindWithTag("Photon").GetComponent<PhotonMatchManager>();
    }

    public void InitializeHealth(float health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
        if (uiManager != null)
        {
            uiManager.SetMaxHealth(maxHealth);
        }
    }

    public void HealPlayer(float damage)
    {
        photonView.RPC("RPCHealPlayer", RpcTarget.All, damage);
    }

    public void TakeDamage(float damage)
    {
        photonView.RPC("RPCTakeDamagePlayer", RpcTarget.All, damage);
    }

    private void UpdateHealthUI()
    {
        if(photonView.IsMine)
        {
            if (uiManager != null)
            {
                uiManager.UpdateHealth(currentHealth, characterType);
            }
        }

    }

    private void HandleDeath()
    {
        if (photonView.IsMine)
        {
            // If enemy, return it to the pool instead of destroying it
            if (characterType == CharacterType.Enemy)
            {
                ReturnToPool();
            }
            else
            {
                // Handle player death logic here
                Debug.Log("You died!");
            }
        }
    }

    private void ReturnToPool()
    {
        // Return the enemy GameObject to the pool
        enemyPool.Enqueue(gameObject);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        // If the GameObject is activated, reset its health to the maximum
        // InitializeHealth(maxHealth);
    }

    private void OnDisable()
    {
        // If the GameObject is deactivated, return it to the pool
        if (characterType == CharacterType.Enemy)
        {
            enemyPool.Enqueue(gameObject);
        }
    }

    private static GameObject GetFromPool(Vector3 position, Quaternion rotation)
    {
        // Try to get an enemy GameObject from the pool, or instantiate a new one if the pool is empty
        if (enemyPool.Count > 0)
        {
            GameObject enemy = enemyPool.Dequeue();
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            enemy.SetActive(true);
            return enemy;
        }
        else
        {
            // Instantiate a new enemy GameObject if the pool is empty
            ///TODO: add to call of this function the path of enemy to spawn
            return PhotonNetwork.Instantiate("YourEnemyPrefabName", position, rotation);
        }
    }

    [PunRPC]
    private void SyncHealth(float newHealth)
    {
        if(photonView.IsMine)
        {
            currentHealth = newHealth;
            UpdateHealthUI();
        }
    }

    [PunRPC]
    private void RPCHealPlayer(float damage)
    {
        if(photonView.IsMine)
        {
            currentHealth += damage;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
                Debug.Log("Health was maxed out");
            }

            UpdateHealthUI();
            if(characterType == CharacterType.Hero)
            {
                photonMatchManager.ChangeStatSent(PhotonNetwork.LocalPlayer.ActorNumber, 1, (int)damage);
            }
            


            // Sync health across the network
            //photonView.RPC("SyncHealth", RpcTarget.OthersBuffered, currentHealth);  
        }
    }

    [PunRPC]
    private void RPCTakeDamagePlayer(float damage)
    {
        if(photonView.IsMine)
        {
            Debug.Log($"{gameObject.name} took {damage} damage");
            currentHealth -= damage;
            UpdateHealthUI();

            // Sync health across the network
            //photonView.RPC("SyncHealth", RpcTarget.OthersBuffered, currentHealth);

            if(characterType == CharacterType.Hero)
            {
                photonMatchManager.ChangeStatSent(PhotonNetwork.LocalPlayer.ActorNumber, 1, -(int)damage);
            }

            /*if (currentHealth <= 0)
            {
                HandleDeath();
            }*/
        }
    }


    // IPunObservable implementation for custom synchronization (optional).
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // If the object is owned by the current client, send its health value to others.
            stream.SendNext(currentHealth);
        }
        else
        {
            // If the object is owned by another client, receive and set its health value.
            currentHealth = (float)stream.ReceiveNext();
            UpdateHealthUI();
        }
    }
}
