using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Heroes.Rosa
{
    //TODO: Add a VFX prefab for the transformation that syncs with the model switching.
    public class UltimateRosa : MonoBehaviour
    {
        public GameObject newModel;
        public GameObject oldModel;
        private Avatar newAvatar;
        public float delay = 10f; // delay in seconds
        private Avatar oldAvatar;
        private Animator animator;
        [SerializeField] GameObject transformationVFXPrefab;
        [SerializeField] private PhotonView _photonView;


        private void Start()
        {
            animator = GetComponentInParent<Animator>();
            newAvatar = newModel.GetComponent<Animator>().avatar;
            oldAvatar = oldModel.GetComponent<Animator>().avatar;
        }

        // This method is called by the first animation event
        public void StartTransformation()
        {
            if (_photonView.IsMine)
            {
                // Start the VFX coroutine
                StartCoroutine(PlayVFX());
            }
        }

        // This method is called by the second animation event
        public void CompleteTransformation()
        {
            if (_photonView.IsMine)
            {
                // Switch avatar before changing models
                SwitchAvatar(newAvatar);

                // Hide the old model and show the new model
                oldModel.SetActive(false);
                newModel.SetActive(true);

                // Delay before reverting to the old model
                StartCoroutine(RevertAfterDelay());
            }
        }

        IEnumerator PlayVFX()
        {
            // Instantiate the VFX prefab
            GameObject transformationVFX = Instantiate(transformationVFXPrefab, transform.position, Quaternion.identity);

            // Parent the VFX to the character
            transformationVFX.transform.parent = transform;

            ParticleSystem particleSystem = transformationVFX.GetComponent<ParticleSystem>();

            // Set the VFX to play
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            // Wait for the VFX duration before destroying it
            yield return new WaitForSeconds(particleSystem.main.duration);

            // Unparent the VFX before destroying it
            transformationVFX.transform.parent = null;

            // Destroy the VFX after it finishes playing
            Destroy(transformationVFX);
        }
        void SwitchAvatar(Avatar avatar)
        {
            if (animator != null && avatar != null)
            {
                animator.avatar = avatar;
            }
            else
            {
                Debug.LogError("Animator or newAvatar is null. Make sure to assign the Animator component and a new Avatar in the Unity Editor.");
            }
        }

        IEnumerator RevertAfterDelay()
        {
            yield return new WaitForSeconds(delay);
            StartCoroutine(PlayVFX());
            yield return new WaitForSeconds(delay * 0.1f);
            // Hide the new model and show the old model
            newModel.SetActive(false);
            oldModel.SetActive(true);

            // Switch back to the original avatar
            SwitchAvatar(oldAvatar);
        }
    }
}
