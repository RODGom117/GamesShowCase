using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using random = UnityEngine.Random;

public class SpiritBond : MonoBehaviour
{
    // Get references to all 4 players
    [SerializeField] private GameObject Player1;
    [SerializeField] private GameObject Player2;
    [SerializeField] private GameObject Player3;
    [SerializeField] private GameObject Player4;
    
    // Initialize an array that will contain all the players' positions
    public Transform[] playerPositions;
    // Array that will contain the player's positions in the correct order when they shift places
    private Transform[] PlayerPositionsDelta;

    // Total distance between players, calculated as the sum of the distance between every player. Damage Multiplier scales directly with this.
    [SerializeField] private float TotalDistance;
    // Damage Multiplier. This is the variable that we are trying to calculate
    [SerializeField] private double bonusMultiplier;

    // Public variable that references the calculated value. Intended for use on other scripts
    public float BonusMultiplier => (float)bonusMultiplier;

    // Rate at which the bonus multiplier decreases inversely proportional to distance. The lower it is, the slower the damage will decrease whenever distance increases
    private float decreaseRate = 1.2f;
    // Controls the distance that has to exist between players before damage starts to decrease. The higher it is, the further apart players must be for falloff to occur
    private float distanceFalloff = 180.0f;
    // References to the children of each Player.
    private GameObject[] spiritBondChains;
    // Boolean variable that is used to know if the SpiritBond is active
    private bool bondActive;
    

    // Initializes player positions as an array of 4 elements, each containing the Transform of its respective player; 
    // TotalDistance as 0.0f; Obtain references to each player's chain; Start the particle systems
    void Start(){
        playerPositions = new Transform[4]{
            Player1.transform, 
            Player2.transform, 
            Player3.transform, 
            Player4.transform
        };

        spiritBondChains = new GameObject[4]{
            Player1.transform.GetChild(0).gameObject,
            Player2.transform.GetChild(0).gameObject,
            Player3.transform.GetChild(0).gameObject,
            Player4.transform.GetChild(0).gameObject,
        };

        TotalDistance = 0.0f;

        PlayerPositionsDelta = playerPositions;

        bondActive = true;

        StartCoroutine(ActivateParticleSystems(PlayerPositionsDelta[0], PlayerPositionsDelta[1], spiritBondChains[0]));
        StartCoroutine(ActivateParticleSystems(PlayerPositionsDelta[1], PlayerPositionsDelta[2], spiritBondChains[1]));
        StartCoroutine(ActivateParticleSystems(PlayerPositionsDelta[2], PlayerPositionsDelta[3], spiritBondChains[2]));
        StartCoroutine(ActivateParticleSystems(PlayerPositionsDelta[3], PlayerPositionsDelta[0], spiritBondChains[3]));
        
    }

    

    void Update()
    {
        // Reset the value of total distance to 0
        // It will increase exponentially if we don't
        TotalDistance = 0.0f;


        // Calculates the total distance between adjacent players contained in the playerPositions Array.
        // This method ensures that the resulting distance will be consistent regardless of how players are arranged. 
        // Determines the distance between each adjacent pair of players, then adds the result to TotalDistance
        for(int i = 0; i < playerPositions.Length; i++) {
            int nextIndex = (i + 1) % playerPositions.Length;
            float distance = Vector3.Distance(playerPositions[i].position, playerPositions[nextIndex].position);

            TotalDistance += distance;

        }

        // A mathematical formula that models the exponential decrease of bonusMultiplier inversely proportional to TotalDistance as:
        // f(t) = -dR^(t-dF/2)+4
        // Players may stray away from each other up to an effective range of about 65 meters before damage starts to decrease.
        // The further away players are from each other, the sharper the decline in damage. This decline is hardcapped to 0.5f to avoid leaving players with negative damage
        // Due to how the formula is written, f(t) cannot be higher than 4.
        bonusMultiplier = -Math.Pow(decreaseRate, (TotalDistance - distanceFalloff / 2)) + 4.0f;
        bonusMultiplier = bonusMultiplier < 0.5 ? 0.5 : bonusMultiplier;

        
        // Calculates the correct player order clockwise
        CalculatePlayerOrder(); 
        
        // Places the spirit bond chains between the positions of each pair of players
        MoveCubeToEdgeCenter(PlayerPositionsDelta[0], PlayerPositionsDelta[1], spiritBondChains[0]);
        MoveCubeToEdgeCenter(PlayerPositionsDelta[1], PlayerPositionsDelta[2], spiritBondChains[1]);
        MoveCubeToEdgeCenter(PlayerPositionsDelta[2], PlayerPositionsDelta[3], spiritBondChains[2]);
        MoveCubeToEdgeCenter(PlayerPositionsDelta[3], PlayerPositionsDelta[0], spiritBondChains[3]);

        // If the bonus reaches the lower limit, deactivate the chains. 
        if (bonusMultiplier == 0.5){
            foreach(GameObject chain in spiritBondChains){
                chain.SetActive(false);
                bondActive = false;
            }
        } else {
            foreach(GameObject chain in spiritBondChains){
                chain.SetActive(true);
                bondActive = true;
            }
        }


    }

    // Places the player's spirit bond chain in the middle point between its parent and the next player on the list
    // Receives two transforms and one GameObject, Returns nothing
    void MoveCubeToEdgeCenter(Transform pos1, Transform pos2, GameObject cube){
        Vector3 edgeCenter = Vector3.Lerp(pos1.position, pos2.position, 0.5f);
        cube.transform.position = edgeCenter;
        cube.transform.LookAt(pos2);
        cube.transform.localScale = new Vector3(0.05f, 0.05f, Vector3.Distance(pos1.position, pos2.position));
    }


    // Finds a random position Vector between two other vectors
    // Receives two Vector3, returns one Vector3
    Vector3 FindRandomPositionInLine(Vector3 pos1, Vector3 pos2){
        return new Vector3(random.Range(pos1.x, pos2.x), random.Range(pos1.y, pos2.y), random.Range(pos1.z, pos2.z));

    }

    // Coroutine that controls the particle systems while they are active.
    // Receives Two transforms, and one GameObject
    IEnumerator ActivateParticleSystems(Transform pos1, Transform pos2, GameObject cube){
        // While the bond is active
        while(bondActive){
            // Wait for the previous system to finish emitting
            yield return new WaitForSeconds(1.5f);
            // For each of the cube's children particle systems
            foreach(Transform child in cube.transform){
                // Find a new RandomPosition
                Vector3 newRandPos = FindRandomPositionInLine(pos1.position, pos2.position);
                // Assign that position to its respective child
                child.gameObject.transform.position = newRandPos;
                // Play the particle system again
                child.gameObject.GetComponent<ParticleSystem>().Play();
            }

        }

    }

    // Bubble Sort Alogrithm adapted to arrange the players in the correct order based from lowest to highest depending on their angle relative to front.
    // Receives an array of transforms, returns nothing
    void BubbleSort(Transform[] playerPos, Vector3 front, Vector3 center){
        Transform temp;
        int i, j;
        bool swapped;
        for (i = 0; i < playerPos.Length - 1; i++){
            swapped = false;
            for (j = 0; j < playerPos.Length - i - 1; j++){
                if (GetAngle(center, front, playerPos[j].position) > GetAngle(center, front, playerPos[j + 1].position)){
                    temp = playerPos[j];
                    playerPos[j] = playerPos[j + 1];
                    playerPos[j + 1] = temp;
                    swapped = true;
                }
            }
            if (swapped == false)
                break;
        }
    }

    // Calculates the Angle formed by three points. The center of a polygon, the point directly in front of it, and a third, variable point
    // Receives Three Vector3, Returns a floating point number
    float GetAngle(Vector3 center, Vector3 front, Vector3 angle){
        // Obtain the direction from the center of the polygon, to the player position we want to compare to front
        Vector3 directionToAngle = angle - center;
        // Obtain the direction from the center of the polygon, to the front vector
        Vector3 directionToFront = front - center;

        // Use the Angle method to calculate the distance between both resulting vectors
        float calculatedAngle = Vector3.Angle(directionToFront, directionToAngle);

        // If the result is negative, convert the angle to its positive equivalent and return it. If not, return it as is
        return (Vector3.Cross(directionToFront, directionToAngle).y < 0) ? 360 - calculatedAngle : calculatedAngle;
    }


    // Determines the correct player order based on their positioning only on the X and Z axes
    void CalculatePlayerOrder(){
        // find the vertex of the angle. this is used in every calculation
        Vector3 centroid = FindCentroid();
        // find the point directly in front of the centroid. this is the control vector to make all comparisons from
        Vector3 front = centroid + new Vector3(0, 0, 20);
        Debug.DrawRay(centroid, front, new Color(255, 0, 0, 255));
            
        // Sort the players from the one closest to the front of the centroid, to the one furthest away
        BubbleSort(PlayerPositionsDelta, front, centroid);
        

    }

    // Finds the center point between four other points by obtaining the average value between them
    Vector3 FindCentroid(){
        // Initialize the coordinates for X and Z as 0
        float centroidX = 0, centroidZ = 0;
        // Add up the x and z coordinates for each of the players in the array
        foreach (Transform vertex in playerPositions){
            centroidX += vertex.position.x;
            centroidZ += vertex.position.z;
        }
        // Divide both coordinates by the amount of players present in the array 
        centroidX /= playerPositions.Length;
        centroidZ /= playerPositions.Length;

        // Return the newly calculated centroid
        return new Vector3(centroidX, 0, centroidZ);
    }

}
