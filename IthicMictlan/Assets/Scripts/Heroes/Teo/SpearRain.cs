using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpearRain : MonoBehaviour
{
    [SerializeField] private GameObject ultimateSpear;
    public ParticleSystem rainSpear;


    public void LaunchUltimateTeo(Vector3 position)
    {
        string spearPath = "Objects/" + ultimateSpear.name;
        GameObject ultSpear = PhotonNetwork.Instantiate(spearPath, position + new Vector3(0, 5, 0), transform.rotation);
        if (ultSpear == null)
        {
            ultimateSpear = PhotonNetwork.Instantiate("Assets/Resources/Objects/TeoUltimateSpear.prefab", position, transform.rotation);

        }
        Rigidbody rb = ultSpear.GetComponent<Rigidbody>();

        Destroy(ultSpear, 5f);
    }
}