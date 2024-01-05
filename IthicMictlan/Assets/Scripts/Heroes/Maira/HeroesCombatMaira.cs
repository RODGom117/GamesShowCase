using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heroes.Maira
{
    public class HeroesCombatMaira : MonoBehaviour, IHeroCombat
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

        private float followUpAttackTimer = 0.0f; //Not used
        private float attackDamage;
        public float attackDamageMultiplier = 1.0f;
        private float attackAoE;
        private string currentAttack;
        
        private float attackSpeed;
        private float attackSpeedMultiplier = 1.0f;


        public TextMeshProUGUI combatStateText;
        [SerializeField] float comboTime = 0.2f;

        private PlayerManager playerManager; //Not used
        public bool IsInCombatMode;
        
    
        //For hitboxes
        Vector3 _currentAttackDirection;

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

        
        [SerializeField] private MairaSounds mairaSounds;
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

            attackSpeed = _heroStats.combatAttributes.attackSpeed;

            
            _currentAttackDirection = transform.position;
            
            
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
            if (primaryAbilityCooldownImage != null && primaryAbilityCooldownText != null)
            {
                primaryAbilityCooldownImage.enabled = true;
                primaryAbilityCooldownText.enabled = true;
            }
            primaryAbilityCooldown = _heroStats.abilityAttributes.primaryAbility.cooldown;

            _currentAttackDirection = -transform.up;
            //We call on this with 0f spheresize to wait with hitbox until Maira lands.
            StartAttackAnimation(currentAttack, primaryAbility, _currentAttackDirection );
        }


        private void HandleSecondaryAbility()
        {
            currentAttack = "SecondaryAbility";
            if (secondaryAbilityCooldownImage != null && secondaryAbilityCooldownText != null)
            {
                secondaryAbilityCooldownImage.enabled = true;
                secondaryAbilityCooldownText.enabled = true;
            }
            secondaryAbilityCooldown = _heroStats.abilityAttributes.secondaryAbility.cooldown;

            _currentAttackDirection = Vector3.zero;
            StartAttackAnimation(currentAttack, secondaryAbility, _currentAttackDirection);
        }

        private void HandleUltimateAbility()
        {
            currentAttack = "UltimateAbility";
            if (ultimateAbilityCooldownImage != null && ultimateAbilityCooldownText != null)
            {
                ultimateAbilityCooldownImage.enabled = true;
                ultimateAbilityCooldownText.enabled = true;
            }

            ultimateAbilityCooldown = _heroStats.abilityAttributes.ultimateAbility.cooldown;
            
            _currentAttackDirection = Vector3.zero;
            StartAttackAnimation(currentAttack, ultimateAbility, _currentAttackDirection);
        }

        private void UpdateCooldowns()
        {
            HandleAbilityCooldown(primaryAbilityCooldownImage, primaryAbilityCooldownText, ref primaryAbilityCooldown);
            HandleAbilityCooldown(secondaryAbilityCooldownImage, secondaryAbilityCooldownText, ref secondaryAbilityCooldown);
            HandleAbilityCooldown(ultimateAbilityCooldownImage, ultimateAbilityCooldownText, ref ultimateAbilityCooldown);
        }
    
        public void HandleAttackStateMachine()
        {
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
                        _currentAttackDirection = transform.forward * 2;
                       StartAttackAnimation(currentAttack, primaryAttack, _currentAttackDirection);
                       mairaSounds.PlayBasicAttackVoice();
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {

                    if (primaryAbilityCooldown <= 0.0f)
                    {
                        HandlePrimaryAbility();
                        mairaSounds.PlayHability1Voice();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (secondaryAbilityCooldown <= 0.0f)
                    {
                        HandleSecondaryAbility();
                        mairaSounds.PlayHability2Voice();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (ultimateAbilityCooldown <= 0.0f)
                    {
                        HandleUltimateAbility();
                        mairaSounds.PlayUltimateVoice();
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


        private void StartAttackAnimation(string attackAnimationName, List<HeroAttackObject> attackType,Vector3 direction)
        {
            if (attackType.Count <= comboCounter)
            {
                comboCounter = 0;
                anim.SetInteger("AttackCombo", 0);                

            }
            IsInCombatMode = true;
            if ((Time.time - lastComboEnd > 0.1f  && comboCounter < attackType.Count))
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
                    /*if (attackAnimationName == "PrimaryAttack")
                    {
                        anim.speed = attackSpeed * attackSpeedMultiplier;
                    }
                    else
                    {
                        anim.speed = attackSpeed;
                    }*/
                    attackDamage = attackType[comboCounter].damage * attackDamageMultiplier;
                    Debug.Log($"Current attack is dealing {attackDamage} damage");
                    attackAoE = attackType[comboCounter].areaOfEffect;
                    HandleAreaOfEffectDamage(attackAoE, direction);
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

            IsInCombatMode = false;
            ExitAttack(currentAttack);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            
            Gizmos.DrawWireSphere(transform.position + _currentAttackDirection, attackAoE);
        }
        
        public void TriggerAreaOfEffectDamageForPrimaryAbility()
        {
            if (_photonView.IsMine)
            {
                Debug.Log("Inside TriggerAreaOfEffectDamageForPrimaryAbility");
                IsInCombatMode = true;
                HandleAreaOfEffectDamage(primaryAbility[0].areaOfEffect, _currentAttackDirection);
                IsInCombatMode = false;
            }
        }


        public void TriggerTauntForSecondaryAbility()
        {
            if (_photonView.IsMine)
            {
                Debug.Log("Triggering Taunt for Maira");
                IsInCombatMode = true;
                Collider[] hitColliders = Physics.OverlapSphere(transform.position + _currentAttackDirection,
                    secondaryAbility[0].areaOfEffect);
                foreach (Collider collider in hitColliders)
                {
                    if (collider.gameObject.CompareTag("Enemy"))
                    {
                        // TODO Refactor this to general enemy Script
                        ChanequeEnemy chanequeEnemy = collider.GetComponent<ChanequeEnemy>();

                        if (chanequeEnemy != null)
                        {
                            chanequeEnemy.ChangeTarget(transform);
                        }
                    }
                }

                IsInCombatMode = false;
            }
        }
        
        //We might want a specific function for each ability, but for now this will do. // Jojo
        private void HandleAreaOfEffectDamage(float sphereSize, Vector3 direction)
        {
            if (IsInCombatMode)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position + direction, sphereSize, enemyLayerMask);
                Debug.Log("Amount of colliders hit: " + hitColliders.Length);
                foreach (Collider collider in hitColliders)
                {
                    if (collider.gameObject.CompareTag("Enemy"))
                    {
                        // Check if the collided object has a HealthSystem component
                        HealthSystem healthSystem = collider.GetComponent<HealthSystem>();
                        BossHealthSystem bossHealthSystem = collider.GetComponent<BossHealthSystem>();
                        if (healthSystem != null)
                        {
                            // Apply damage from the current attack
                            healthSystem.TakeDamage(attackDamage);
                            mairaSounds.PlayHitVoice();

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
                    cooldownTime = primaryAbilityCooldown;
                    cooldownImage = primaryAbilityCooldownImage;
                    cooldownText = primaryAbilityCooldownText;
                    break;
                case "SecondaryAbility":
                    cooldownTime = secondaryAbilityCooldown;
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
        
        public void InsideWrestlingRing()
        {
            attackSpeedMultiplier += 1.0f;
            attackDamageMultiplier += 1.0f;
            Debug.Log("maira is inside wrestling ring");
        }
        public void OutsideWrestlingRing()
        {
            attackSpeedMultiplier -= 1.0f;
            attackDamageMultiplier -= 1.0f;
            Debug.Log("Maira has left wrestling ring");
        }
        void ExitAttack(string attackTagName)
        {
            if (attackTagName != null)
            {
                if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && anim.GetCurrentAnimatorStateInfo(0).IsTag(attackTagName))
                {
                    Debug.Log("Exiting attack for " + attackTagName);
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
