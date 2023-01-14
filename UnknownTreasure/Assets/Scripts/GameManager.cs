using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  


public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;

    public static GameManager Singleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    // void Awake()
    // {
    //     DontDestroyOnLoad(gameObject);
    // }
    #endregion

    public GameObject enemyPrefab;

    #region Player
    public Player player;
    public int treasureTowerHealth = 100;
    public bool isInTower = false;

    public int wood;
    public int stone;
    public int magic;
    #endregion

    #region Waves and Enemies
    public MapGenerator mapGenerator;
    public WaveSettings waveSettings = new WaveSettings();

    public int CurrentWave
    {
        get { return currentWave; }
        private set { currentWave = value; }
    }

    [SerializeField] private int currentWave = -1;

    public float CurrentPauseTime
    {
        get { return currentPauseTime; }
        private set { currentPauseTime = value; }
    }
    [SerializeField] private float currentPauseTime = 0;
    private List<CardinalPoint> activeCardinalPoints;
    [SerializeField] public int enemiesOnMap = 0;
    #endregion

    public bool isMenuOpen = false;
    private int killCounter = 0;

    public bool mainMenuPreview = false;

    private void OnEnable()
    {
        Initiate();
    }

    public void Initiate()
    {
        // Get the player in the scene
        if (!mainMenuPreview) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // Start the game
        StartPause();

        if (!mainMenuPreview) SoundController.Singleton.playSoundeffect(SoundController.Singleton.startGame, SoundController.Singleton.controllerAudioSource);
    }

    /// <summary>
    /// Apply damage to the treasure tower of the player.
    /// </summary>
    /// <param name="damage"></param>
    public void DamageTreasureTower(int damage)
    {
        treasureTowerHealth -= damage;

        if (treasureTowerHealth <= 0)
        {
            Debug.Log(" --- PLAYER HAS LOST - GAME IS OVER ---");
            UI.Singleton.OpenGameOverScreen(false);
            UI.Singleton.SetScore(SetHighscore());
        }
    }

    private IEnumerator SpawnEnemyBatch(EnemySettings enemySettings, float delay)
    {
        // Wait the delay (burst period or inital delay)
        yield return new WaitForSeconds(delay);

        // Spawn enemies on random allowed cardinal point from random spawn
        int cp = Random.Range(0, activeCardinalPoints.Count);
        int st = Random.Range(0, mapGenerator.spawnTilesGrouped[activeCardinalPoints[cp]].Count);
        Transform spawnTile = mapGenerator.spawnTilesGrouped[activeCardinalPoints[cp]][st].transform;

        for (int k = 0; k < enemySettings.burstStrength + (int)(enemySettings.burstStrengthIncrease * (currentWave - enemySettings.firstWave)); k++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
            Enemy newEnemy = Instantiate(enemySettings.enemyPrefab, spawnTile.position + randomOffset, Quaternion.identity, null).GetComponent<Enemy>();
            enemiesOnMap += 1;
            newEnemy.agent.SetDestination(mapGenerator.playerBase.transform.position);
        }

        // Check if last burst was spawned
        enemySettings.burstsLeft--;
        if (enemySettings.burstsLeft <= 0) yield return null;
        else StartCoroutine(SpawnEnemyBatch(enemySettings, enemySettings.burstPeriod));
    }

    /// <summary>
    /// Starts the spawn loop for every enemy batch.
    /// </summary>
    private void StartEnemyBatches()
    {
        foreach (EnemySettings enemySettings in waveSettings.enemySettings)
        {
            // Check if batch is to be deployed this wave
            if (EnemySettingsActive(enemySettings))
            {
                Debug.Log("Starting batch: " + enemySettings.enemyPrefab);
                StartCoroutine(SpawnEnemyBatch(enemySettings, enemySettings.burstDelay));
            }
        }
    }

    /// <summary>
    /// Stops all coroutines and starts a pause.
    /// </summary>
    private void StartPause()
    {
        SoundController.Singleton.ChangeMusic(SoundController.Singleton.musicPause);
        Debug.Log("Starting pause...");
        StopAllCoroutines();
        currentPauseTime = waveSettings.maxPauseTime;

        StartCoroutine(UpdatePause());
    }

    private IEnumerator UpdatePause()
    {
        yield return new WaitForEndOfFrame();
        currentPauseTime -= Time.deltaTime;

        // Check if pause has ended
        if (currentPauseTime <= 0) StartNextWave();
        else StartCoroutine(UpdatePause());
    }

    private bool EnemySettingsActive(EnemySettings enemySettings)
    {
        return currentWave >= enemySettings.firstWave
            && currentWave <= enemySettings.lastWave
            && (enemySettings.wavePeriod == 0 || (currentWave - enemySettings.firstWave) % enemySettings.wavePeriod == 0);
    }

    /// <summary>
    /// An update loop to check whether all enemies are defeated.
    /// Should be started when the last enemy batch was deployed.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckWaveStatus(float delay)
    {
        yield return new WaitForSeconds(delay);

        bool burstsFinished = true;
        foreach (EnemySettings enemySettings in waveSettings.enemySettings)
        {
            if ((currentWave > enemySettings.firstWave || currentWave < enemySettings.lastWave)) continue;

            // Check if active bursts are finished
            if (EnemySettingsActive(enemySettings)
                && enemySettings.burstsLeft > 0)
            {
                burstsFinished = false;
            }
        }

        if (!burstsFinished || enemiesOnMap > 0) StartCoroutine(CheckWaveStatus(delay));
        else
        {
            if (currentWave >= waveSettings.waves - 1) StartNextWave();
            else StartPause();
        }
    }

    private void StartNextWave()
    {
        // if (!mainMenuPreview) SoundController.Singleton.playSoundeffect(SoundController.Singleton.waveStart, SoundController.Singleton.controllerAudioSource);
        currentWave++;
        if (!mainMenuPreview) SoundController.Singleton.ChangeMusic(SoundController.Singleton.GetWaveMusic());
        Debug.Log("Start of wave " + currentWave + "...");

        // Check if last wave was last wave
        if (currentWave >= waveSettings.waves)
        {
            Debug.Log("--- PLAYER HAS WON - GAME IS OVER ---");
            UI.Singleton.OpenGameOverScreen(true);
            UI.Singleton.SetScore(SetHighscore());
            return;
        }

        // Reset all enemy batches
        for (int k = 0; k < waveSettings.enemySettings.Count; k++)
        {
            waveSettings.enemySettings[k].Reset();
        }

        // Recalculate new active cardinal points
        activeCardinalPoints = new List<CardinalPoint>();
        for (int k = 0; k < waveSettings.cardinalPointSettings.Length; k++)
        {
            // Check if cardinal point is open for this wave
            // Cardinal points without spawns are not contained in the dictionary
            if (currentWave >= waveSettings.cardinalPointSettings[k].openFromWave && mapGenerator.spawnTilesGrouped.ContainsKey(waveSettings.cardinalPointSettings[k].cardinalPoint))
            {
                activeCardinalPoints.Add(waveSettings.cardinalPointSettings[k].cardinalPoint);
            }
        }

        StopAllCoroutines();
        StartEnemyBatches();
        StartCoroutine(CheckWaveStatus(5f));
    }

    /// <summary>
    /// Computes the score the player achieved in this level.
    /// </summary>
    /// <returns></returns>
    private int ComputeScore(float levelDifficulty, float mapDifficulty, int enemyKills, int currentWave)
    {
        float result = enemyKills * (10 + levelDifficulty) * mapDifficulty;

        if (waveSettings.isEndless) result += currentWave * (mapDifficulty + 1);
        else result += 100;

        return ((int) result);
    }

    /// <summary>
    /// Compares the score with the in the PlayerPrefs saved highscore and updates if accordingly.
    /// </summary>
    /// <returns></returns>
    private int SetHighscore()
    {
        int score = ComputeScore(waveSettings.levelDifficulty, mapGenerator.mapDifficulty, killCounter, currentWave);
        int currentHighscore = PlayerPrefs.GetInt("highscore");

        if (score > currentHighscore)
        {
            PlayerPrefs.SetInt("highscore", score);
            Debug.LogError("New highscore!");
        }
        Debug.Log("Score: " + score +  " " + currentHighscore);

        return score;
    }

    public void DecrementEnemyCounter()
    {
        enemiesOnMap -= 1;

        if (enemiesOnMap < 0)
        {
            Debug.LogError("Less than 0 enemies on map. This should not be the case.");
        }
    }

    public void IncrementKillCounter()
    {
        killCounter += 1;
    }

    public void DecrementKillCounter()
    {
        killCounter -= 1;
    }

    public int getKillCounter()
    {
        return killCounter;
    }

    public void SkipPause()
    {
        currentPauseTime = 0f;
    }

    public void LoadNewScene(string sceneName, bool lockCursor, bool scaleTimer)
    {
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        //Time.timeScale = scaleTimer ? 1 : 0;
        SceneManager.LoadScene(sceneName);
    }
}