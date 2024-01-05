using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float maxHealth = 100f;
    public float currentHealth;

    // UI health bar
    public Slider healthBar;

    private void Start()
    {
        currentHealth = maxHealth;

        // Instantiate the UI health bar above the enemy's head.
        if (photonView.IsMine)
        {
            GameObject canvasGO = new GameObject("HealthCanvas");
            canvasGO.transform.SetParent(transform, false);

            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            healthBar = canvasGO.AddComponent<Slider>();
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    private void Update()
    {
        // Example: Damage the enemy when a certain condition is met.
        if (Input.GetKeyDown(KeyCode.Space) && photonView.IsMine)
        {
            TakeDamage(10f);
        }
    }

    private void TakeDamage(float damage)
    {
        if (photonView.IsMine)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Max(0f, currentHealth);

            // Sync the updated health value across the network.
            photonView.RPC("SyncHealth", RpcTarget.OthersBuffered, currentHealth);

            // Update the UI health bar locally.
            UpdateHealthBar();

            // Handle death or other logic as needed.
            if (currentHealth <= 0f)
            {
                // Call a method to handle enemy death.
                HandleDeath();
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    [PunRPC]
    private void SyncHealth(float newHealth)
    {
        currentHealth = newHealth;

        // Update the UI health bar on all clients.
        UpdateHealthBar();
    }

    private void HandleDeath()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
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

            // Update the UI health bar on all clients.
            UpdateHealthBar();
        }
    }
}
