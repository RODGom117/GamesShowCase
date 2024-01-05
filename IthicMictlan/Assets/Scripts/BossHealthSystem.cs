using System.Collections;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthSystem : MonoBehaviour
{
    public float maxHealth = 100f; // Default max health
    public float currentHealth;

    public void Initialize(float health)
    {
        maxHealth = health;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        // Implement death logic here, such as destroying the GameObject
        yield return new WaitForSeconds(10f);
        PhotonNetwork.Destroy(gameObject);
    }
}
