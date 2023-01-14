using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryTower : Tower
{
    public MilitaryTowerZone zone;
    public bool playerInside = false;
    public Weapon weapon;

    public float rangeBuff = 50f;
    public int damageBuff = 10;
    public float fireRateBuff = 1f;

    private void Start()
    {
        if (weapon.preAim)
        {
            StartCoroutine(ShootProjectiles(weapon.fireRate));
        } else
        {
            StartCoroutine(ShootLaser(weapon.fireRate));
        }
    }

    protected override void SubUpdate()
    {
        if (!isActive)
        {
            // Set range through zone
            float rangeBuffByHeight = 3 * transform.position.y;
            zone.transform.localScale = new Vector3(weapon.range, weapon.range, weapon.range) + rangeBuffByHeight * Vector3.one;
        }
    }

    protected override void SubBuild()
    {
        weapon.baseRange += 3 * transform.position.y;
    }

    /// <summary>
    /// Shoots projectiles and aims a bit in front of an enemy.
    /// Is not 100% accurate.
    /// </summary>
    /// <param name="fireRate"></param>
    /// <returns></returns>
    private IEnumerator ShootProjectiles(float fireRate)
    {
        if (isActive && zone.GetEnemyCount() > 0 && playerInside == false)
        {
            Transform enemyTransform = zone.GetFirstEnemy();
            if (enemyTransform != null)
            {
                Enemy enemy = enemyTransform.GetComponent<Enemy>();

                // Approximate position
                float distance = enemy.agent.speed * (weapon.transform.position - enemy.transform.position).magnitude / 8.5f;
                Vector3 nextCorner = enemy.transform.position;

                for (int k = 0; k < enemy.agent.path.corners.Length; k++)
                {
                    float distanceNextCornerSqrd = (enemy.agent.path.corners[0] - enemy.agent.path.corners[k]).sqrMagnitude;
                    if (distanceNextCornerSqrd >= distance * distance)
                    {
                        nextCorner = enemy.agent.path.corners[k];
                        break;
                    }
                }

                Vector3 target = (nextCorner - enemy.transform.position).normalized * distance + enemy.transform.position;

                Debug.DrawRay(weapon.transform.position, enemy.transform.position - weapon.transform.position, Color.red, 2f);
                Debug.DrawRay(weapon.transform.position, nextCorner - weapon.transform.position, Color.black, 2f);
                Debug.DrawRay(weapon.transform.position, target - weapon.transform.position, Color.blue, 2f);
                weapon.Shoot(weapon.transform.position, target);
            }
        }

        yield return new WaitForSeconds(1 / fireRate);
        StartCoroutine(ShootProjectiles(weapon.fireRate));
    }

    /// <summary>
    /// Shoots rays directly at enemies.
    /// </summary>
    /// <param name="fireRate"></param>
    /// <returns></returns>
    private IEnumerator ShootLaser(float fireRate)
    { 
        if (isActive && zone.GetEnemyCount() > 0 && playerInside == false)
        {
            Transform enemy = zone.GetFirstEnemy();
            if (enemy != null)
            {
                // Shoot a directly at where the enemy currently is
                Debug.DrawRay(weapon.transform.position, enemy.transform.position - weapon.transform.position, Color.red, .25f);
                weapon.Shoot(weapon.transform.position, enemy.transform.position);
            }
        }
        yield return new WaitForSeconds(1 / fireRate);
        StartCoroutine(ShootLaser(weapon.fireRate));
    }

    public override void Interact(Player player)
    {
        if (GameManager.Singleton.isInTower == true)
        {
            player.TeleportTo(entrance.position);
            GameManager.Singleton.isInTower = false;
            playerInside = false;
            BuffWeapon(false);
            GameManager.Singleton.player.weapon = GameManager.Singleton.player.GetComponent<Weapon>();
        }
        else
        {
            player.TeleportTo(topOfTower.position);
            GameManager.Singleton.isInTower = true;
            playerInside = true;
            BuffWeapon(true);
            GameManager.Singleton.player.weapon = weapon;
        }
    }

    public void BuffWeapon(bool status)
    {
        if (status)
        {
            weapon.range = weapon.baseRange + rangeBuff;
            weapon.damage = weapon.baseDamage + damageBuff;
            weapon.fireRate = weapon.baseFireRate + fireRateBuff;
        }
        else
        {
            weapon.range = weapon.baseRange;
            weapon.damage = weapon.baseDamage;
            weapon.fireRate = weapon.baseFireRate;
        }
    }

    public override void Build()
    {
        base.Build();
        zone.visualization.SetActive(false);
    }
}