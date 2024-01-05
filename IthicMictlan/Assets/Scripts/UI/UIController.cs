using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    private void Awake() {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    
    }
}
