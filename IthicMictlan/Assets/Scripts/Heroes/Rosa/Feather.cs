using System;
using System.Collections;
using Photon.Pun;
using UI;
using UnityEngine;

namespace Heroes.Rosa
{
    public class Feather : MonoBehaviour
    {

        [SerializeField] private HeroStats rosaStats; // Set the damage amount

        [SerializeField] private GameObject featherPrefab;
        public float maxRange = 5f;
        
        public Vector3 featherDirection = Vector3.forward;
        

        public UIManager rosaUIManager;

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
            GetComponent<Rigidbody>().velocity = featherDirection;
        }
        private void OnTriggerEnter(Collider other)
        {
            // Check if the collided object is an enemy
            if (other.gameObject.CompareTag("Enemy")) // assuming your enemies have an "Enemy" tag
            {
                // Apply damage to enemy
                other.gameObject.GetComponent<HealthSystem>().TakeDamage(rosaStats.abilityAttributes.primaryAbility.damage);
                rosaUIManager.UpdateMadness(rosaStats.abilityAttributes.primaryAbility.madnessValue);
                // Destroy the feather game object
                PhotonNetwork.Destroy(gameObject);
            }
        }
        
    }
}