using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Animations;

public class XoloitzcuintleController : MonoBehaviour
{
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float runAwaySpeed;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float wanderChangeInterval;

    private List<Transform> players = new List<Transform>();
    private bool isPlayerDetected;
    private float wanderTimer;
    private Vector3 wanderTarget;
    [SerializeField] private Animator animationController;

    private bool wasCatched;

    // PHOTON
    [SerializeField] private PhotonView photonView;


    private void OnEnable()
    {
        if(photonView.IsMine)
        {
            isPlayerDetected = false;
            wanderTimer = 0f;
            wasCatched = false;

            GameObject[] temp = GameObject.FindGameObjectsWithTag("Hero");

            foreach (GameObject item in temp)
            {
                players.Add(item.transform);
            }

            SetNewWanderTarget();
        }
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            if (wasCatched == false)
            {
                isPlayerDetected = false;

                foreach (Transform player in players)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                    if (distanceToPlayer < detectionRadius)
                    {
                        // Player is within detection radius of at least one player, switch to run away state
                        isPlayerDetected = true;
                        RunAway();
                        animationController.SetBool("isRunning", isPlayerDetected);
                        animationController.SetFloat("Speed", runAwaySpeed);
                        break; // Exit the loop as soon as one player is detected
                    }
                }

                if (!isPlayerDetected)
                {
                    // No player is within detection radius, switch to wander state
                    Wander();
                    animationController.SetBool("isRunning", false);
                    animationController.SetFloat("Speed", wanderSpeed);
                }
            }
            else
            {
                runAwaySpeed = 0;
                wanderSpeed = 0;
            }   
        }
    }

    private void Wander()
    {
        // Update the timer
        wanderTimer += Time.deltaTime;

        // Change wander direction every 5 seconds
        if (wanderTimer >= wanderChangeInterval)
        {
            SetNewWanderTarget();
            wanderTimer = 0f;
        }

        // Move towards the wander target
        transform.Translate(Vector3.forward * wanderSpeed * Time.deltaTime);
    }

    private void SetNewWanderTarget()
    {
        // Set a new random wander target
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        wanderTarget = transform.position + new Vector3(randomDirection.x, 0, randomDirection.y) * 5f;
        transform.LookAt(wanderTarget);
    }

    private void RunAway()
    {
        // Implement your running away behavior here
        Vector3 runAwayDirection = Vector3.zero;

        foreach (Transform player in players)
        {
            runAwayDirection += transform.position - player.position;
        }

        runAwayDirection /= players.Count;

        runAwayDirection.y = 0f;
        transform.LookAt(transform.position + runAwayDirection);
        transform.Translate(Vector3.forward * runAwaySpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection radius in the scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void SetWasCatched()
    {
        photonView.RPC("Catch", RpcTarget.All);
    }
    
    [PunRPC]
    private void Catch()
    {
        if(photonView.IsMine)
        {
            wasCatched = true;
            animationController.SetBool("isRunning", false);
            animationController.SetFloat("Speed", 0);
            animationController.SetBool("hasStopped", true);
            StartCoroutine(DispawnXolo());
        }
    }

    private IEnumerator DispawnXolo()
    {
        yield return new WaitForSeconds(6f);
        PhotonNetwork.Destroy(gameObject);
    }
}
