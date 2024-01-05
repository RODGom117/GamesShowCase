using System.Collections;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Heroes.Ignacio
{
    public class IgnacioBeam : MonoBehaviour
    {
        [SerializeField] private HeroStats ignacioStats; // Set the damage amount
        public float maxRange = 30f;
        public Vector3 ignacioBeamDirection = Vector3.forward;
        //Reference to ignacio
        public GameObject ignacioInstance;
        private Vector3 startPosition;
        [SerializeField] private PhotonView photonView;
        public GameObject greenFlash;
        private void OnEnable()
        {      
            startPosition = transform.position;
            Invoke("DestroyBeam", 2f);        
        }
        
        private Vector3 GetSpawnPosition()
        {
            // Assuming the player's forward direction is the desired spawn direction
            return ignacioInstance.transform.position + ignacioInstance.transform.forward * 21f;
        }
        public void Spawn()
        {
            transform.position = GetSpawnPosition();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Hero"))
            {
                other.GetComponent<IHeroCombat>().IgnacioBuffBeam(ignacioStats.abilityAttributes.secondaryAbility.duration);
                // Display the green flash
                ShowGreenFlash();
            }
        }

        void DestroyBeam()
        {
            PhotonNetwork.Destroy(gameObject);
        }

        // Function to display the green flash
        private void ShowGreenFlash()
        {
            if (greenFlash != null)
            {
                // Instantiate the prefab of the particle system
                GameObject greenFlashs = Instantiate(greenFlash, transform.position, Quaternion.identity);
                
                // Destroy the prefab after a certain time
                Destroy(greenFlashs, 5f);
            }
        }
    }
}