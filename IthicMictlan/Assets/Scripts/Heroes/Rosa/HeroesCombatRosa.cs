using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Heroes.Rosa
{
    public class HeroesCombatRosa : MonoBehaviour, IHeroCombat
    {
        public List<HeroAttackObject> primaryAttack;
        public List<HeroAttackObject> primaryAbility;
        public List<HeroAttackObject> secondaryAbility;
        public List<HeroAttackObject> ultimateAbility;

        private float lastClickedTime;
        private float lastComboEnd;
        private int comboCounter;
        private Animator anim;
        private RuntimeAnimatorController originalAnim;
        [SerializeField] private UIManager uiManager;

        private float followUpAttackTimer = 0.0f; //Not used
        private float attackDamage;
        private float attackAoE;
        private string currentAttack;
        
        private float attackSpeed;
        private float attackSpeedMultiplier = 1.0f;

        private float coolDownMultiplier = 1.0f;

        private float _totalMadness;
        private float currentMadness;
        private float attackDamageMultiplier;
        
        //Rosa Primary Ability
        [SerializeField] Feather featherPrefab;
        public float throwForce = 10f;
        public int numberOfFeathers = 5; // number of feathers to fire
        public float spreadAngle = 45f; // angle of spread in degrees

        public TextMeshProUGUI combatStateText;
        [SerializeField] float comboTime = 0.2f;

        private PlayerManager playerManager; //Not used
        public bool IsInCombatMode;
    
    
        //For UI
        [SerializeField] private int primaryAbilityTimerUI;
        [SerializeField] private int secondaryAbilityTimerUI;
        [SerializeField] private int UltimateAbilityTimerUI;

        [SerializeField] LayerMask enemyLayerMask;


        [SerializeField] HeroStats _heroStats;
        
        [SerializeField] private PhotonView _photonView;
    
        private float basicAttackCooldown = 0.0f;
        private float primaryAbilityCooldown = 0.0f;
        private float secondaryAbilityCooldown = 0.0f;
        private float ultimateAbilityCooldown = 0.0f;
    
    
        //For UI, TODO maybe move this into its own script.
        public Image primaryAbilityCooldownImage;
        public Image secondaryAbilityCooldownImage;
        public Image ultimateAbilityCooldownImage;

        private TextMeshProUGUI primaryAbilityCooldownText;
        private TextMeshProUGUI secondaryAbilityCooldownText;
        private TextMeshProUGUI ultimateAbilityCooldownText;

        [SerializeField] private RosaSounds rosaSounds;
        private enum HeroesAttackState
        {
            Idle,
            PrimaryAttack,
            PrimaryAbility,
            SecondaryAbility,
            UltimateAbility
        }

        private HeroesAttackState currentHeroesAttackState = HeroesAttackState.Idle;

        // Start is called before the first frame update
        void Start()
        {   
            //UI initialization
            InitializeCooldownUI(primaryAbilityCooldownImage, ref primaryAbilityCooldownText);
            InitializeCooldownUI(secondaryAbilityCooldownImage, ref secondaryAbilityCooldownText);
            InitializeCooldownUI(ultimateAbilityCooldownImage, ref ultimateAbilityCooldownText);
            //Setting ultimate to not usable as madness is below 100 when game starts
            if (_totalMadness <= 100f)
            {
                ultimateAbilityCooldownImage.enabled = true;
            }
            attackSpeed = _heroStats.combatAttributes.attackSpeed;
            
            combatStateText.enabled = false; // Hide the debug text
            anim = GetComponent<Animator>();
            originalAnim = anim.runtimeAnimatorController;
        
            playerManager = GetComponentInParent<PlayerManager>(); // Get the PlayerManager reference
        
        }
    
        private void InitializeCooldownUI(Image cooldownImage, ref TextMeshProUGUI cooldownText)
        {
            if (cooldownImage != null)
            {
                cooldownText = cooldownImage.GetComponentInChildren<TextMeshProUGUI>();

                if (cooldownText != null)
                {
                    cooldownText.text = "";
                    cooldownImage.enabled = false;
                    cooldownText.enabled = false;
                }
            }
        }

        private void HandleAbilityCooldown(Image cooldownImage, TextMeshProUGUI cooldownText, ref float cooldown)
        {
            if (cooldownImage != null && cooldownText != null)
            {
                if (cooldown > 0.0f)
                {
                    cooldown -= Time.deltaTime;
                    if (cooldown <= 0.0f)
                    {
                        cooldown = 0.0f;
                        cooldownImage.enabled = false; // Hide the cooldown image
                        cooldownText.enabled = false; // Hide the cooldown text
                    }
                    else
                    {
                        cooldownText.text = cooldown.ToString("F1"); // Display cooldown timer
                    }
                }
            }
        }

        private void HandlePrimaryAbility()
        {
            currentAttack = "PrimaryAbility";
            currentMadness = _heroStats.abilityAttributes.primaryAbility.madnessValue;
            if (primaryAbilityCooldownImage != null && primaryAbilityCooldownText != null)
            {
                primaryAbilityCooldownImage.enabled = true;
                primaryAbilityCooldownText.enabled = true;
            }
            primaryAbilityCooldown = _heroStats.abilityAttributes.primaryAbility.cooldown;
            StartAttackAnimation(currentAttack, primaryAbility);
        }



        private void HandleSecondaryAbility()
        {
            currentAttack = "SecondaryAbility";
            currentMadness = _heroStats.abilityAttributes.secondaryAbility.madnessValue;
            if (secondaryAbilityCooldownImage != null && secondaryAbilityCooldownText != null)
            {
                secondaryAbilityCooldownImage.enabled = true;
                secondaryAbilityCooldownText.enabled = true;
            }
            secondaryAbilityCooldown = _heroStats.abilityAttributes.secondaryAbility.cooldown;
            TeleportToClosestEnemy(secondaryAbility[0].areaOfEffect*20f);
            StartAttackAnimation(currentAttack, secondaryAbility);
        }

        private void HandleUltimateAbility()
        {
            currentAttack = "UltimateAbility";
            currentMadness = _heroStats.abilityAttributes.ultimateAbility.madnessValue;
            if (ultimateAbilityCooldownImage != null && ultimateAbilityCooldownText != null && _totalMadness >= 100.0f)
            {
                ultimateAbilityCooldownImage.enabled = true;
                ultimateAbilityCooldownText.enabled = true;
            }
            ultimateAbilityCooldown = _heroStats.abilityAttributes.ultimateAbility.cooldown;
            StartAttackAnimation(currentAttack, ultimateAbility);

            coolDownMultiplier = 0.5f;
            attackSpeedMultiplier = 1.5f;
            _totalMadness = 0.0f;
            uiManager.SetMadness(_totalMadness);
            StartCoroutine(ReturnToNormal());
        }

        IEnumerator ReturnToNormal()
        {
            yield return new WaitForSeconds(10f);
            coolDownMultiplier = 1.0f;
            attackSpeedMultiplier = 1.0f;
        }

        private void UpdateCooldowns()
        {
            HandleAbilityCooldown(primaryAbilityCooldownImage, primaryAbilityCooldownText, ref primaryAbilityCooldown);
            HandleAbilityCooldown(secondaryAbilityCooldownImage, secondaryAbilityCooldownText, ref secondaryAbilityCooldown);
            HandleAbilityCooldown(ultimateAbilityCooldownImage, ultimateAbilityCooldownText, ref ultimateAbilityCooldown);
        }
    
        public void HandleAttackStateMachine()
        {
            if (uiManager != null)
            {
                _totalMadness = uiManager.GetMadness();
            }

            if (_totalMadness >= 99f)
            {
                if (ultimateAbilityCooldownImage != null && ultimateAbilityCooldownText != null)
                {
                    ultimateAbilityCooldownImage.enabled = false;
                }
            }
            else
            {
                ultimateAbilityCooldownImage.enabled = true;;
            }
                
 
            if(currentAttack != null)
            {
                ExitAttack(currentAttack);
            }
            if (currentHeroesAttackState == HeroesAttackState.Idle)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (basicAttackCooldown <= 0.0f)
                    {
                        currentAttack = "PrimaryAttack";
                        basicAttackCooldown = 0f; 
                        StartAttackAnimation(currentAttack, primaryAttack);
                        rosaSounds.PlayBasicAttackVoice();
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {

                    if (primaryAbilityCooldown <= 0.0f)
                    {
                        HandlePrimaryAbility();
                        rosaSounds.PlayHability1Voice();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (secondaryAbilityCooldown <= 0.0f)
                    {
                        HandleSecondaryAbility();
                        rosaSounds.PlayHability2Voice();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (uiManager.GetMadness() >= 100.0f)
                    {
                        if (ultimateAbilityCooldown <= 0.0f)
                        {
                            HandleUltimateAbility();
                            rosaSounds.PlayUltimateVoice();
                        }
                    }
                }
                else if(Input.GetKeyDown(KeyCode.E))
                {
                    HandleXoloCatch();
                }
            }
        }
        
        private void HandleXoloCatch()
        {
            Collider[] xolos = Physics.OverlapSphere(transform.position, 2f);

            if(xolos != null)
            {
                foreach (Collider xolo in xolos)
                {
                    if(xolo.CompareTag("Xolo"))
                    {
                        if(xolo.GetComponent<XoloitzcuintleController>() != null)
                        {
                            xolo.GetComponent<XoloitzcuintleController>().SetWasCatched();
                        }

                    }
                }
            }
        }

        private void StartAttackAnimation(string attackAnimationName, List<HeroAttackObject> attackType)
        {
            
            if (attackType.Count <= comboCounter)
            {
                comboCounter = 0;
                anim.SetInteger("AttackCombo", 0);                
            }
            IsInCombatMode = true;
            
            if ((Time.time - lastComboEnd > 0.1f && comboCounter < attackType.Count))
            {   
                CancelInvoke("EndCombo");
                if (Time.time - lastClickedTime >= comboTime)
                {
                    if(attackType.Count > 1)
                    {
                        anim.SetInteger("AttackCombo", comboCounter + 1);
                    }
                    else
                    {
                        if (attackAnimationName == "PrimaryAbility")
                        {
                            anim.SetBool("IsPrimaryAbility", true);
                        }
                        else if (attackAnimationName == "SecondaryAbility")
                        {
                            anim.SetBool("IsSecondaryAbility", true);
                        }
                        else if (attackAnimationName == "UltimateAbility")
                        {
                            anim.SetBool("IsUltimateAbility", true);
                        }
                    }

                    currentMadness = attackType[comboCounter].madnessValue;
                    //TODO add attackspeed to new controller
                    //anim.speed = attackSpeed * attackSpeedMultiplier;
                    //Todo fix bug for finding damage.
                    attackDamage = attackType[comboCounter].damage * attackDamageMultiplier;
                    Debug.Log($"Current attack is dealing {attackDamage} damage");
                    attackAoE = attackType[comboCounter].areaOfEffect;
                    HandleAreaOfEffectDamage();
                    comboCounter++;
                    lastClickedTime = Time.time;
                    
                    if (comboCounter >= attackType.Count)
                    {
                        comboCounter = 0;
                    }
                }
            }
            Debug.Log($"Starting cooldown for {currentAttack}.");
            StartCoroutine(ResetCooldown(currentAttack));
            
            // Wait for the animation to finish
            IsInCombatMode = false;
            ExitAttack(currentAttack);
        }
        
        
        //TODO: fix hitbox for primary ability
        /*private void StartAttackAnimationForPrimaryAbility(string attackAnimationName, List<HeroAttackObject> attackType)
        {
            IsInCombatMode = true;
            anim.runtimeAnimatorController = attackType[comboCounter].animatorOV;
            anim.speed = attackSpeed * attackSpeedMultiplier;
            anim.Play(attackAnimationName, 0, 0);
            attackDamage = attackType[comboCounter].damage;
            Debug.Log($"Current attack is dealing {attackDamage} damage");
            lastClickedTime = Time.time;
            Debug.Log($"Starting cooldown for {currentAttack}.");
            StartCoroutine(ResetCooldown(currentAttack));

            // Wait for the animation to finish
            yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
            IsInCombatMode = false;
            ExitAttack(currentAttack);
        }*/
        
        
        private void SpawnFeathersForPrimaryAbility()
        {
            if (_photonView.IsMine)
            {
                //Feather newFeather = Instantiate(featherPrefab, transform.position, Quaternion.identity).GetComponent<Feather>();
                for (int i = 0; i < numberOfFeathers; i++)
                {
                    // Calculate rotation offset for this feather
                    float offsetAngle = spreadAngle * ((float)i / (numberOfFeathers - 1) - 0.5f);
                    Quaternion rotation = Quaternion.Euler(new Vector3(0, offsetAngle, 0)) * transform.rotation;


                    // Instantiate a feather object at Rosa's position
                    string featherPath = "Objects/Rosa/" + featherPrefab.name;
                    
                    GameObject featherObject = PhotonNetwork.Instantiate(featherPath, transform.position, transform.rotation);
                    if (featherObject == null)
                    {
                        featherObject = PhotonNetwork.Instantiate("Assets/Resources/Objects/Feather.prefab", transform.position, transform.rotation);
                    }
                    
                    Feather feather = featherObject.GetComponent<Feather>();
                    feather.rosaUIManager = uiManager;
                    feather.transform.rotation = rotation * Quaternion.Euler(90f, 0f, 0f);

                    // Get direction from rotation
                    Vector3 direction = rotation * Vector3.forward;
                    feather.featherDirection = direction * 30f;
                    
                    feather.Launch();
                }
            }
        }
        private void TeleportToClosestEnemy(float abilityRange)
        {
            if (_photonView.IsMine)
            {
                Vector3 center = transform.position + transform.forward * abilityRange;
                Collider[] hitColliders = Physics.OverlapSphere(center, abilityRange, enemyLayerMask);
                float closestDistanceSqr = abilityRange * abilityRange;
                Collider closestEnemy = null;
                Debug.Log(hitColliders.Length);
                foreach (var hitCollider in hitColliders)
                {
                    float distanceSqrToEnemy = (hitCollider.transform.position - transform.position).sqrMagnitude;
                    if (distanceSqrToEnemy < closestDistanceSqr)

                    {
                        closestDistanceSqr = distanceSqrToEnemy;
                        closestEnemy = hitCollider;
                    }
                }

                if (closestEnemy != null)
                {
                    Debug.Log("Rosa teleported to closest enemy");
                    CharacterController controller = GetComponent<CharacterController>();
                    //controller.transform.position = closestEnemy.transform.position + Vector3.right * 2;
                    Vector3 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;

                    // Get the exact distance to the enemy, subtracting the radius of the CharacterController
                    float radius = controller.radius;
                    float distanceToEnemy = Mathf.Sqrt(closestDistanceSqr) - radius;

                    // Include an offset to prevent the character from ending up inside another enemy
                    Vector3 offset = directionToEnemy * (closestEnemy.bounds.extents.magnitude + radius);

                    // Move to the enemy
                    controller.Move(directionToEnemy * distanceToEnemy + offset + Vector3.up);
                }
                else
                {
                    Debug.Log("Couldnt find an enemy to teleport too");
                }
            }
        }
        
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + transform.forward, attackAoE);
        }
        
        
        
        private void HandleAreaOfEffectDamage()
        {
            if (IsInCombatMode)
            {
                // Replace the 'Vector3.forward' with the actual direction you want the frontal area to be.
                Vector3 frontDirection = transform.forward;

                Collider[] hitColliders = Physics.OverlapSphere(transform.position + frontDirection, attackAoE, enemyLayerMask);

                foreach (Collider collider in hitColliders)
                {
                    if (collider.gameObject.tag == "Enemy")
                    {
                        // Check if the collided object has a HealthSystem component
                        HealthSystem healthSystem = collider.GetComponent<HealthSystem>();
                        BossHealthSystem bossHealthSystem = collider.GetComponent<BossHealthSystem>();
                        if (healthSystem != null)
                        {
                            uiManager.UpdateMadness(currentMadness);
                            _totalMadness += currentMadness;
                            // Apply damage from the current attack
                            healthSystem.TakeDamage(attackDamage);
                            rosaSounds.PlayHitVoice();
                        }
                        if (bossHealthSystem != null)
                        {
                            // Apply damage from the current attack
                            bossHealthSystem.TakeDamage(attackDamage);
                        }
                    }
                }
            }
        }

        private IEnumerator ResetCooldown(string cooldownType)
        {
            float cooldownTime = 0.0f;
            Image cooldownImage = null;
            TextMeshProUGUI cooldownText = null;

            switch (cooldownType)
            {
                case "PrimaryAttack":
                    cooldownTime = basicAttackCooldown;
                    break;
                case "PrimaryAbility":
                    cooldownTime = primaryAbilityCooldown * coolDownMultiplier;
                    cooldownImage = primaryAbilityCooldownImage;
                    cooldownText = primaryAbilityCooldownText;
                    break;
                case "SecondaryAbility":
                    cooldownTime = secondaryAbilityCooldown * coolDownMultiplier;
                    cooldownImage = secondaryAbilityCooldownImage;
                    cooldownText = secondaryAbilityCooldownText;
                    break;
                case "UltimateAbility":
                    cooldownTime = ultimateAbilityCooldown;
                    cooldownImage = ultimateAbilityCooldownImage;
                    cooldownText = ultimateAbilityCooldownText;
                    break;
            }

            if (cooldownImage != null)
            {
                cooldownImage.enabled = true; // Show the cooldown image
            }
            if (cooldownText != null)
            {
                cooldownText.enabled = true; // Show the cooldown text
            }

            // Update the UI for each second of the cooldown
            for (float timeRemaining = cooldownTime; timeRemaining > 0.0f; timeRemaining -= 1.0f)
            {
                if (cooldownText != null)
                {
                    cooldownText.text = Mathf.RoundToInt(timeRemaining).ToString(); // Display remaining cooldown time
                }
                yield return new WaitForSeconds(1.0f); // Wait for 1 second
            }

            if (cooldownImage != null)
            {
                cooldownImage.enabled = false; // Hide the cooldown image
            }
            if (cooldownText != null)
            {
                cooldownText.text = ""; // Clear the cooldown text
                cooldownText.enabled = false; // Hide the cooldown text when the cooldown is complete
            }

            // Reset the current cooldown for the specific ability
            switch (cooldownType)
            {
                case "PrimaryAttack":
                    basicAttackCooldown = 0.0f;
                    break;
                case "PrimaryAbility":
                    primaryAbilityCooldown = 0.0f;
                    break;
                case "SecondaryAbility":
                    secondaryAbilityCooldown = 0.0f;
                    break;
                case "UltimateAbility":
                    ultimateAbilityCooldown = 0.0f;
                    break;
            }
        }


        void ExitAttack(string attackTagName)
        {
            if (attackTagName != null)
            {
                if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && anim.GetCurrentAnimatorStateInfo(0).IsTag(attackTagName))
                {
                    Invoke("EndCombo", 0f);
                }
            }
        }

        void EndCombo()
        {
            comboCounter = 0;
            anim.SetInteger("AttackCombo", 0);
            anim.SetBool("IsPrimaryAbility", false);
            anim.SetBool("IsSecondaryAbility", false);
            anim.SetBool("IsUltimateAbility", false);
            lastComboEnd = Time.time;
        }
        public IEnumerator IgnacioBuffBeam(float duration)
        {
            attackSpeedMultiplier += 1.0f;
            attackDamageMultiplier += 1.0f;

            //TODO: add glowy effect vfx here. Make it look cool pls
            yield return new WaitForSeconds(duration);
            attackSpeedMultiplier -= 1.0f;
            attackDamageMultiplier -= 1.0f;

            
        }

        public IEnumerator IgnacioBuffUltimate(float strength, float duration)
        {
            attackDamageMultiplier += strength * 0.1f;
            yield return new WaitForSeconds(duration);
            attackDamageMultiplier -= strength * 0.1f;  
            
            
            //TODO: Buff health or defense here
        }
    }
}
