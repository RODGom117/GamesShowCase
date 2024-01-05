using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool allXolosCactched;
    private bool xoloStatus;
    [SerializeField] int xolosInScene;
    [SerializeField] private Spawner xoloSpawner;

    [SerializeField] private Spawner chanequeSpawner;

    void Awake() {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() {
        allXolosCactched = false;
        xolosInScene = xoloSpawner.xoloPositions.Count;
        
    }

    void Update() {

        if(xoloStatus == true)
        {
            xolosInScene--;
            xoloStatus = false;
        }

        if(xolosInScene == 0)
        {
            Debug.Log("Congrats, you have catched all the Xolos");
            allXolosCactched = true;
            Debug.Log(allXolosCactched);
        }

    }

    public void GetXoloStatus(bool status)
    {
        xoloStatus = status;
    }

}
