using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    // Serialized variables referencing the different elements on the canvas
    // Reference to the InventoryUI canvas group, containing all the elements related to the passive buff card
    [SerializeField] private CanvasGroup inventoryUICanvas;
    // Reference to item sprite, which shows what item was used to the player when the card is displayed
    [SerializeField] private Image ItemSprite;
    // Reference to TMP_Text that contains the item's name
    [SerializeField] private TMP_Text ItemNameText;
    // Reference to TMP_Text that contains the item's description
    [SerializeField] private TMP_Text ItemDescriptionText;


    // A separate text field meant to show alerts when the player uses an active item, and when the inventory is full
    [SerializeField] private TMP_Text ItemGetText;

    // Methods to change the contents of the sprite, name and description on the item card
    public void SetSprite(Sprite sprite) => ItemSprite.sprite = sprite;
    public void SetItemName(string name) => ItemNameText.text = name;
    public void SetItemDescription(string desc) => ItemDescriptionText.text = desc;

    // Methods to show, hide, and modify the active item alert
    public void SetItemGetText(string text) => ItemGetText.text = $"The Item {text} was added to your inventory";
    public void SetInventoryFullText(string text) => ItemGetText.text = $"Your Inventory is full";
    public void ShowItemGetText() => ItemGetText.enabled = true;
    public void HideItemGetText() => ItemGetText.enabled = false;

    // Methods to show/hide the canvas InventoryUI canvas group
    public void ShowCanvasGroup() => inventoryUICanvas.alpha = 1;  
    public void HideCanvasGroup() => inventoryUICanvas.alpha = 0;  
    


}
