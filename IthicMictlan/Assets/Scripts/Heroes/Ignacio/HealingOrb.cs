using System;
using System.Collections;
using Photon.Pun;
using UI;
using UnityEngine;

namespace Heroes.Ignacio
{
    public class HealingOrb : MonoBehaviour
    {
        [SerializeField] private HeroStats ignacioStats; // Set the damage amount
        public float maxRange = 10f;
        public Vector3 healingOrbDirection = Vector3.forward;
        //Reference to ignacio
        public GameObject ignacioInstance;
        private Vector3 startPosition;
        [SerializeField] private PhotonView _photonView;
        public GameObject enemyHit;
        public GameObject heroHit;
        private float vfxDuration = 1.3f;

        private void OnEnable()
        {
            startPosition = transform.position;
        }
    
        void Update()
        {
            float distanceTravelled = Vector3.Distance(startPosition, transform.position);
            //Only destroy when orb has reached max range
            if (distanceTravelled > maxRange)
            {
                PhotonNetwork.Destroy(gameObject);
            }           
        }
        
        public void Launch()
        {
            GetComponent<Rigidbody>().velocity = healingOrbDirection;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if the collided object is an enemy
            if (other.gameObject.CompareTag("Enemy")) // assuming your enemies have an "Enemy" tag
            {
                // Apply damage to enemy
                other.gameObject.GetComponent<HealthSystem>().TakeDamage(ignacioStats.abilityAttributes.primaryAbility.damage);
                ignacioInstance.GetComponent<HealthSystem>().HealPlayer(ignacioStats.abilityAttributes.primaryAbility.damage * 5f);
                
                // The VFX is generated when it hits the enemy
                if (enemyHit != null)
                {
                    GameObject vfx = Instantiate(enemyHit, transform.position, Quaternion.identity);
                    Destroy(vfx, vfxDuration);

                    // Destroy the projectile after hitting the enemy
                    PhotonNetwork.Destroy(gameObject);
                }
            }
            else if (other.gameObject.CompareTag("Hero") && !IsFriendlyPlayer(other.gameObject))
            {
                other.gameObject.GetComponent<HealthSystem>().HealPlayer(ignacioStats.abilityAttributes.primaryAbility.damage);
                // The VFX is generated when it hits the hero
                if (heroHit != null)
                {
                    GameObject vfx = Instantiate(heroHit, transform.position, Quaternion.identity);
                    Destroy(vfx, vfxDuration);
                    
                    // Destroy the projectile after hitting the hero
                    PhotonNetwork.Destroy(gameObject);
                }  
            }
        }

        private bool IsFriendlyPlayer(GameObject player)
        {
            // Verify if the player is the one who launched the object based on a specific script
            return player.GetComponent<PlayerManagerIgnacioPhoton>() != null;
        }
    }
}