using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class XoloController : MonoBehaviour
{
    public static XoloController instance;
    public GameObject mainBody;
    private NavMeshAgent agent;
    private Animator animControl;
    private SphereCollider catchCollider;
    private float speed;
    private float catchRange;
    private bool isRunning;
    private bool wasCatched;
    //private bool canBeGrabbed;

    [SerializeField] private PhotonView photonView;

    private void Awake() {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start() {
        agent = mainBody.GetComponent<NavMeshAgent>();
        animControl = mainBody.GetComponent<Animator>();
        catchCollider = GetComponent<SphereCollider>();
        speed = agent.speed;
        catchRange = catchCollider.radius;
        isRunning = false;
        wasCatched = false;
        //canBeGrabbed = false;
    }

    void Update() {
        if(photonView.IsMine)
        {
            speed = agent.speed;
            // Animation handling 
            if(speed == 10)
            {
                isRunning = true;
            }
            else if(speed == 3.5f)
            {
                isRunning = false;
            }

            // Setting parameters of the animator
            animControl.SetBool("isRunning", isRunning);
            animControl.SetFloat("Speed", speed);
        }

        // Catch logic
        /*if(canBeGrabbed)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                StopRun();
            }
        }*/
        
    }

    public void StopRun()
    {
        if(photonView.IsMine)
        {
            wasCatched = true;
            isRunning = false;
            //canBeGrabbed = false;
            animControl.SetBool("isRunning", isRunning);
            animControl.SetFloat("Speed", 0);
            animControl.SetBool("hasStopped", wasCatched);
            GameManager.instance.GetXoloStatus(wasCatched);
            StartCoroutine(DispawnXolo());
        }        
    }

    public bool CatchCheck()
    {
        return wasCatched;
    }

    public void CatchXolo()
    {
        if(photonView.IsMine)
        {
            StopRun();
        }
        
    }

    /*private void OnTriggerEnter(Collider other) {
        if(other.tag == "Hero")
        {
            canBeGrabbed = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.tag == "Hero")
        {
            canBeGrabbed = false;
        }
    }*/

    private IEnumerator DispawnXolo()
    {
        yield return new WaitForSeconds(6f);
        PhotonNetwork.Destroy(mainBody);
    }

}
