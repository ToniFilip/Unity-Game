using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySettings
{
    public Enemy enemyPrefab;

    public int firstWave;
    public int lastWave;

    /// <summary>
    /// The amount of waves (+1) between a batch is spawned.
    /// </summary>
    public int wavePeriod;

    /// <summary>
    /// The base count of enemies being spawned in one burst.
    /// </summary>
    public int burstStrength;

    /// <summary>
    /// The increase of a burst per wave (in wave k additional burstStrengthIncrease * k enemies will be spawned).
    /// </summary>
    public float burstStrengthIncrease;

    /// <summary>
    /// The amount of bursts being spawned in one wave.
    /// Bursts are distributed across spawn tiles.
    /// </summary>
    public int burstCount;

    /// <summary>
    /// Duration in seconds between two bursts.
    /// </summary>
    public float burstPeriod;

    /// <summary>
    /// The initial duration in seconds before the first burst is spawned during a wave.
    /// </summary>
    public float burstDelay;

    [HideInInspector] public int burstsLeft;

    public void Reset()
    {
        burstsLeft = burstCount;
    }
}