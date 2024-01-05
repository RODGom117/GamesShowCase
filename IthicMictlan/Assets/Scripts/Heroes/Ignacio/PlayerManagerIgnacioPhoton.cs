using Photon.Pun;
using UnityEngine;

namespace Heroes.Ignacio
{
    public class PlayerManagerIgnacioPhoton : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public HeroesCombatIgnacio heroesCombatScript;
        public HeroStats heroStats;
        private bool isMoving;
        [SerializeField] private PhotonView photonView;

        void Start()
        {
            isMoving = true;
            HealthSystem healthSystem = GetComponent<HealthSystem>();
            healthSystem.InitializeHealth(heroStats.healthAttributes.maxHealth);
            playerMovement = GetComponent<PlayerMovement>();
            heroesCombatScript = GetComponent<HeroesCombatIgnacio>();
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
                playerMovement.HandleMovement();

                heroesCombatScript.HandleAttackStateMachine();
            }

        }
    }
}
