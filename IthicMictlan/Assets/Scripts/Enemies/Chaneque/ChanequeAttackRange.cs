using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanequeAttackRange : MonoBehaviour
{
    public static ChanequeAttackRange instance;
    [SerializeField] private SphereCollider attackRange;
    [SerializeField] private GameObject closestTarget;
    [SerializeField] private List<GameObject> targets;
    private bool canAttack;

    void Awake() {
        if(instance == null)
        {
            instance = this;
        }
        
    }

    void OnEnable() {
        canAttack = false;
        GameObject [] players = GameObject.FindGameObjectsWithTag("Hero");

        foreach (GameObject player in players)
        {
            targets.Add(player);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Hero"))
        {
            foreach (GameObject currentTarget in targets)
            {
                if(closestTarget == currentTarget)
                {
                    canAttack = true;
                }
            }
            
        }
    }

    private void OnTriggerExit(Collider other) {
        canAttack = false;
        closestTarget = null;
    }

    public bool GetAttackState()
    {
        return canAttack;
    }

    public GameObject GetClosestTarget()
    {
        return closestTarget;
    }

    public void SetClosestTarget(GameObject target)
    {
        closestTarget = target;
    }
}
