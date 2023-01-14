using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryTowerZone : MonoBehaviour
{
    [SerializeField] private List<Transform> enemies;

    public GameObject visualization;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == MyLayer.enemyLayer && !enemies.Contains(other.transform))
        {
            enemies.Insert(0, other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == MyLayer.enemyLayer)
        {
            enemies.Remove(other.transform);
        }
    }

    public Transform GetFirstEnemy()
    {
        for (int k = enemies.Count - 1; k >= 0; k--)
        {
            if (enemies[k] == null)
            {
                enemies.RemoveAt(k);
            } else
            {
                return enemies[k];
            }
        }
        return null;
    }

    public int GetEnemyCount()
    {
        return enemies.Count;
    }
}
