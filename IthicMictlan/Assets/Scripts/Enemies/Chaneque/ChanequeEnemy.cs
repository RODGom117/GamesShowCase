using Heroes.Maira;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Photon.Pun;

public class ChanequeEnemy : MonoBehaviourPun
{
    private enum ChanequeState
    {
        Idle,
        Walk,
        Attack
        // Add more states as needed (e.g., Attack)
    }
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private ChanequeState currentState;

    private HealthSystem _healthSystem;

    [SerializeField] private float runDistanceThreshold;
    [SerializeField] private float attackDistanceThreshold;

    public EnemyStats enemyStats;
    private List<GameObject> targets;
    private GameObject closestTarget;

    private bool isAttacking = false;
    private bool canAttack = true;

    private void Start()
    {
        targets = new List<GameObject>();
        ///TODO: Get list of players in scene from gamemanager here.
        GameObject[] tempTargets = GameObject.FindGameObjectsWithTag("Hero");
        Debug.Log("Found " + tempTargets.Length + " targets" + tempTargets[0].name);
        for(int i = 0; i < tempTargets.Length; i++)
        {
            targets.Add(tempTargets[i]);
        }
        /// 
        _healthSystem = GetComponent<HealthSystem>();
        _healthSystem.InitializeHealth(enemyStats.healthAttributes.maxHealth);

        runDistanceThreshold = 40f;
        attackDistanceThreshold = 4f;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        ChangeState(ChanequeState.Idle);

        if (targets == null || targets.Count == 0)
        {
            Debug.LogError("Targets not assigned!");
        }

        // Call RPC to initialize the enemy state on all clients
        //photonView.RPC("InitializeEnemyState", RpcTarget.AllBuffered, enemyStats.healthAttributes.maxHealth);
    }

    private void Update()
    {
        // Check if this is the local player's enemy
        if (photonView.IsMine)
        {
            if(_healthSystem.currentHealth <= 0)
            {
                StartCoroutine(KillChaneque());
            }
            // Your AI logic to determine when to switch states
            closestTarget = GetClosestTarget();

            if (closestTarget != null && Vector3.Distance(transform.position, closestTarget.transform.position) < attackDistanceThreshold)
            {
                if(ChanequeAttackRange.instance.GetAttackState())
                {
                    if(canAttack)
                    {
                        // Call RPC to sync the attack action across the network
                        ChangeState(ChanequeState.Attack);
                        StartCoroutine(TriggerAttack());
                    }  
                }        
            }
            else if (closestTarget != null)
            {
                navMeshAgent.SetDestination(closestTarget.transform.position);
                ChangeState(ChanequeState.Walk);
            }
            else
            {
                ChangeState(ChanequeState.Idle);
            }
        }
    }

    private IEnumerator KillChaneque()
    {
        yield return new WaitForSeconds(2f);
        PhotonNetwork.Destroy(gameObject);
    }

    private void ChangeState(ChanequeState newState)
    {
        // Handle state entry logic
        switch (newState)
        {
            case ChanequeState.Idle:
                // Enter the Idle state
                // Debug.Log("Entering Idle state");
                animator.SetBool("IsIdle", true);
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAttacking", false);
                break;
            case ChanequeState.Walk:
                // Enter the Walk state
                // Debug.Log("Entering Walk state");
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsAttacking", false);
                break;
            case ChanequeState.Attack:
                // Enter the Attack state
                // Debug.Log("Entering Attack state");
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsWalking", false);
                break;
            // Handle entry logic for other states as needed
        }

        // Update the current state
        currentState = newState;
    }

    private IEnumerator TriggerAttack()
    {
        /*animator.SetBool("IsAttacking", true);
        isAttacking = true;
        closestTarget.GetComponent<HealthSystem>().TakeDamage(enemyStats.combatAttributes.basicAttackDamage);
        yield return new WaitForSeconds(2f);
        isAttacking = false;
        animator.SetBool("IsAttacking", false);*/
        animator.SetBool("IsAttacking", canAttack);
        canAttack = false;
        animator.SetBool("IsAttacking", canAttack);
        GameObject targetObject = ChanequeAttackRange.instance.GetClosestTarget();
        targetObject.GetComponent<HealthSystem>().TakeDamage(enemyStats.combatAttributes.basicAttackDamage);
        gameObject.transform.LookAt(targetObject.transform);
        yield return new WaitForSeconds(3f);
        canAttack = true;
        animator.SetBool("IsAttacking", canAttack);
    }
    
    GameObject GetClosestTarget()
    {
        GameObject newTarget = null;
        float closestDistance = Mathf.Infinity; // TODO: Change this to a very large number
        foreach (GameObject target in targets)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                closestTarget = target;
                ChanequeAttackRange.instance.SetClosestTarget(closestTarget);
            }
        }
        return closestTarget;
    }

    //TODO: Implement taunt for maira (bool istaunted)
    public void ChangeTarget(Transform position)
    {
        // // Set the new target
        // targets[0] = position;
    }
}
