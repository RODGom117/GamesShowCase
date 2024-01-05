using UnityEngine;

namespace Heroes.Maira
{
    public class PlayerManagerMaira : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public HeroesCombatMaira heroesCombatScript;
        public HeroStats heroStats;
        private bool isMoving;

        void Start()
        {
            isMoving = true;
            HealthSystem healthSystem = GetComponent<HealthSystem>();
            healthSystem.InitializeHealth(heroStats.healthAttributes.maxHealth);
            playerMovement = GetComponent<PlayerMovement>();
            heroesCombatScript = GetComponent<HeroesCombatMaira>();
        }

        void Update()
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
