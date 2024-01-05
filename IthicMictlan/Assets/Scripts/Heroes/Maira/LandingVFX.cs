using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Heroes.Maira
{
    public class LandingVFX : MonoBehaviour
    {
        
        public GameObject landingVFXPrefab;
        [SerializeField] private PhotonView _photonView;

        public void PlayLandingVFX()
        {
            StartCoroutine(CoRoutineLandingVFX());
        }
        public IEnumerator CoRoutineLandingVFX()
        {
            if (_photonView.IsMine)
            {
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);
                // Instantiate the VFX prefab
                GameObject landingVFX = Instantiate(landingVFXPrefab,
                    transform.position + transform.forward * 2 + -transform.up, rotation);

                // Set the VFX to play
                ParticleSystem particleSystem = landingVFX.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {

                    particleSystem.Play();
                }

                yield return new WaitForSeconds(particleSystem.main.duration);
                // Destroy the VFX after it finishes playing
                PhotonNetwork.Destroy(landingVFX);
            }
        }
    }
}