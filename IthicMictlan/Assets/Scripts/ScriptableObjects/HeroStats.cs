using UnityEngine;

// HeroStats.cs
// Version: 1.0
// This script defines the attributes for a hero character in the game.
// If you change the script, adjust the version number above.
// Serializable class for character-specific attributes
[System.Serializable]
public class CharacterAttributes
{
    public string characterName = "default";
    public string description = "very long description";
}


// Serializable class for movement attributes
[System.Serializable]
public class MovementAttributes
{
    public float movementSpeed = 10f;                  // Character movement speed
    public float gravitySpeed = 5f;                   // Gravity applied to the character
    public float jumpHeight = 20f;                    // Jump height
    public float jumpAgility = 5f;                   // Jump agility
    public float sprintSpeedMultiplier = 2f;          // Sprint speed multiplier
    public float sprintGradualAcceleration = 5.0f;    // Sprint acceleration speed
}

// Serializable class for health attributes
[System.Serializable]
public class HealthAttributes
{
    public float maxHealth = 200;                    // Maximum health
    public float healthRegen = 0.1f;                 // Health regeneration rate
}

// Serializable class for combat attributes
[System.Serializable]
public class CombatAttributes
{
    //List of attacks ad combos must be here. 
    public float basicAttackDamage = 10f;            // Basic attack damage
    public float attackSpeed = 1f;                  // Attack speed
    public float criticalHitChance = 1.0f;           // Crit Chance
    public float defense = 1.0f;                    // Defense
}

// Serializable class for lasso attributes
[System.Serializable]
public class LassoAttributes
{
    public float lassoCircleDiameter = 10f;          // Diameter of the lasso circle
}

[System.Serializable]
public class AbilityAttributes
{
    public HeroAttackObject primaryAbility;
    public HeroAttackObject secondaryAbility;
    public HeroAttackObject ultimateAbility;
}

// Create a ScriptableObject asset for hero stats
[CreateAssetMenu(fileName = "FILENAME", menuName = "Hero Stats", order = 0)] 
public class HeroStats : ScriptableObject
{
    public CharacterAttributes baseAttributes;       // Character-specific attributes
    public MovementAttributes movementAttributes;    // Movement attributes
    public HealthAttributes healthAttributes;        // Health attributes
    public CombatAttributes combatAttributes;        // Combat attributes
    public LassoAttributes lassoAttributes;          // Lasso attributes
    public AbilityAttributes abilityAttributes;     // Ability attributes
    

    // Add more attributes as needed

}
