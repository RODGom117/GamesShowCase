using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AhuizotlAttack : MonoBehaviour
{
    public static AhuizotlAttack instance;
    private bool canAttack;
    private GameObject myTarget;

    void Awake() {
        instance = this;
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Hero")
        {
            canAttack = true;
            myTarget = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Hero")
        {
            canAttack = false;
        }
    }

    public bool GetAttackState()
    {
        return canAttack;
    }

    public GameObject GetTarget()
    {
        return myTarget;
    }

}
