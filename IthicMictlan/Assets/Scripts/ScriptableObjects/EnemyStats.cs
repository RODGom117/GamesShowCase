using UnityEngine;

// EnemyStats.cs
// Version: 1.0
// This script defines the attributes for an enemy character in the game.
// If you change the script, adjust the version number above.

// Serializable class for character-specific attributes
[System.Serializable]
public class EnemyAttributes
{
    public string enemyName = "default";
    public int CacaoDrop = 10;
    public int maxSpawn = 10;
    public GameObject enemyPrefab;
}

// Serializable class for movement attributes
[System.Serializable]
public class EnemyMovementAttributes
{
    public float movementSpeed = 10f;                  // Character movement speed
    public float gravity = 5f;                   // Gravity applied to the character
}

// Serializable class for health attributes
[System.Serializable]
public class EnemyHealthAttributes
{
    public float maxHealth = 200;                    // Maximum health
    public float healthRegen = 0.1f;                 // Health regeneration rate
}

// Serializable class for combat attributes
[System.Serializable]
public class EnemyCombatAttributes
{
    public float basicAttackDamage = 10f;            // Basic attack damage
    public float attackSpeed = 1f;                  // Attack speed
    public int numberOfSkills = 1;                  // Number of skills
}

// Serializable class for enemy vision range to detect player
[System.Serializable]
public class EnemyVisionAttributes
{
    public float visionRange = 10f;                  // Vision range
}

// Create a ScriptableObject asset for enemy stats
[CreateAssetMenu(fileName = "FILENAME", menuName = "Enemy Stats", order = 0)]
public class EnemyStats : ScriptableObject
{
    public EnemyAttributes baseAttributes;       // Character-specific attributes
    public EnemyMovementAttributes movementAttributes;    // Movement attributes
    public EnemyHealthAttributes healthAttributes;        // Health attributes
    public EnemyCombatAttributes combatAttributes;        // Combat attributes
    public EnemyVisionAttributes visionAttributes;        // Vision attributes

    // Add more attributes as needed
}
