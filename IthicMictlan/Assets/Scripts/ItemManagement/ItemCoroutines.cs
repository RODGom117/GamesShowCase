using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCoroutines : MonoBehaviour
{
    // Singleton instance meant to make item prefabs disappear after a short bit. 
    // Logic should be built upon for other item types, or integrated into another script if possible
    public static ItemCoroutines Instance;

    public void Awake(){
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        } else { 
            Instance = this; 
        } 
    }

    // Method calling the coroutine with the corresponding item
    public void CallDestroyHover(GameObject spawnedItem){
        StartCoroutine(DestroyHoverItemPrefab(spawnedItem));
    }

    // Wait for 2.5 seconds, then destroy the provided prefab
    public IEnumerator DestroyHoverItemPrefab(GameObject spawnedItem){
        
        yield return new WaitForSeconds(2.5f);
        GameObject.Destroy(spawnedItem);
    }
}
