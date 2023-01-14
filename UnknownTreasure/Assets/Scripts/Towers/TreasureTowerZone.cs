using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureTowerZone : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == MyLayer.enemyLayer)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            GameManager.Singleton.DamageTreasureTower(enemy.towerDamage);
            GameManager.Singleton.DecrementKillCounter();
            enemy.Damage(enemy.health, this.GetType().ToString());
            SoundController.Singleton.playSoundeffect(SoundController.Singleton.damageTower, audioSource);
        }
    }
}
