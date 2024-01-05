using Heroes.Rosa;
using UnityEngine;

namespace Heroes
{
    public class PlayerManagerRosa : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public HeroesCombatRosa heroesCombatScript;
        public HeroStats heroStats;
        private bool isMoving = true;

        void Start()
        {
            HealthSystem healthSystem = GetComponent<HealthSystem>();
            healthSystem.InitializeHealth(heroStats.healthAttributes.maxHealth);
            playerMovement = GetComponent<PlayerMovement>();
            heroesCombatScript = GetComponent<HeroesCombatRosa>();
        }

        void Update()
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
           playerMovement.HandleMovement();

           heroesCombatScript.HandleAttackStateMachine();

        
        }
    }
}
