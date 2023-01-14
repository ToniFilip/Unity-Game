using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : Projectiles
{
    bool inCollision = false;
    private Rigidbody body;
    private void Update()
    {
        if (inCollision)
        {
            return;
        }
        transform.rotation = Quaternion.LookRotation(-(body.velocity));
    }

    private void Start()
    {
       body = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if(inCollision) {
            return;
        }

        if (effect != null)
        {
            Instantiate(effect, transform.position, transform.rotation);
        }

        inCollision = true;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, MyLayer.enemyLayerMask);
        foreach (Collider nearbyObject in colliders)
        {
            Enemy enemy = nearbyObject.GetComponent<Enemy>();
            enemy.Damage(damage, this.GetType().ToString());
        }
        Destroy(gameObject,1f);
    }
}
