using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using Photon.Pun;

public class YouPlayerControllerScript : MonoBehaviourPun
{
    private bool isMasterPlayer = false;
    private Transform masterPlayerTransform;

    void Start()
    {
        if (!photonView.IsMine)
        {
            SetMasterPlayerTransform();
        }
    }

    void SetMasterPlayerTransform()
    {
        // Encuentra al jugador maestro por su tag (ajusta según tu estructura)
        GameObject masterPlayer = GameObject.FindGameObjectWithTag("Player");

        if (masterPlayer != null)
        {
            masterPlayerTransform = masterPlayer.transform;
        }
    }

    public void SetMasterPlayer()
    {
        isMasterPlayer = true;
    }

    void Update()
    {
        if (!isMasterPlayer && masterPlayerTransform != null)
        {
            // Si este jugador no es el maestro, sigue al jugador maestro
            transform.position = Vector3.MoveTowards(transform.position, masterPlayerTransform.position, Time.deltaTime * 5f);
        }
    }
}
