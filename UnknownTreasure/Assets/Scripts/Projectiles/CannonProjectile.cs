using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonProjectile : Projectiles
{
    public float force = 500f;

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
            //Debug.Log(this.GetType());
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null && enemy.isAffectedByForces)
            {
                enemy.DeactivateAgent(3f);
                rb.AddExplosionForce(this.force, transform.position, radius);
            }
        }

        // Sound effect
        SoundController.Singleton.createSoundObject(SoundController.Singleton.projectileCanon, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}