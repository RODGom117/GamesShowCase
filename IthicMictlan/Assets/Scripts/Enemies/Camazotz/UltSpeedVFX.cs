using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UltSpeedVFX : MonoBehaviour
{
    [SerializeField] private GameObject particleVFXPrefab;

    public void PlayUltSpeedVFX()
    {
        GameObject ultSpeedVFX = Instantiate(particleVFXPrefab, particleVFXPrefab.transform.position, particleVFXPrefab.transform.rotation);

        // Set the VFX to play
        ParticleSystem particleSystem = ultSpeedVFX.GetComponent<ParticleSystem>();
        Debug.Log("particleSystem: " + particleSystem);
        if (particleSystem != null)
        {

            particleSystem.Play();
        }
        Destroy(ultSpeedVFX, particleSystem.main.duration);
    }
}
