using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Heroes.Maira
{
    public class WrestlingRing : MonoBehaviour
    {
        //Do we need to add photnoview here?
        void OnTriggerEnter(Collider other) 
        {
            if(other.gameObject.CompareTag("Hero"))
            {
                Debug.Log("Player inside of WrestlingRing");
                HeroesCombatMaira combatMairaScript = other.gameObject.GetComponent<HeroesCombatMaira>();
                if (combatMairaScript != null)
                {
                    combatMairaScript.InsideWrestlingRing();
                    UltimateVFX vfxScript = other.GetComponent<UltimateVFX>();
                    vfxScript.PlayFireHandVFX();

                }
                // Add VFX to specific body parts with a particle system
                // Modify the scale of the player GameObject to increase their size
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Hero"))
            {
                HeroesCombatMaira combatMairaScript = other.gameObject.GetComponent<HeroesCombatMaira>();
                if (combatMairaScript != null)
                {
                    combatMairaScript.OutsideWrestlingRing();
                    UltimateVFX vfxScript = other.GetComponent<UltimateVFX>();
                    vfxScript.StopFireHandVFX();

                }
            }
        }
    }
}
