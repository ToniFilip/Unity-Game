using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraviProjectile : Projectiles
{
    public float attractionForce = 200f;
    void OnCollisionEnter(Collision collision)
    {
        if (effect != null)
        {
            Instantiate(effect, transform.position, transform.rotation);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, MyLayer.enemyLayerMask);
        foreach (Collider nearbyObject in colliders)
        {
            Enemy enemy = nearbyObject.GetComponent<Enemy>();
            enemy.Damage(damage, this.GetType().ToString());
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null && enemy.isAffectedByForces)
            {
                enemy.DeactivateAgent(5f);
                Vector3 forceDirection = transform.position - rb.transform.position;
                rb.AddForce(attractionForce * forceDirection);
            }
        }

        // Sound effect
        SoundController.Singleton.createSoundObject(SoundController.Singleton.projectileGravi, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
