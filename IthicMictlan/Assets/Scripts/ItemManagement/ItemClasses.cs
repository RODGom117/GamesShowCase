using System.Collections;
using UnityEngine;

// Regular consumer items, which restore health or mana to the player
// The values for health and mana are meant to be added to the player's total
// values can be 0
[System.Serializable]
public class ConsumableItem : Item {   
    // Inherit parameters from parent class
    public float healthRestored;

    // Model tied to the item 
    public GameObject itemPrefab;

    // Spirte tied to the item 
    public Sprite itemSprite;

    public void applyEffect(HealthSystem playerHealth, Transform playerTransform){
        // heal the player by the specified amount
        playerHealth.HealPlayer(healthRestored);
        // Instantiate the item's prefab for the hover effect
        GameObject prefabTemp = GameObject.Instantiate(itemPrefab, playerTransform.position + Vector3.up * 2.0f, playerTransform.rotation);
        // Set the prefab's parent to the player's transform
        prefabTemp.transform.parent = playerTransform;
        // Start the coroutine to get rid of the item
        ItemCoroutines.Instance.CallDestroyHover(prefabTemp);
    }

}

// Items that buff player's base stats
// Values for each buff are meant to be multiplied by the player's stats. If there is no buff, the value should be 1, not 0
[System.Serializable]
public class BuffConsumable : Item{
    // Health attributes
    public float healthBuff;
    public float defenseBuff;
    // Movement attributes
    public float fallSpeed;
    public float movSpeedBuff;
    public float jumpHeightBuff;
    // Combat attributes
    public float dmgBuff;
    public float atkSpeedBuff;
    public float critChanceBuff;

    // Spirte tied to the item 
    public Sprite itemSprite;
    // Model tied to the item 
    public GameObject itemPrefab;

    public void applyEffect(HeroStats heroStats){
        
        // buffing health attributes
        heroStats.healthAttributes.maxHealth *= healthBuff;
        heroStats.combatAttributes.defense *= defenseBuff;
        // buff movement attributes
        heroStats.movementAttributes.movementSpeed *= movSpeedBuff;
        heroStats.movementAttributes.gravitySpeed *= fallSpeed;
        heroStats.movementAttributes.jumpHeight *= jumpHeightBuff;
        // buff combat attributes
        heroStats.combatAttributes.basicAttackDamage *= dmgBuff;
        heroStats.combatAttributes.attackSpeed *= atkSpeedBuff;
        heroStats.combatAttributes.criticalHitChance *= critChanceBuff;

    }
    
}

// ----------------------------------------------------------------
//----------------------------------------------------------------

public class JaguarWarriorItem : Item{   
    // Inherit parameters from parent class
    public void applyEffect(){
        Debug.Log("UsedJWItem");
    }

}

public class EagleWarriorItem : Item{   
    // Inherit parameters from parent class
    public void applyEffect(){
        Debug.Log("UsedEWItem");
    }

}

public class ChamanItem : Item{   
    // Inherit parameters from parent class
    public void applyEffect(){
        Debug.Log("UsedChamanItem");
    }
}

public class OwlWitchItem : Item{   
    // Inherit parameters from parent class

    public void applyEffect(){
        Debug.Log("UsedOWItem");
    }

}

public class ActiveItem : Item {

    public void applyEffect(){
        Debug.Log("Activated spacebar item");
    }
}


