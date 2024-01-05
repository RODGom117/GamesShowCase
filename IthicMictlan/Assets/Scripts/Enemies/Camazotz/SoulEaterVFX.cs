using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEaterVFX : MonoBehaviour
{
    [SerializeField] private GameObject particleVFXPrefab;

    public void PlaySoulEaterVFX()
    {
        GameObject soulEaterVFX = Instantiate(particleVFXPrefab, particleVFXPrefab.transform.position, particleVFXPrefab.transform.rotation);

        // Set the VFX to play
        ParticleSystem particleSystem = soulEaterVFX.GetComponent<ParticleSystem>();
        Debug.Log("particleSystem: " + particleSystem);
        if (particleSystem != null)
        {

            particleSystem.Play();
        }
        Destroy(soulEaterVFX, particleSystem.main.duration);
    }
}
