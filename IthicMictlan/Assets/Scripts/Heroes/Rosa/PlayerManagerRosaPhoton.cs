using Heroes.Rosa;
using Photon.Pun;
using UnityEngine;
using System.Collections;

namespace Heroes.Rosa
{
    public class PlayerManagerRosaPhoton : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public HeroesCombatRosa heroesCombatScript;
        public HeroStats heroStats;
        private bool isMoving = true;
        [SerializeField] private PhotonView photonView;
        [SerializeField] private GameObject canvas;
        private HealthSystem healthSystem;
        [SerializeField] private Canvas GameplayUI;
        [SerializeField] private RosaSounds rosaSounds;


        void Start()
        {
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.InitializeHealth(heroStats.healthAttributes.maxHealth);
            playerMovement = GetComponent<PlayerMovement>();
            heroesCombatScript = GetComponent<HeroesCombatRosa>();
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                /*// Check if the player is in combat mode
                if (heroesCombatScript.IsInCombatMode)
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
                //Debug.Log("combat mode is: " + heroesCombatScript.IsInCombatMode.ToString());
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
                    GetComponent<PlayerManagerRosaPhoton>().enabled = false;
                    GetComponent<HeroesCombatRosa>().enabled = false;
                    GetComponent<PlayerMovement>().enabled = false;
                    GetComponent<HealthSystem>().enabled = false;
                    GameplayUI.enabled = false;
                    UISelectScreenManager.instance.ToggleDeathScreen(1);
                    StartCoroutine(RespawnCoroutine());
                    rosaSounds.PlayDefeatVoice();

                }
            }
        }

        public void Respawn()
        {
            if (photonView.IsMine)
            {
                GetComponent<Animator>().SetBool("isDead", false);
                GetComponent<PlayerManagerRosaPhoton>().enabled = true;
                GetComponent<HeroesCombatRosa>().enabled = true;
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
