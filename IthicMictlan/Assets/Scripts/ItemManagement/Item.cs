using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Item 
{
    // internal id of the item 
    public int id;

    // name of the item which the player will see 
    public string name;

    // 1 - common
    // 2 - rare
    // 3 - very rare
    // 4 - legendary
    public int rarity;

    // short description of the item and its effect
    public string description;

}
