using UnityEngine;
using Photon.Pun;
namespace Heroes

{
    public class PlayerManager : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public HeroesCombat heroesCombatScript;
        public HeroStats heroStats;
        private bool isMoving = true;
        [SerializeField] private PhotonView photonView;

        void Start()
        {
            HealthSystem healthSystem = GetComponent<HealthSystem>();
            healthSystem.InitializeHealth(heroStats.healthAttributes.maxHealth);
            playerMovement = GetComponent<PlayerMovement>();
            heroesCombatScript = GetComponent<HeroesCombat>();
        }

        void Update()
        {
            if(photonView.IsMine)
            {
                // Check if the player is in combat mode
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
                }
            //Debug.Log("combat mode is: " + heroesCombatScript.IsInCombatMode.ToString());
                heroesCombatScript.HandleAttackStateMachine();
            }

        
        }
    }
}
