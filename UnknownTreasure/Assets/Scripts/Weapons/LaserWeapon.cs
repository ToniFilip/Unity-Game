using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : Weapon
{
    private LineRenderer laserLineRenderer;
    public GameObject fx_laserHitPrefab;
    private float laserDuration = 0.2f;

    public Transform targetTrack;

    private void Start()
    {
        laserLineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Shoots from the transform firePoint.
    /// </summary>
    public override void Shoot(Transform firePoint, Transform referencePoint)
    {

        SoundController.Singleton.playSoundeffect(shootSound, audioSource, 0.3f);
        RaycastHit hit;

        if (Physics.Raycast(referencePoint.position, referencePoint.forward, out hit, range, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.GetComponent<Enemy>() != null)
            {
                hit.collider.GetComponent<Enemy>().Damage(damage, this.GetType().ToString());
            }

            Visualize(firePoint, hit.point, true);

            Instantiate(fx_laserHitPrefab, hit.point, Quaternion.identity, null);
        }
        else
        {
            Visualize(firePoint, referencePoint.position + referencePoint.forward * range, false);
        }
    }

    /// <summary>
    /// Shoots with direction defined by from- and to-position.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public override void Shoot(Vector3 from, Vector3 to)
    {
        SoundController.Singleton.playSoundeffect(shootSound, audioSource, 0.3f);
        RaycastHit hit;
        if (Physics.Raycast(from, (to - from).normalized, out hit, Mathf.Infinity, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.GetComponent<Enemy>() != null)
            {
                hit.collider.GetComponent<Enemy>().Damage(damage, this.GetType().ToString());
            }

            Visualize(transform, to, true);
        }
    }

    private void Visualize(Transform from, Vector3 to, bool hit)
    {
        if (laserLineRenderer != null)
        {
            targetTrack.position = to;
            StopAllCoroutines();
            StartCoroutine(ShootLaserDelay(laserDuration, from));
        }

        if (hit) Instantiate(fx_laserHitPrefab, to, Quaternion.identity, null);
    }


    IEnumerator ShootLaserDelay(float time, Transform from)
    {
        SetBeam(from.position, targetTrack.position);

        yield return new WaitForEndOfFrame();
        time -= Time.deltaTime;

        if (time < 0)
        {
            laserLineRenderer.enabled = false;
        }
        else
        {
            StartCoroutine(ShootLaserDelay(time, from));
        }
    }

    private void SetBeam(Vector3 from, Vector3 to)
    {
        if (!laserLineRenderer.enabled) laserLineRenderer.enabled = true;
        laserLineRenderer.SetPosition(0, from);
        laserLineRenderer.SetPosition(1, to);
    }


    public override void SelectSound()
    {
        shootSound = SoundController.Singleton.weaponLaser;
    }
}
