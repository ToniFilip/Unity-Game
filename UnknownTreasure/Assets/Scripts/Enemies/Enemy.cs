using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public float speed;
    private static float acceleration = 24;
    private static float angularSpeed = 240;

    public int health = 100;
    private int maxHealth;

    public bool isAffectedByForces = true;
    public float multiplierAoeDamageTaken = 1f;
    public float multiplierLaserDamageTaken = 1f;
    
    public int towerDamage = 1;

    public bool spawnsChildEnemies = false;
    public int childEnemyCount;
    public Enemy childEnemyPrefab;

    public GameObject magicPrefab;
    public float magicDropProbability = 0.1f;
    public int magicDropAmount = 1;

    public GameObject fx_enemyDeathPrefab;

    [HideInInspector]
    public NavMeshAgent agent;
    private bool destroyed = false;

    private Material material;

    [SerializeField] public AudioSource audioSource;

    [Range(0f, 1f)]
    public float difficulty = 0.5f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        maxSpeedSqrd = speed * speed;

        agent.acceleration = acceleration;
        agent.angularSpeed = angularSpeed;

        material = GetComponent<MeshRenderer>().material;

        maxHealth = health;
    }

    private float timeStuck = 0;
    private static float maxTimeStuck = 10f;
    private float maxSpeedSqrd = 1;
    private void FixedUpdate()
    {
        // Check if the agent is stuck
        // If agent has path
        if (!agent.pathPending)
        {
            if (agent.pathStatus != NavMeshPathStatus.PathComplete || agent.velocity.sqrMagnitude < .25f * maxSpeedSqrd)
            {
                timeStuck += Time.fixedDeltaTime;
            } else if (timeStuck > 0)
            {
                timeStuck -= Time.fixedDeltaTime;
            }

            // Check if agent was longer stuck than maxTimeStuck
            if (timeStuck > maxTimeStuck)
            {
                Debug.LogWarning("Agent was stuck and got destroyed: " + gameObject.name);
                Damage(health, "");
            }
        }
    }

    /// <summary>
    /// Can be used to inflict damage on an enemy.
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(int damage, string damageSourceClass)
    {
        // Ignore damage while computing path
        if (agent.pathPending) return;

        // Checking damage type
        if (damageSourceClass.CompareTo("LaserWeapon") == 0)
        {
            damage = (int)Mathf.Round((float)damage * multiplierLaserDamageTaken);
        }
        else if (damageSourceClass.CompareTo("CannonProjectile") == 0 || damageSourceClass.CompareTo("GraviProjectile") == 0)
        {
            damage = (int)Mathf.Round((float)damage * multiplierAoeDamageTaken);
        }

        health -= damage;
        if (health <= 0 && !destroyed)
        {
            destroyed = true;
            GameManager.Singleton.DecrementEnemyCounter();
            GameManager.Singleton.IncrementKillCounter();

            // Particle effect
            Instantiate(fx_enemyDeathPrefab, transform.position, Quaternion.identity, null);

            // Sound effect
            SoundController.Singleton.createSoundObject(SoundController.Singleton.enemyDie, transform.position, Quaternion.identity);

            // Drop magic
            if (Random.Range(0, 1f) < magicDropProbability)
            {
                for (int k = 0; k < magicDropAmount; k++)
                {
                    Vector3 randomOffset = new Vector3(Random.Range(-.5f, .5f), Random.Range(-.5f, .5f), Random.Range(-.5f, .5f));
                    Instantiate(magicPrefab, transform.position + randomOffset, Quaternion.identity, null);
                }
            }

            // Spawn child enemies 
            if (spawnsChildEnemies)
            {
                for (int k = 0; k < childEnemyCount; k++)
                {
                    Vector3 randomOffset = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
                    Enemy newEnemy = Instantiate(childEnemyPrefab.gameObject, transform.position + randomOffset, Quaternion.identity, null).GetComponent<Enemy>();
                    newEnemy.agent.SetDestination(GameManager.Singleton.mapGenerator.playerBase.transform.position);
                }
            }

            Destroy(gameObject);
        }
        else
        {
            // Otherwise adjust transparency to visualize current health
            Color newColor = material.color;
            float alpha = (health / (float)maxHealth) * 0.75f + 0.25f;
            newColor.a = Mathf.Max(alpha, 0.25f);
            material.color = newColor;
            SoundController.Singleton.playSoundeffect(SoundController.Singleton.enemyHit, audioSource, 0.15f);
        }
    }

    /// <summary>
    /// Deactivates the nav mesh agent so that physics can be applied.
    /// </summary>
    /// <param name="duration">The duration of the deactivation in seconds.</param>
    public void DeactivateAgent(float duration)
    {
        agent.enabled = false;
        GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine(ActivateAgent(duration));
    }

    private IEnumerator ActivateAgent(float duration)
    {
        yield return new WaitForSeconds(duration);
        GetComponent<Rigidbody>().isKinematic = true;
        agent.enabled = true;
        agent.SetDestination(GameManager.Singleton.mapGenerator.playerBase.transform.position);
    }

    /// <summary>
    /// Reduces the agent speed for a certain duration.
    /// </summary>
    /// <param name="slow">The target speed given in percent.</param>
    /// <param name="duration">The duration of the slow in seconds.</param>
    public void SlowFor(float slow, float duration)
    {
        agent.speed = slow * speed;
        StartCoroutine(ResetSpeedIn(duration));
    }

    private IEnumerator ResetSpeedIn(float duration)
    {
        yield return new WaitForSeconds(duration);
        agent.speed = speed;
    }
}