using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonWeapon : Weapon
{
    public GameObject projectile;
    /// <summary>
    /// Shoots a projectile from a specific point to a target point.
    /// Changes rotation of weapon.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public override void Shoot(Vector3 from, Vector3 to)
    {
        SoundController.Singleton.playSoundeffect(shootSound, audioSource);
        float alpha = 40f / 360f * 2 * Mathf.PI;
        float h = to.y - from.y;
        Vector3 directionXZ = new Vector3(to.x - from.x, 0, to.z - from.z);
        float r = directionXZ.magnitude;
        float v0 = Mathf.Sqrt(
            Physics.gravity.y * r * r / (2 * (h - r * Mathf.Tan(alpha)))
            ) / Mathf.Cos(alpha);

        GameObject launchedObject = Instantiate(projectile, from, Quaternion.identity);
        launchedObject.GetComponent<Projectiles>().damage = this.damage;
        transform.rotation = Quaternion.LookRotation(directionXZ);
        transform.Rotate(Vector3.left * 40f, Space.Self);
        launchedObject.GetComponent<Rigidbody>().velocity = transform.forward * v0;
    }

    /// <summary>
    /// Shoots from the transform firePoint.
    /// </summary>
    public override void Shoot(Transform firePoint, Transform referencePoint)
    {
        RaycastHit hit;
        SoundController.Singleton.playSoundeffect(shootSound, audioSource);
        if(Physics.Raycast(referencePoint.position, referencePoint.forward, out hit, range, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {

        }
        else
        {
            hit.point = firePoint.position + referencePoint.forward * range;
        }
        firePoint.rotation = Quaternion.LookRotation(hit.point - firePoint.position);
        GameObject launchedObject = Instantiate(projectile, firePoint.position + firePoint.forward, firePoint.rotation);
        launchedObject.GetComponent<Projectiles>().damage = this.damage;
        launchedObject.GetComponent<Rigidbody>().velocity = firePoint.forward * range * 0.5f;
    }

    public override void SelectSound()
    {
        if(projectile.name == "GraviProjectile")
        {
            Debug.Log("*****");
        }
        else
        {
            Debug.Log("_____");
        }
        Debug.Log("Projectile: " + projectile.name);
        switch (projectile.name)
        {
            case "Projectile":
                shootSound = SoundController.Singleton.weaponCanon;
                break;
            case "ArrowProjectile":
                shootSound = SoundController.Singleton.weaponArrow;
                break;
            case "GraviProjectile":
                shootSound = SoundController.Singleton.weaponGravi;
                break;
            default:
                shootSound = SoundController.Singleton.weaponCanon;
                break;
        }
    }
}
