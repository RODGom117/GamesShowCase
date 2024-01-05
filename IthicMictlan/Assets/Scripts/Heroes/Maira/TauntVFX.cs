using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Heroes.Maira
{
    public class TauntEffect : MonoBehaviour
    {
        [SerializeField] List<Material> materials = new List<Material>();
        [SerializeField] Color outlineColorChange = Color.blue;
        [SerializeField] private float outlineSizeChange = 1f;
        [SerializeField] private float lerpDuration = 1f;
        
        [SerializeField] private float duration = 5f;
        private Color originalColor;
        private float originalOutlineSize;

        private static readonly int Property = Shader.PropertyToID("_OutlineColor");
        private static readonly int Property2 = Shader.PropertyToID("_OutlineSize");

        private bool isQuitting = false;
        [SerializeField] private PhotonView _photonView;
        
        // Start is called before the first frame update
        void OnEnable()
        {
            if (materials != null && materials.Count > 0)
            {
                originalColor = materials[0].GetColor(Property);
                originalOutlineSize = materials[0].GetFloat(Property2);
            }
        }

        private void OnApplicationQuit()
        {
            // Ensure materials are reverted back to original state when quitting
            if (!isQuitting)
            {
                RevertMaterials();
            }
        }

        public void RunTauntEffect()
        {
            if (_photonView.IsMine)
            {
                StartCoroutine(DoTauntEffect());
            }
        }

        IEnumerator DoTauntEffect()
        {
            float elapsedTime = 0f;
            Color startColor = originalColor;
            float startOutlineSize = originalOutlineSize;

            // Apply the taunt effect with lerping
            while (elapsedTime < lerpDuration)
            {
                foreach (Material material in materials)
                {
                    material.SetColor(Property, Color.Lerp(startColor, outlineColorChange, elapsedTime / lerpDuration));
                    material.SetFloat(Property2, Mathf.Lerp(startOutlineSize, outlineSizeChange, elapsedTime / lerpDuration));
                }

                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // Ensure the final state is set
            foreach (Material material in materials)
            {
                material.SetColor(Property, outlineColorChange);
                material.SetFloat(Property2, outlineSizeChange);
            }

            yield return new WaitForSeconds(duration);

            // Revert the changes with lerping
            elapsedTime = 0f;
            while (elapsedTime < lerpDuration)
            {
                foreach (Material material in materials)
                {
                    material.SetColor(Property, Color.Lerp(outlineColorChange, startColor, elapsedTime / lerpDuration));
                    material.SetFloat(Property2, Mathf.Lerp(outlineSizeChange, startOutlineSize, elapsedTime / lerpDuration));
                }

                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }

            // Ensure the final state is set
            foreach (Material material in materials)
            {
                material.SetColor(Property, startColor);
                material.SetFloat(Property2, startOutlineSize);
            }
        }

        void RevertMaterials()
        {
            // Revert materials to their original state
            foreach (Material material in materials)
            {
                material.SetColor(Property, originalColor);
                material.SetFloat(Property2, originalOutlineSize);
            }
        }
    }
}
