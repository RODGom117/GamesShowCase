using Photon.Pun;
using UnityEngine;
using System.Collections;

namespace Heroes.Maira
{
    public class PlayerManagerMairaPhoton : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public HeroesCombatMaira heroesCombatScript;
        public HeroStats heroStats;
        private bool isMoving;
        [SerializeField] private PhotonView photonView;
        [SerializeField] private GameObject canvas;

        private HealthSystem healthSystem;
        [SerializeField] private Canvas GameplayUI;
        [SerializeField] private MairaSounds mairaSounds;

        void Start()
        {
            isMoving = true;
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.InitializeHealth(heroStats.healthAttributes.maxHealth);
            playerMovement = GetComponent<PlayerMovement>();
            heroesCombatScript = GetComponent<HeroesCombatMaira>();
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                // Check if the player is in combat mode
                /*if (heroesCombatScript.IsInCombatMode)
                {
                    isMoving = false; // Disable movement
                }
                else
                {
                    isMoving = true; // Enable movement
                }
    
                if (isMoving)
                {
                    playerMovement.HandleMovement();
                }*/
                CheckDeath();
                playerMovement.HandleMovement();

                heroesCombatScript.HandleAttackStateMachine();
            }

        }
        public void ActivationUI()
        {
            if (photonView.IsMine)
            {
                canvas.SetActive(true);
            }
        }

        public void CheckDeath()
        {
            if(photonView.IsMine)
            {
                if (healthSystem.currentHealth <= 0)
                {
                    GetComponent<Animator>().SetBool("isDead", true);
                    GetComponent<PlayerManagerMairaPhoton>().enabled = false;
                    GetComponent<HeroesCombatMaira>().enabled = false;
                    GetComponent<PlayerMovement>().enabled = false;
                    GetComponent<HealthSystem>().enabled = false;
                    GameplayUI.enabled = false;
                    UISelectScreenManager.instance.ToggleDeathScreen(1);
                    StartCoroutine(RespawnCoroutine());
                    mairaSounds.PlayDefeatVoice();
                }
            }
        }

        public void Respawn()
        {
            if (photonView.IsMine)
            {
                GetComponent<Animator>().SetBool("isDead", false);
                GetComponent<PlayerManagerMairaPhoton>().enabled = true;
                GetComponent<HeroesCombatMaira>().enabled = true;
                GetComponent<PlayerMovement>().enabled = true;
                GetComponent<HealthSystem>().enabled = true;
                GameplayUI.enabled = true;
                UISelectScreenManager.instance.ToggleDeathScreen(0);
                healthSystem.HealPlayer(heroStats.healthAttributes.maxHealth);
            }
        }

        private IEnumerator RespawnCoroutine()
        {
            int timer = 5;
            while (timer >= 0)
            {
                UISelectScreenManager.instance.SetTimer(timer);
                yield return new WaitForSeconds(1f);
                timer--;
            }
            Respawn();
        }
    }
}
