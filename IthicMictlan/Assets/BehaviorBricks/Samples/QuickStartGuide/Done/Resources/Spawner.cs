using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
/// <summary>
/// Component that is responsible for spawning a GameObjects in a certain position. 
/// When instancing, it will move randomly in the area until it detects the player and begins to follow him.
/// </summary>
public class Spawner : MonoBehaviour {

    ///<value>The gameobject that will be spawned</value>
    public GameObject prefab;
    ///<value>the position that the gameobject will be spawned</value>
    //public Vector3 position;

    [SerializeField] private string xoloPath;
    public List<Transform> xoloPositions = new List<Transform>();

    //public Text text;

    /// <summary>
    /// method that instantiates the gameobjet in a certain position and adds the BehaviorExcutor component to follow the player
    /// </summary>
	void Start() {
        /*foreach (Transform xoloSpawn in xoloPositions)
        {
            GameObject instance = Instantiate(prefab, xoloSpawn.position,Quaternion.identity) as GameObject;
            //BehaviorExecutor behaviorExecutor = instance.GetComponent<BehaviorExecutor>();


            //Codigo comentado para comprobaciones de editor y runtime

            //if (BBUnity.Managers.BBManager.Instance.IsEditor)
            //    text.text = "EDITOR";
            //else
            //    text.text = "RUNTIME";

            /*if (behaviorExecutor != null)
            {
                behaviorExecutor.SetBehaviorParam("wanderArea", wanderArea);
                behaviorExecutor.SetBehaviorParam("player", player);
            }
        }*/
	}

    public void SetSpawns()
    {
        xoloPath = "NPCS/" + prefab.name;

        foreach (Transform xoloSpawn in xoloPositions)
        {
            GameObject instance = PhotonNetwork.Instantiate(xoloPath,xoloSpawn.position,Quaternion.identity);
        }
    }
}
