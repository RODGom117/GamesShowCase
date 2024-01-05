using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Heroes;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Photon.Pun;

public class HeroesCombat : MonoBehaviour
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
    private float attackAoE;
    private string currentAttack;

    public TextMeshProUGUI combatStateText;
    [SerializeField] float comboTime = 0.2f;

    private PlayerManager playerManager; //Not used
    public bool IsInCombatMode;

    //Photon variables
    [SerializeField] private PhotonView photonView;
    
    //For UI
    [SerializeField] private int primaryAbilityTimerUI;
    [SerializeField] private int secondaryAbilityTimerUI;
    [SerializeField] private int UltimateAbilityTimerUI;

    [SerializeField] LayerMask enemyLayerMask;


    [SerializeField] HeroStats _heroStats;
    
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

    [SerializeField] private NachoSounds nachoSounds;
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
        StartAttackAnimation(currentAttack, primaryAbility);
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

        StartAttackAnimation(currentAttack, secondaryAbility);
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
        StartAttackAnimation(currentAttack, ultimateAbility);
    }

    private void UpdateCooldowns()
    {
        HandleAbilityCooldown(primaryAbilityCooldownImage, primaryAbilityCooldownText, ref primaryAbilityCooldown);
        HandleAbilityCooldown(secondaryAbilityCooldownImage, secondaryAbilityCooldownText, ref secondaryAbilityCooldown);
        HandleAbilityCooldown(ultimateAbilityCooldownImage, ultimateAbilityCooldownText, ref ultimateAbilityCooldown);
    }
    
    public void HandleAttackStateMachine()
    {
        if(photonView.IsMine)
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
                        StartAttackAnimation(currentAttack, primaryAttack);
                        nachoSounds.PlayBasicAttackVoice();
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {

                    if (primaryAbilityCooldown <= 0.0f)
                    {
                        HandlePrimaryAbility();
                        nachoSounds.PlayHability1Voice();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (secondaryAbilityCooldown <= 0.0f)
                    {
                        HandleSecondaryAbility();
                        nachoSounds.PlayHability2Voice();
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (ultimateAbilityCooldown <= 0.0f)
                    {
                        HandleUltimateAbility();
                        nachoSounds.PlayUltimateVoice();
                    }
                }
                else if(Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("E");
                    HandleXoloCatch();
                }
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
                    if(xolo.transform.GetComponent<XoloitzcuintleController>() != null)
                    {
                        Debug.Log("GOT U >:C");
                        xolo.transform.GetComponent<XoloitzcuintleController>().SetWasCatched();
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
                } else {
                    if(attackAnimationName == "PrimaryAbility")
                    {
                        anim.SetBool("IsPrimaryAbility", true);
                    } else if(attackAnimationName == "SecondaryAbility")
                    {
                        anim.SetBool("IsSecondaryAbility", true);
                    } else if(attackAnimationName == "UltimateAbility")
                    {
                        anim.SetBool("IsUltimateAbility", true);
                    }
                }
                comboCounter++;
                lastClickedTime = Time.time;
                attackDamage = attackType[comboCounter].damage;
                Debug.Log($"Current attack is dealing {attackDamage} damage");
                attackAoE = attackType[comboCounter].areaOfEffect;
                HandleAreaOfEffectDamage();

                if (comboCounter >= attackType.Count)
                {
                    comboCounter = 0;
                }
            }
        }
        Debug.Log($"Starting cooldown for {currentAttack}.");
        StartCoroutine(ResetCooldown(currentAttack));


        // Wait for the animation to finish
       // yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        IsInCombatMode = false;
        ExitAttack(currentAttack);
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
                        // Apply damage from the current attack
                        healthSystem.TakeDamage(attackDamage);
                        nachoSounds.PlayHitVoice();
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
}
