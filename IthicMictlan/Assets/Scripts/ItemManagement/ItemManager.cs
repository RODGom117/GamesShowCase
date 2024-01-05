using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    // Singleton instance controlling the logic and attributes of the items. 
    // Any new items should be declared here before they become usable
    public static ItemManager Instance;
    
    // Array containing items of the common rarity (1)
    public Item[] CommonItemPool;
    // Array containing items of the rare rarity (2)
    public Item[] RareItemPool;
    public Item[] VeryRareItemPool;
    public Item[] LegendaryItemPool;
    public Item[] JaguarWarriorItemPool;


    // Editor references to Prefabs and Sprites for common items that have them
    [SerializeField] public GameObject[] CommonItemPrefabs;
    [SerializeField] public Sprite[] CommonItemSprites;




    void Awake(){
        // Checks if the instance exists. Destroys any duplicates
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        } else { 
            Instance = this; 
        } 

        // Setting the attributes for every common item. 
        // Each item must be set to its own prefab or sprite depending on the case. 
        // This array only contains items of the common rarity
        CommonItemPool = new Item[]
        {
            new ConsumableItem
            {
                id = 101,
                name = "Flor de Cempasuchil",
                rarity = 1,
                description = "Recupera 30 de salud",
                healthRestored = 30.0f,
                itemPrefab = CommonItemPrefabs[0],
                itemSprite = CommonItemSprites[0],
                
            },

            new ConsumableItem
            {
                id = 102,
                name = "Tamal de dulce",
                rarity = 1,
                description = "Recupera 80 de salud",
                healthRestored = 30.0f,
                itemPrefab = CommonItemPrefabs[1],
                itemSprite = CommonItemSprites[1],
                
            },

            new BuffConsumable {
            
                id = 301,
                name = "Lanza Eterea",
                rarity = 1,
                description = "Incrementa el daño base del usuario por 15%", 


                healthBuff = 1f,
                defenseBuff = 1f,

                fallSpeed = 1f,
                movSpeedBuff = 1f,
                jumpHeightBuff = 1f,

                dmgBuff = 1.15f,
                atkSpeedBuff = 1f,

                critChanceBuff = 1f,

                itemSprite = CommonItemSprites[2],
                itemPrefab = CommonItemPrefabs[2],
                
            },

            new BuffConsumable {
                id = 302,
                name = "Amuleto Eclipse",
                rarity = 1,
                description = "Incrementa la defensa base del usuario por 15%", 


                healthBuff = 1f,
                defenseBuff = 1.15f,

                fallSpeed = 1f,
                movSpeedBuff = 1f,
                jumpHeightBuff = 1f,

                dmgBuff = 1f,
                atkSpeedBuff = 1f,

                critChanceBuff = 1f,

                itemSprite = CommonItemSprites[3],
                itemPrefab = CommonItemPrefabs[3],
                
            }, 

            new BuffConsumable {
                id = 303,
                name = "Colgante de Tlapaltic",
                rarity = 2,
                description = "Incrementa la probabilidad de critco del usuario por 15%", 

                healthBuff = 1f,
                defenseBuff = 1f,

                fallSpeed = 1f,
                movSpeedBuff = 1f,
                jumpHeightBuff = 1f,

                dmgBuff = 1f,
                atkSpeedBuff = 1f,

                critChanceBuff = 1.15f,

                itemSprite = CommonItemSprites[4],
                itemPrefab = CommonItemPrefabs[4],
                
            },

            new BuffConsumable {
                id = 401,
                name = "Silbato de Jaguar",
                rarity = 2,
                description = "Incrementa tu velocidad de movimiento por 50%",  

                healthBuff = 1f,
                defenseBuff = 1f,

                fallSpeed = 1f,
                movSpeedBuff = 1.5f,
                jumpHeightBuff = 1f,

                dmgBuff = 1f,
                atkSpeedBuff = 1f,

                critChanceBuff = 1f,

                itemSprite = CommonItemSprites[5],
                itemPrefab = CommonItemPrefabs[5],
            },
            

        };


        

    }

    // Chooses a random common item for the chest logic. 
    // Returns an item type object
    public Item ChooseRandomCommonItem(){
        int randomIndex = Random.Range(0, CommonItemPool.Length);
        Item item = CommonItemPool[randomIndex];
        
        return item;
    }
}
