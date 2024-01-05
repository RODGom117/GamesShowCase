using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Heroes.Ignacio
{
    public class IgnacioUltimate : MonoBehaviour
    {
        public GameObject ultimateModel;

        public GameObject ignacioInstance;

        [SerializeField] private HeroStats ignacioStats; // Set the damage amount

        public float maxRange = 30f;
        
        public Vector3 startPosition = Vector3.forward;

        public Vector3 ultimatePosition;
        [SerializeField] PhotonView _photonView;

        public void SpawnUltimate()
        {

            Debug.Log("Playing Ignacio's ultimate");
            ultimatePosition = startPosition + (Vector3.forward * 5f);
            transform.position = ultimatePosition;

            GameObject ultimateInstance = Instantiate(ultimateModel, ultimatePosition, Quaternion.identity);
            Destroy(ultimateInstance, 1f);

            DamageEnemies();
            StartCoroutine(DestroyUltimate());
        }


        IEnumerator DestroyUltimate()
        {
            yield return new WaitForSeconds(1f);
            PhotonNetwork.Destroy(gameObject);
        }

        public void DamageEnemies()
        {
            // Get all colliders within the radius
            Collider[] hitColliders = Physics.OverlapSphere(ultimatePosition, maxRange);

            float amountOfEnemiesHit = 0f;
            // Iterating using 'foreach' loop
            foreach (Collider hitCollider in hitColliders)
            {
                // If the collider is an enemy
                if (hitCollider.CompareTag("Enemy"))
                {
                    HealthSystem healthSystem = GetComponent<Collider>().GetComponent<HealthSystem>();
                    if (healthSystem != null)
                    {
                        // Damage the enemy
                        healthSystem.TakeDamage(ignacioStats.abilityAttributes.ultimateAbility.damage);
                        amountOfEnemiesHit++;
                    }
                }
                BuffHeroes(amountOfEnemiesHit);
            }
            

            
        }
        public void BuffHeroes(float amountOfEnemiesHit)
        {
            // Get all heroes in the scene
            GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");

            // Buff each hero
            foreach (GameObject heroObject in heroes)
            {
                // Get the hero component
                IHeroCombat hero = heroObject.GetComponent<IHeroCombat>();

                hero.IgnacioBuffUltimate(amountOfEnemiesHit, ignacioStats.abilityAttributes.ultimateAbility.duration);
            }
        }
    }
}