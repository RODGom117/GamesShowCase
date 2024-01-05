using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class AhuizotlBehaviour : MonoBehaviour
{
    [SerializeField, Header("Ahuizotl Scriptable Object")] private EnemyStats enemyStats; // El scriptable object que contiene la info del Ahuizotl
    [SerializeField] private Transform target; // El objetivo actual del agente
    [SerializeField] private GameObject targetObject; // El objecto del objetivo
    public float scanRadius = 5f; // Radio de escaneo alrededor del agente 
    public LayerMask targetLayer; // Capa de objetos objetivo
    private float moveSpeed; // Velocidad de movimiento del agente
    private NavMeshAgent agent;
    private static Transform sharedTarget; // Objetivo compartido entre todos los agentes
    private Vector3 spawnPoint; // Punto de origen del agente
    private float timeCounter; // Contador de tiempo para el movimiento circular

    public float circleRadius = 5f; // Radio del movimiento circular
    public float circleSpeed = 1f; // Velocidad del movimiento circular

    // Estadisticas base del Ahuizotl
    private float health;
    private float atackDamage;
    private float cacaoDrop;
    private bool canBite;
    private bool targetWasFound;
    private bool isDead;

    private Animator animator;
    private HealthSystem myAhuizotlHealth;
    // Photon
    [SerializeField] private PhotonView photonView;


    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        moveSpeed = enemyStats.movementAttributes.movementSpeed;
        spawnPoint = transform.position;
        atackDamage = enemyStats.combatAttributes.basicAttackDamage;
        cacaoDrop = enemyStats.baseAttributes.CacaoDrop;
        canBite = true;
        targetWasFound = false;
        isDead = false;
        animator = GetComponent<Animator>();
        myAhuizotlHealth = GetComponent<HealthSystem>();
        myAhuizotlHealth.InitializeHealth(enemyStats.healthAttributes.maxHealth);
        animator.SetBool("targetWasFound", targetWasFound);
    }

    void Update()
    {
        if(photonView.IsMine)
        {
            CheckHealth();

            // Detection and movement logic
            if (sharedTarget == null)
            {
                // Move in a circular pattern
                timeCounter += Time.deltaTime * circleSpeed;
                float x = Mathf.Cos(timeCounter) * circleRadius + spawnPoint.x;
                float z = Mathf.Sin(timeCounter) * circleRadius + spawnPoint.z;
                Vector3 newPosition = new Vector3(x, transform.position.y, z);
                agent.SetDestination(newPosition);
                // Si no hay un objetivo compartido, escanea la zona en busca de uno
                ScanForTarget();
            }
            else
            {
                // Usa el objetivo compartido
                target = sharedTarget;
                targetWasFound = true;
                agent.SetDestination(target.position);
            }

            animator.SetBool("targetWasFound", targetWasFound);

            // TODO: CAMBIAR LA FORMA EN LA QUE SE HACE EL GET COMPONENT >:c ESTA MAL HACER EL SET TODO EL TIEMPO 

            // Attack logic
            if(AhuizotlAttack.instance.GetAttackState())
            {
                if(canBite)
                {
                    StartCoroutine(attackSpeedController());
                }
                animator.SetBool("canAttack", AhuizotlAttack.instance.GetAttackState());
            }

            if(!AhuizotlAttack.instance.GetAttackState())
            {
                animator.SetBool("canAttack", AhuizotlAttack.instance.GetAttackState());
            }
        }
    }

    void ScanForTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, scanRadius, targetLayer);

        if (targets.Length > 0)
        {
            // Encontrar el objetivo más cercano dentro del radio de escaneo
            Transform closestTarget = null;
            float closestDistance = float.MaxValue;

            foreach (Collider col in targets)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);

                if (distance < closestDistance)
                {
                    closestTarget = col.transform;
                    closestDistance = distance;
                }
            }

            // Asignar el objetivo encontrado a TODOS los agentes
            sharedTarget = closestTarget;
            targetWasFound = true;
        }
    }

    void OnDrawGizmos()
    {
        // Draw a wire sphere to visualize the scanRadius in the scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, scanRadius);
    }

    public void CheckHealth()
    {
        if(myAhuizotlHealth.currentHealth <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", isDead);
            agent.speed = 0;
            agent.isStopped = true;
            StartCoroutine(KillAhuizotl());
        }
    }

    private IEnumerator KillAhuizotl()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private IEnumerator attackSpeedController()
    {
        animator.SetBool("canBite", canBite);
        canBite = false;
        animator.SetBool("canBite", canBite);
        targetObject = AhuizotlAttack.instance.GetTarget();
        targetObject.GetComponent<HealthSystem>().TakeDamage(atackDamage);
        gameObject.transform.LookAt(targetObject.transform);
        yield return new WaitForSeconds(5f);
        canBite = true;
        animator.SetBool("canBite", canBite);
    }
}
