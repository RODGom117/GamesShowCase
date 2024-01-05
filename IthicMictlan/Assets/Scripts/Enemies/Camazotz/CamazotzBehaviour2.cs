using System.Collections;
using Unity.Barracuda;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CamazotzBehaviour2 : MonoBehaviour
{
    public enum State
    {
        InitialState, InitialSelectionOfPlayerToTargetState, FirstPhaseState, BasicAttackState,
        CloseRangeBasicAttackState, LargeRangeBasicAttackState, SoulEaterAttackState,
        UpsideDownWorldAttackState, InfernalScreechAttackState, ChangeOfPlayerToTargetState,
        SecondPhaseState, SoulDevourerAttackState, PhaseTransitionState, CamazotzDeathState
    }

    [Header("Camazotz States DEBUG ONLY, DO NOT TOUCH")]
    [SerializeField] private State currentState; //Current state of the Camazotz
    private bool isInSecondPhase; //Checks if the Camazotz is in the second phase
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private GameObject objective;
    CharacterController objectiveController;
    public GameObject CamazotzHand;
    private GameObject camazotzBody;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private GameObject[] playerList;

    private bool isAttacking;
    [SerializeField] private bool isInMidAttack;
    private float playerDistance;
    BossHealthSystem healthSystem;

    [SerializeField] private Animator animator;
    private string currentAnimationBool;
    UltSpeedVFX ultSpeedVFX;
    SoulEaterVFX soulEaterVFX;
    AirBulletVFX airBulletVFX;

    //Cooldowns and ranges for the attacks
    private float shortRange = 8.0f;
    private float largeRange = 20f;
    private float basicAttackCooldown = 0.0f;
    private float soulEaterCooldown = 0.0f;
    private float upsideDownWorldCooldown = 0.0f;
    private float infernalScreechCooldown = 0.0f;
    private float soulDevourerCooldown = 0.0f;
    // DEATH STUFFF
    [SerializeField] private PhotonMatchManager photonMatchManager;

    void Start()
    {
        currentState = State.InitialState;
        isAttacking = true;
        isInMidAttack = false;
        healthSystem = GetComponent<BossHealthSystem>();
        healthSystem.Initialize(enemyStats.healthAttributes.maxHealth);
        playerList = GameObject.FindGameObjectsWithTag("Hero");
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        camazotzBody = transform.GetChild(0).gameObject;
        ultSpeedVFX = GetComponent<UltSpeedVFX>();
        soulEaterVFX = GetComponent<SoulEaterVFX>();
        airBulletVFX = GetComponent<AirBulletVFX>();
        CamazotzAgentSetter();
        photonMatchManager = GameObject.FindWithTag("Photon").GetComponent<PhotonMatchManager>();
    }

    void CamazotzAgentSetter()
    {
        agent.speed = enemyStats.movementAttributes.movementSpeed;
    }

    void Update()
    {
        HandleInputDebug();

        switch (currentState)
        {
            case State.InitialState:
                HandleInitialState();
                break;

            case State.InitialSelectionOfPlayerToTargetState:
                HandleInitialSelectionOfPlayerToTargetState();
                break;

            case State.FirstPhaseState:
                HandleFirstPhaseState();
                agent.SetDestination(objective.transform.position);
                break;

            case State.BasicAttackState:
                HandleBasicAttackState();
                break;

            case State.CloseRangeBasicAttackState:
                HandleCloseRangeBasicAttackState();
                break;

            case State.LargeRangeBasicAttackState:
                HandleLargeRangeBasicAttackState();
                break;

            case State.SoulEaterAttackState:
                HandleSoulEaterAttackState();
                break;

            case State.UpsideDownWorldAttackState:
                HandleUpsideDownWorldAttackState();
                break;

            case State.InfernalScreechAttackState:
                HandleInfernalScreechAttackState();
                break;

            case State.ChangeOfPlayerToTargetState:
                HandleChangeOfPlayerToTargetState();
                break;

            case State.SecondPhaseState:
                HandleSecondPhaseState();
                agent.SetDestination(objective.transform.position);
                break;

            case State.SoulDevourerAttackState:
                HandleSoulDevourerAttackState();
                break;

            case State.PhaseTransitionState:
                HandlePhaseTransitionState();
                break;

            case State.CamazotzDeathState:
                HandleCamazotzDeathState();
                break;
        }
    }

    // Q0
    private void HandleInitialState()
    {
        if (isAttacking)
        {
            ChangeState(State.InitialSelectionOfPlayerToTargetState);
        }
    }

    // Q1
    private void HandleInitialSelectionOfPlayerToTargetState()
    {
        int pick = Random.Range(0, playerList.Length);
        objective = playerList[pick];
        agent.SetDestination(objective.transform.position);
        objectiveController = objective.GetComponent<CharacterController>();
        animator.SetBool("Idle", false);
        ChangeState(State.FirstPhaseState);
    }

    // Q2
    private void HandleFirstPhaseState()
    {
        int attackIndex = Random.Range(0, 4);

        if (healthSystem.currentHealth <= 0)
        {
            ChangeState(State.CamazotzDeathState); //Q13
        }
        else if (healthSystem.currentHealth <= healthSystem.maxHealth / 2)
        {
            ChangeState(State.SecondPhaseState); // Q10
        }
        else if (attackIndex == 0)
        {
            ChangeState(State.BasicAttackState); // Q3
        }
        else if (attackIndex == 1)
        {
            ChangeState(State.SoulEaterAttackState); // Q6
        }
        else if (attackIndex == 2)
        {
            ChangeState(State.UpsideDownWorldAttackState); // Q7
        }
        else if (attackIndex == 3)
        {
            ChangeState(State.InfernalScreechAttackState); // Q8
        }
    }

    private void DealDamageToTarget(float damage)
    {
        if (objective != null)
        {
            HealthSystem targetHealth = objective.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
        }
    }

    private void HandleAreaOfEffectDamage(float damage)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3f);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Hero"))
            {
                HealthSystem targetHealth = collider.gameObject.GetComponent<HealthSystem>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damage);
                }
            }
        }
    }

    private IEnumerator ResetBooleanParametersAfterDelay(string animationBool, Quaternion oldHeroRotation, string vfx, float delay = 1.0f)
    {
        agent.isStopped = true;

        float preDelay = 1f;

        yield return new WaitForSeconds(preDelay);

        if (vfx == "SoulEaterVFX")
        {
            soulEaterVFX.PlaySoulEaterVFX();
        }

        yield return new WaitForSeconds(delay - preDelay);

        // Reset the boolean parameters after the animation is complete
        animator.SetBool(animationBool, false);
        objective.GetComponent<Animator>().SetBool("Struggle", false);
        animator.SetBool("Throw", true);

        yield return new WaitForSeconds(1.16f);
        objective.transform.SetParent(null);
        objective.transform.position = transform.position + transform.forward * 15;
        objective.transform.rotation = oldHeroRotation;
        animator.SetBool("Throw", false);

        objectiveController.enabled = true;
        objective.GetComponent<PlayerMovement>().enabled = true;

        yield return new WaitForSeconds(1.0f);
        agent.speed = enemyStats.movementAttributes.movementSpeed;
        agent.isStopped = false;
        isInMidAttack = false;
    }

    private IEnumerator ResetBooleanParametersAfterDelay(string animationBool, string vfx, float delay = 1.0f)
    {
        agent.isStopped = true;

        float preDelay = .2f;

        yield return new WaitForSeconds(preDelay);

        if (vfx == "LargeBasic")
        {
            airBulletVFX.LaunchAirBullet(transform.position, new Vector3(1f, 1f, 1f), 1.1f, 25);
        }
        else if (vfx == "InfernalScreech")
        {
            airBulletVFX.LaunchAirBullet(transform.position, new Vector3(3f, 3f, 3f), 3.1f, 30);
        }

        yield return new WaitForSeconds(delay - preDelay);

        // Reset the boolean parameters after the animation is complete
        animator.SetBool(animationBool, false);
        agent.speed = enemyStats.movementAttributes.movementSpeed;
        agent.isStopped = false;
        isInMidAttack = false;
    }

    // Q3
    private void HandleBasicAttackState()
    {
        if (!isInMidAttack && basicAttackCooldown <= 0.0f)
        {

            playerDistance = Vector3.Distance(transform.position, objective.transform.position);
            if (playerDistance <= shortRange)
            {
                agent.speed = 0;
                agent.stoppingDistance = 8;
                ChangeState(State.CloseRangeBasicAttackState); // Q4
            }
            else if (playerDistance <= largeRange)
            {
                agent.speed = 0;
                agent.stoppingDistance = 20;
                ChangeState(State.LargeRangeBasicAttackState); // Q5
            }
            basicAttackCooldown = 3.0f;
            StartCoroutine(ResetCooldown("BasicAttackCooldown"));
        }
        else
        {
            ChangeState(State.FirstPhaseState);
        }
    }

    // Q4
    private void HandleCloseRangeBasicAttackState()
    {
        int attackIndex = Random.Range(0, 2);
        if (attackIndex == 1)
        {
            int left_Right = Random.Range(0, 2);
            if (left_Right == 0)
            {
                currentAnimationBool = "BasicRight";
                animator.SetBool(currentAnimationBool, true);
                isInMidAttack = true;
                DealDamageToTarget(20);
            }
            else
            {
                currentAnimationBool = "BasicLeft";
                animator.SetBool(currentAnimationBool, true);
                isInMidAttack = true;
                DealDamageToTarget(20);
            }
            StartCoroutine(ResetBooleanParametersAfterDelay(currentAnimationBool, "None", .5f));
        }
        AttacksSoftReset();
        PhaseChecker(attackIndex);
    }

    // Q5
    private void HandleLargeRangeBasicAttackState()
    {
        int attackIndex = Random.Range(0, 2);
        if (attackIndex == 1)
        {
            currentAnimationBool = "LongRange";
            animator.SetBool(currentAnimationBool, true);
            isInMidAttack = true;
            StartCoroutine(ResetBooleanParametersAfterDelay(currentAnimationBool, "LargeBasic", .5f));
        }
        AttacksSoftReset();
        PhaseChecker(attackIndex);
    }

    // Q6
    private void HandleSoulEaterAttackState()
    {
        if (!isInMidAttack && soulEaterCooldown <= 0.0f)
        {

            playerDistance = Vector3.Distance(transform.position, objective.transform.position);

            if (playerDistance <= shortRange)
            {
                agent.speed = 0;
                agent.stoppingDistance = 8;
                int attackIndex = Random.Range(0, 2);

                if (attackIndex == 1)
                {
                    currentAnimationBool = "SoulEaterHit";
                    animator.SetBool(currentAnimationBool, true);
                    isInMidAttack = true;
                    HandleSoulEaterHit();
                }
                else
                {
                    currentAnimationBool = "SoulEaterMiss";
                    animator.SetBool(currentAnimationBool, true);
                    isInMidAttack = true;
                    StartCoroutine(ResetBooleanParametersAfterDelay(currentAnimationBool, "None", .83f));
                }

                AttacksSoftReset();
                PhaseChecker(attackIndex);
                soulEaterCooldown = 10.0f;
                StartCoroutine(ResetCooldown("SoulEaterCooldown"));
            }
            else
            {
                ChangeState(State.FirstPhaseState);
            }
        }
        else
        {
            ChangeState(State.FirstPhaseState);
        }
    }

    private void HandleSoulEaterHit()
    {
        DealDamageToTarget(30);
        objectiveController.enabled = false;
        objective.GetComponent<PlayerMovement>().enabled = false;
        objective.GetComponent<Animator>().SetBool("Struggle", true);

        Quaternion oldHeroRotation = objective.transform.rotation;
        objective.transform.rotation = Quaternion.Euler(0, 0, 0);
        objective.transform.SetParent(CamazotzHand.transform);
        objective.transform.localPosition = Vector3.zero;
        objective.transform.localRotation = Quaternion.Euler(27.588f, 136.45f, 33.142f);

        string vfx = "SoulEaterVFX";
        StartCoroutine(ResetBooleanParametersAfterDelay(currentAnimationBool, oldHeroRotation, vfx, 3.5f));
    }

    private IEnumerator FlyUp(Quaternion oldHeroRotation, Vector3 oldPosition)
    {
        yield return new WaitForSeconds(1.0f);
        float tiempoInicio = Time.time;
        GetComponent<NavMeshAgent>().enabled = false;
        while (Time.time - tiempoInicio < 1.0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up * 10, 20 * Time.deltaTime);
            yield return null;
        }

        GetComponent<NavMeshAgent>().enabled = true;
        yield return StartCoroutine(ResetBooleanParametersAfterDelay(currentAnimationBool, oldHeroRotation, "None", 0.1f));
        GetComponent<NavMeshAgent>().enabled = false;
        transform.position = oldPosition;
        GetComponent<NavMeshAgent>().enabled = true;
    }

    // Q7
    private void HandleUpsideDownWorldAttackState()
    {
        if (!isInMidAttack && upsideDownWorldCooldown <= 0.0f)
        {

            playerDistance = Vector3.Distance(transform.position, objective.transform.position);
            if (playerDistance <= shortRange)
            {
                agent.speed = 0;
                agent.stoppingDistance = 8;
                int attackIndex = Random.Range(0, 2);
                Quaternion oldHeroRotation;
                Vector3 oldCamazotzPosition;
                if (attackIndex == 1)
                {
                    oldCamazotzPosition = transform.position;
                    DealDamageToTarget(30);
                    objectiveController.enabled = false;
                    objective.GetComponent<PlayerMovement>().enabled = false;
                    currentAnimationBool = "UpsideDownWorldHit";
                    animator.SetBool(currentAnimationBool, true);
                    isInMidAttack = true;
                    objective.GetComponent<Animator>().SetBool("Struggle", true);
                    oldHeroRotation = objective.transform.rotation;
                    objective.transform.SetParent(CamazotzHand.transform);
                    objective.transform.localPosition = Vector3.zero;
                    objective.transform.localRotation = Quaternion.Euler(27.588f, 136.45f, 33.142f);
                    StartCoroutine(FlyUp(oldHeroRotation, oldCamazotzPosition));
                }
                else
                {
                    currentAnimationBool = "UpsideDownWorldMiss";
                    animator.SetBool(currentAnimationBool, true);
                    isInMidAttack = true;
                    StartCoroutine(ResetBooleanParametersAfterDelay(currentAnimationBool, "None", .66f));
                }
                AttacksSoftReset();
                PhaseChecker(attackIndex);
                upsideDownWorldCooldown = 12.0f; // Set a 12-second cooldown for Upside Down World attack
                StartCoroutine(ResetCooldown("UpsideDownWorldCooldown"));
            }
            else
            {
                ChangeState(State.FirstPhaseState);
            }
        }
        else
        {
            ChangeState(State.FirstPhaseState);
        }
    }

    // Q8
    private void HandleInfernalScreechAttackState()
    {
        if (!isInMidAttack && infernalScreechCooldown <= 0.0f)
        {
            playerDistance = Vector3.Distance(transform.position, objective.transform.position);
            if (playerDistance >= shortRange && playerDistance <= largeRange)
            {
                agent.speed = 0;
                agent.stoppingDistance = playerDistance;
                int attackIndex = Random.Range(0, 2);
                if (attackIndex == 0)
                {
                    ChangeState(State.FirstPhaseState);
                }
                else
                {
                    currentAnimationBool = "InfernalScreech";
                    animator.SetBool(currentAnimationBool, true);
                    isInMidAttack = true;
                    StartCoroutine(ResetBooleanParametersAfterDelay(currentAnimationBool, "InfernalScreech", 1.6f));
                    ChangeState(State.ChangeOfPlayerToTargetState);
                }
                AttacksSoftReset();
                infernalScreechCooldown = 15.0f; // Set a 15-second cooldown for Infernal Screech attack
                StartCoroutine(ResetCooldown("InfernalScreechCooldown"));
            }
            else
            {
                ChangeState(State.FirstPhaseState);
            }
        }
        else
        {
            ChangeState(State.FirstPhaseState);
        }
    }

    // Q9
    private void HandleChangeOfPlayerToTargetState()
    {
        playerList = GameObject.FindGameObjectsWithTag("Hero");
        int pick = Random.Range(0, playerList.Length);
        objective = playerList[pick];
        agent.SetDestination(objective.transform.position);
        objectiveController = objective.GetComponent<CharacterController>();
        ChangeState(State.PhaseTransitionState);
    }

    // Q10
    private void HandleSecondPhaseState()
    {
        int attackIndex = Random.Range(0, 5);

        if (healthSystem.currentHealth <= 0)
        {
            ChangeState(State.CamazotzDeathState);
        }
        else if (attackIndex == 0)
        {
            ChangeState(State.BasicAttackState);
        }
        else if (attackIndex == 1)
        {
            ChangeState(State.SoulEaterAttackState);
        }
        else if (attackIndex == 2)
        {
            ChangeState(State.UpsideDownWorldAttackState);
        }
        else if (attackIndex == 3)
        {
            ChangeState(State.InfernalScreechAttackState);
        }
        else if (attackIndex == 4)
        {
            ChangeState(State.SoulDevourerAttackState);
        }
    }

    private IEnumerator UltimateAttack()
    {
        agent.isStopped = true;
        GetComponent<NavMeshAgent>().enabled = false;
        currentAnimationBool = "SoulDevourerJump";
        animator.SetBool(currentAnimationBool, true);
        yield return new WaitForSeconds(.6f);
        float tiempoInicio = Time.time;
        while (Time.time - tiempoInicio < 1.0f)
        {
            // transform.Translate(Vector3.up * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.up * 10, 20 * Time.deltaTime);
            yield return null;
        }

        //Hacemos invisible a Camazotz
        camazotzBody.SetActive(false);
        animator.SetBool(currentAnimationBool, false);

        //Esperar antes de volver a hacer visible el objeto
        yield return new WaitForSeconds(1f);

        GetComponent<NavMeshAgent>().enabled = true;

        //Movimiento lateral 1
        transform.position = objective.transform.position + new Vector3(-15f, 0f, 0f); // Posición inicial en el lado izquierdo
        camazotzBody.SetActive(true); // Hacemos visible el objeto

        // Adjust this value to control how much past the objective the game object should go
        float offset = 12.0f;
        // Calculate a new destination position
        Vector3 targetPosition = objective.transform.position + (objective.transform.position - transform.position).normalized * offset;

        // Movimiento de lado a lado
        tiempoInicio = Time.time;
        transform.LookAt(objective.transform.position);
        while (Time.time - tiempoInicio < 1.5f)
        {
            ultSpeedVFX.PlayUltSpeedVFX();
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 20 * Time.deltaTime);
            HandleAreaOfEffectDamage(2);
            yield return null;
        }

        //Hacer invisible el objeto
        camazotzBody.SetActive(false);

        //Esperar antes de volver a hacer visible el objeto
        yield return new WaitForSeconds(1f);

        //Movimiento lateral 2
        transform.position = objective.transform.position + new Vector3(15f, 0f, 0f); // Posición inicial en el lado derecho
        camazotzBody.SetActive(true); // Hacemos visible el objeto

        // Adjust this value to control how much past the objective the game object should go
        offset = 12.0f;
        // Calculate a new destination position
        targetPosition = objective.transform.position + (objective.transform.position - transform.position).normalized * offset;

        tiempoInicio = Time.time;
        transform.LookAt(objective.transform.position);
        while (Time.time - tiempoInicio < 1.5f)
        {
            ultSpeedVFX.PlayUltSpeedVFX();
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 20 * Time.deltaTime);
            HandleAreaOfEffectDamage(2);
            yield return null;
        }

        // Hacemos invisible el objeto
        camazotzBody.SetActive(false);

        // Esperamos antes de volver a hacer visible el objeto
        yield return new WaitForSeconds(1f);

        // Movimiento hacia atrás
        transform.position = objective.transform.position + new Vector3(0f, 0f, 15f); // Posición inicial en el lado derecho
        camazotzBody.SetActive(true); // Hacemos visible el objeto

        // Adjust this value to control how much past the objective the game object should go
        offset = 12.0f;
        // Calculate a new destination position
        targetPosition = objective.transform.position + (objective.transform.position - transform.position).normalized * offset;

        tiempoInicio = Time.time;
        transform.LookAt(objective.transform.position);
        while (Time.time - tiempoInicio < 1.5f)
        {
            ultSpeedVFX.PlayUltSpeedVFX();
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, 20 * Time.deltaTime);
            HandleAreaOfEffectDamage(2);
            yield return null;
        }

        StartCoroutine(ResetBooleanParametersAfterDelay(currentAnimationBool, "None", 2f));

    }

    // Q11
    private void HandleSoulDevourerAttackState()
    {
        if (!isInMidAttack && soulDevourerCooldown <= 0.0f)
        {

            playerDistance = Vector3.Distance(transform.position, objective.transform.position);
            if (playerDistance <= 25)
            {
                agent.speed = 0;
                agent.stoppingDistance = playerDistance;
                isInMidAttack = true;
                StartCoroutine(UltimateAttack());
                AttacksSoftReset();
                soulDevourerCooldown = 20.0f; // Set a 20-second cooldown for Soul Devourer attack
                StartCoroutine(ResetCooldown("SoulDevourerCooldown"));
                ChangeState(State.SecondPhaseState);
            }
            else
            {
                ChangeState(State.SecondPhaseState);
            }
        }
        else
        {
            ChangeState(State.SecondPhaseState);
        }
    }

    // Q12
    private void HandlePhaseTransitionState()
    {
        if (healthSystem.currentHealth > healthSystem.maxHealth / 2)
        {
            ChangeState(State.FirstPhaseState);
        }
        else if (healthSystem.currentHealth <= healthSystem.maxHealth / 2)
        {
            isInSecondPhase = true;
            ChangeState(State.SecondPhaseState);
        }
        else if (healthSystem.currentHealth <= 0)
        {
            ChangeState(State.CamazotzDeathState);
        }
    }

    private void HandleCamazotzDeathState()
    {
        Debug.Log("Camazotz Death");
        currentAnimationBool = "Death";
        animator.SetBool(currentAnimationBool, true);
        ResetBooleanParametersAfterDelay(currentAnimationBool, "none", 2f);
        photonMatchManager.EndGame();
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
    }

    private void AttacksSoftReset()
    {
        StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        agent.speed = enemyStats.movementAttributes.movementSpeed;
        agent.stoppingDistance = shortRange;
        yield return new WaitForSeconds(2.0f);
    }

    private void PhaseChecker(int attackIndex)
    {
        if (attackIndex == 0)
        {
            if (isInSecondPhase)
            {
                ChangeState(State.SecondPhaseState);
            }
            else
            {
                ChangeState(State.FirstPhaseState);
            }
        }
        else
        {
            ChangeState(State.PhaseTransitionState);
        }
    }

    private void HandleInputDebug()
    {
        
    }

    private IEnumerator ResetCooldown(string cooldownType)
    {
        float cooldownTime = 0.0f;

        switch (cooldownType)
        {
            case "BasicAttackCooldown":
                cooldownTime = 5.0f;
                break;
            case "SoulEaterCooldown":
                cooldownTime = 10.0f;
                break;
            case "UpsideDownWorldCooldown":
                cooldownTime = 15.0f;
                break;
            case "InfernalScreechCooldown":
                cooldownTime = 10.0f;
                break;
            case "SoulDevourerCooldown":
                cooldownTime = 25.0f;
                break;
        }

        yield return new WaitForSeconds(cooldownTime);
        switch (cooldownType)
        {
            case "BasicAttackCooldown":
                basicAttackCooldown = 0.0f;
                break;
            case "SoulEaterCooldown":
                soulEaterCooldown = 0.0f;
                break;
            case "UpsideDownWorldCooldown":
                upsideDownWorldCooldown = 0.0f;
                break;
            case "InfernalScreechCooldown":
                infernalScreechCooldown = 0.0f;
                break;
            case "SoulDevourerCooldown":
                soulDevourerCooldown = 0.0f;
                break;
        }
    }
}
