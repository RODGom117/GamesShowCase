using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBulletDamage : MonoBehaviour
{
    private float projectileDamage;
    
    void OnTriggerEnter(Collider other)
    {
        HealthSystem objectiveLife = other.GetComponent<HealthSystem>();

        if (objectiveLife != null)
        {
            objectiveLife.TakeDamage(projectileDamage);
        }
    }

    public void SetDamage(float damage)
    {
        projectileDamage = damage;
    }
    
}
