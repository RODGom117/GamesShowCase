using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace Heroes.Teo
{
    public class PlayerManagerTeo : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public HeroesCombatTeo heroesCombatScript;
        public HeroStats heroStats;
        private bool isMoving;
        [SerializeField] private PhotonView photonView;
        [SerializeField] private GameObject canvas;

        private HealthSystem healthSystem;
        [SerializeField] private Canvas GameplayUI;
        [SerializeField] private TeoSounds teoSounds;

        void Start()
        {
            isMoving = true;
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.InitializeHealth(heroStats.healthAttributes.maxHealth);
            playerMovement = GetComponent<PlayerMovement>();
            heroesCombatScript = GetComponent<HeroesCombatTeo>();
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                CheckDeath();
                playerMovement.HandleMovement();

                heroesCombatScript.HandleAttackStateMachine();
            }
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
                    GetComponent<PlayerManagerTeo>().enabled = false;
                    GetComponent<HeroesCombatTeo>().enabled = false;
                    GetComponent<PlayerMovement>().enabled = false;
                    GetComponent<HealthSystem>().enabled = false;
                    GameplayUI.enabled = false;
                    UISelectScreenManager.instance.ToggleDeathScreen(1);
                    StartCoroutine(RespawnCoroutine());
                    teoSounds.PlayDefeatVoice();
                }
            }
        }

        public void Respawn()
        {
            if (photonView.IsMine)
            {
                GetComponent<Animator>().SetBool("isDead", false);
                GetComponent<PlayerManagerTeo>().enabled = true;
                GetComponent<HeroesCombatTeo>().enabled = true;
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
