using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.SideChannels;
using UnityEngine;

public class AirBulletVFX : MonoBehaviour
{
    [SerializeField] private GameObject airBulletVFX;
    AirBulletDamage airBulletDamage;
    public float projectileSpeed = 25f;

    public void LaunchAirBullet(Vector3 position, Vector3 scale, float scScale, float damage)
    {
        GameObject airBullet = Instantiate(airBulletVFX, position, transform.rotation);
        
        airBullet.transform.localScale = scale;
        Rigidbody rb = airBullet.GetComponent<Rigidbody>();
        SphereCollider sc = airBullet.GetComponent<SphereCollider>();
        sc.radius = scScale;
        rb.velocity = transform.forward * projectileSpeed;

        airBulletDamage = airBullet.GetComponent<AirBulletDamage>();
        airBulletDamage.SetDamage(damage);

        Destroy(airBullet, 5f);
    }

}
