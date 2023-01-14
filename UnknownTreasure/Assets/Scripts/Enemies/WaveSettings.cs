using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveSettings
{
    public bool isEndless = false;
    public int waves = 10;
    public float maxPauseTime = 30f;
    public float levelDifficulty;
    public CardinalPointSetting[] cardinalPointSettings = new CardinalPointSetting[4];

    public List<EnemySettings> enemySettings;
}

[System.Serializable]
public class CardinalPointSetting
{
    public CardinalPoint cardinalPoint;
    public int openFromWave;
}