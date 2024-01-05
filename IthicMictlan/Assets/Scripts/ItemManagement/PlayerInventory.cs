using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerInventory : MonoBehaviour
{   
    
    // Reference to the scriptable object containing the player's stats
    public HeroStats heroStats;
    // Inventory of the player the script is attached to
    [SerializeField] public List<Item> Inventory = new List<Item>();
    // Script containing information on the player's health
    public HealthSystem playerHealth;
    // A variable to store the player's current position
    private Transform playerPosition;

    // Serialized variables referencing the different elements on the canvas
    // Reference to the InventoryUI canvas group, containing all the elements related to the passive buff card
    [SerializeField] private CanvasGroup inventoryUICanvas;
    // Reference to the scroll view containing passive item data
    [SerializeField] private ScrollRect inventoryCard;
    // Reference to item sprite, which shows what item was used to the player when the card is displayed
    [SerializeField] private Image ItemSprite;
    // Reference to TMP_Text that contains the item's name
    [SerializeField] private TMP_Text ItemNameText;
    // Reference to TMP_Text that contains the item's description
    [SerializeField] private TMP_Text ItemDescriptionText;
    // A separate text field meant to show alerts when the player uses an active item, and when the inventory is full
    [SerializeField] private TMP_Text ItemGetText;
    
    [SerializeField] private Transform InventoryTransform;
    [SerializeField] private GameObject ItemIcon;


    
    // Set the playerHealth component to the player's HealthSystemScript
    private void Start(){
        HideItemCard();
        HideItemGetText();
        playerHealth = GetComponent<HealthSystem>();
    }

    // Applies the effect of any buff the player picks up, or adds heal items to the inventory
    // Receives an Item type object. Returns nothing
    public void AddItem(Item item){
        // checks what type of item we got. if it's a buff, it's used instantly
        
        if (item is BuffConsumable buffItem) {
            
            buffItem.applyEffect(heroStats); 
            StartCoroutine(ItemGetCardFadeInOut(buffItem.name, buffItem.description, buffItem.itemSprite));
            return;
        }
        // if it's a heal item and the inventory is not full, store it for later use
        if (Inventory.Count < 3){
            Inventory.Add(item); // Add item to inventory array
            ListAllItems();
            StartCoroutine(ChangeItemMessageFadeInOut($"{item.name} se agregó a tu inventario"));
            return;
        
        } else { // else statement to activate the canvas that notifies the player if their inventory is full
            StartCoroutine(ChangeItemMessageFadeInOut("No puedes cargar mas cosas"));
            return;
        }
    }

    // Removes an item from the player's inventory
    // Receives an Item type object. Returns nothing
    public void RemoveItem(Item item) {
        Inventory.Remove(item);
        ListAllItems();
    }

    // Uses a heal item from the player's inventory
    // Receives an Item type object. Returns nothing
    public void UseItem(Item item) {
        // Checks if the item is a consumable. Other items might use this method too eventually, so their respective checks can go here to allow them to have different logic
        if (item is ConsumableItem healItem) {
            playerPosition = transform;
            healItem.applyEffect(playerHealth, playerPosition); 
            RemoveItem(item);
            StartCoroutine(ChangeItemMessageFadeInOut($"Utilizaste {item.name}"));
        }
    }
    

    // Update checks for the KeyCode F and uses the first item on the queue
    void Update(){
        if(Input.GetKeyDown(KeyCode.F) && Inventory.Count != 0){
            UseItem(Inventory[0]);
        }
    }

    // Methods to show and hide the item scroll view
    public void ShowItemCard() => inventoryCard.gameObject.SetActive(true);
    public void HideItemCard() => inventoryCard.gameObject.SetActive(false);

    // Methods to change the contents of the sprite, name and description on the item card
    public void SetSprite(Sprite sprite) => ItemSprite.sprite = sprite;
    public void SetItemName(string name) => ItemNameText.text = name;
    public void SetItemDescription(string desc) => ItemDescriptionText.text = desc;

    // Methods to show, hide, and modify the active item alert
    public void SetItemGetText(string text) => ItemGetText.text = text;
    public void ShowItemGetText() => ItemGetText.enabled = true;
    public void HideItemGetText() => ItemGetText.enabled = false;

    // Methods to show/hide the canvas InventoryUI canvas group
    public void ShowCanvasGroup() => inventoryUICanvas.alpha = 1;  
    public void HideCanvasGroup() => inventoryUICanvas.alpha = 0;  

    public IEnumerator ItemGetCardFadeInOut(string itemName, string itemDescription, Sprite itemSprite){
        ShowItemCard();
        SetSprite(itemSprite);
        SetItemName(itemName);
        SetItemDescription(itemDescription);
        
        yield return new WaitForSeconds(5f);
        HideItemCard();
        
    }


    public IEnumerator ChangeItemMessageFadeInOut(string message){
        SetItemGetText(message);
        ShowItemGetText();
        yield return new WaitForSeconds(5f);
        HideItemGetText();
        
    }

    public void ListAllItems(){
        foreach (Transform item in InventoryTransform){
            Destroy(item.gameObject);
        }
        foreach (Item item in Inventory){
            GameObject obj = Instantiate(ItemIcon, InventoryTransform); 
            Image itemImage = obj.GetComponent<Image>();
            if (item is ConsumableItem healItem) {
                itemImage.sprite = healItem.itemSprite;
            }   
            
        }
    }

    
}
