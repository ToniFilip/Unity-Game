using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Destroys any gameobject that enters the trigger.
/// Used for removing gameobjects that fall through the map (performance).
/// </summary>
public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Damage(enemy.health, "");
        } else
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TeleportTo(GameManager.Singleton.mapGenerator.playerBase.transform.position + Vector3.right * 3f);
            } else
            {
                Destroy(other.gameObject);
            }
        }
    }
}
