using System;
using System.Collections;
using Photon.Pun;
using UI;
using UnityEngine;

namespace Heroes.Teo
{
    public class Spear : MonoBehaviour
    {

        [SerializeField] private HeroStats teoStats; // Set the damage amount

        public float maxRange = 25f;
        
        public Vector3 spearDirection;
        
        private Vector3 startPosition;
        [SerializeField] private PhotonView _photonView;
        private void OnEnable()
        {

            startPosition = transform.position;
            
        }
        

        void Update()
        {
            float distanceTravelled = Vector3.Distance(startPosition, transform.position);

            if (distanceTravelled > maxRange)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            
        }

        public void Launch()
        {
            GetComponent<Rigidbody>().velocity = spearDirection;
        }
        private void OnTriggerEnter(Collider other)
        {
            // Check if the collided object is an enemy
            if (other.gameObject.CompareTag("Enemy")) // assuming your enemies have an "Enemy" tag
            {
                // Apply damage to enemy
                other.gameObject.GetComponent<HealthSystem>().TakeDamage(teoStats.abilityAttributes.primaryAbility.damage);
                // Destroy the feather game object
                PhotonNetwork.Destroy(gameObject);
            }
        }
        
    }
}
