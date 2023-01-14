using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Sets map generator values for the map generator demo scene.
/// </summary>
public class MapGeneratorDemo : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public TMP_InputField seed;

    public Slider sizeSlider;
    public TMP_InputField sizeInputField;
    public Slider spawnsSlider;
    public TMP_InputField spawnsInputField;
    public Slider accessesSlider;
    public TMP_InputField accessesInputField;

    #region Heightmap
    public Image heightMapPreview;
    public TextMeshProUGUI heightMapName;
    private List<Texture2D> heightmaps;
    private List<string> heightmapNames;
    public int heightMapIndex = 0;

    private void LoadHeightmaps(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        DirectoryInfo info = new DirectoryInfo(path);
        FileInfo[] fileInfo = info.GetFiles();
        List<Texture2D> heightmaps = new List<Texture2D>();
        List<string> heightmapNames = new List<string>();
        foreach (FileInfo file in fileInfo)
        {
            Debug.Log("Loading: " + file.FullName);
            byte[] bytes = File.ReadAllBytes(file.FullName);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            heightmaps.Add(texture);
            heightmapNames.Add(file.Name);
        }

        this.heightmaps =  heightmaps;
        this.heightmapNames = heightmapNames;
    }

    public void NextHeightmap(int change)
    {
        heightMapIndex = (heightMapIndex + change) % heightmaps.Count;
        if (heightMapIndex < 0) heightMapIndex = heightmaps.Count - 1;

        mapGenerator.heightMap = heightmaps[heightMapIndex];
        heightMapName.text = heightmapNames[heightMapIndex];
        heightMapPreview.enabled = true;
        heightMapPreview.sprite = Sprite.Create(mapGenerator.heightMap, new Rect(0, 0, mapGenerator.heightMap.width, mapGenerator.heightMap.height), new Vector2(0.5f, 0.5f));
    }

    public void ClearHeightmap()
    {
        heightMapIndex = 0;
        mapGenerator.heightMap = null;
        heightMapPreview.enabled = false;
        heightMapName.text = "-";
    }

    private void Awake()
    {
        string heightmapsPath = Application.persistentDataPath + "/Heightmaps";

        // Create heightmap folder if not existent
        if (!File.Exists(heightmapsPath))
        {
            Directory.CreateDirectory(heightmapsPath);

            // Add precreated heightmaps
            Texture2D[] textures = Resources.LoadAll<Texture2D>("Heightmaps");
            foreach (Texture2D t in textures)
            {
                byte[] bytes = t.EncodeToPNG();
                File.WriteAllBytes(heightmapsPath + "/" + t.name + ".png", bytes);
            }
        }

        // Load heightmaps from folder
        LoadHeightmaps(heightmapsPath);
    }
    #endregion

    public Toggle endlessToggle;
    public TMP_InputField wavesInputField;
    public TMP_Dropdown difficultyDropdown;
    public Slider mapDifficultySlider;

    public GameObject createLevelObjects;
    public GameObject playLevelObjects;
    public GameObject player;
    public Transform creatorCamera;
    public Transform gameCamera;
    public Transform water;

    public List<Enemy> enemyTypes;

    private void Start()
    {
        sizeInputField.onEndEdit.AddListener(delegate { Inputs(); });
        spawnsInputField.onEndEdit.AddListener(delegate { Inputs(); });
        accessesInputField.onEndEdit.AddListener(delegate { Inputs(); });

        sizeSlider.onValueChanged.AddListener(delegate { Sliders(); });
        spawnsSlider.onValueChanged.AddListener(delegate { Sliders(); });
        accessesSlider.onValueChanged.AddListener(delegate { Sliders(); });

        seed.onValueChanged.AddListener(delegate { UpdateSeed(); });

        endlessToggle.onValueChanged.AddListener(delegate { UpdateGamesettings(); });
        wavesInputField.onEndEdit.AddListener(delegate { UpdateGamesettings(); });
        difficultyDropdown.onValueChanged.AddListener(delegate { UpdateGamesettings(); });

        Sliders();
        UpdateGamesettings();

        mapGenerator.GenerateMap();
        mapGenerator.CreateMapBorder();
        AdjustCameraAndWater();
        ComputeAndDisplayDifficulties();

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// Get changes from sliders.
    /// </summary>
    private void Sliders()
    {
        int size = (int)sizeSlider.value;
        if (size % 2 == 0) size++;

        int spawns = (int)spawnsSlider.value;
        int accesses = (int)accessesSlider.value;

        sizeInputField.text = "" + size;
        spawnsInputField.text = "" + spawns;
        accessesInputField.text = "" + accesses;

        SetMapGeneratorValues(size, spawns, accesses);
    }

    /// <summary>
    /// Get changes from input field.
    /// </summary>
    private void Inputs()
    {
        int size;
        if (!int.TryParse(sizeInputField.text, out size)) return;
        if (size % 2 == 0) size++;
        size = Mathf.Max(Mathf.Min(size, (int)sizeSlider.maxValue), (int) sizeSlider.minValue);

        int spawns;
        if (!int.TryParse(spawnsInputField.text, out spawns)) return;
        spawns = Mathf.Max(Mathf.Min(spawns, (int)spawnsSlider.maxValue), (int) spawnsSlider.minValue);

        int accesses;
        if (!int.TryParse(accessesInputField.text, out accesses)) return;
        accesses = Mathf.Max(Mathf.Min(accesses, (int)accessesSlider.maxValue), (int)accessesSlider.minValue);

        sizeSlider.value = size;
        spawnsSlider.value = spawns;
        accessesSlider.value = accesses;

        sizeSlider.value = size;
        spawnsSlider.value = spawns;
        accessesSlider.value = accesses;

        SetMapGeneratorValues(size, spawns, accesses);
    }

    private void UpdateSeed()
    {
        mapGenerator.seed = seed.text;
    }

    private void SetMapGeneratorValues(int size, int spawns, int accesses)
    {
        mapGenerator.size = size;
        mapGenerator.spawns = spawns;
        mapGenerator.accesses = accesses;
    }

    private void UpdateGamesettings()
    {
        bool isEndless = endlessToggle.isOn;
        if (isEndless) wavesInputField.gameObject.SetActive(false);
        else wavesInputField.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        // Create map (or at least try to)
        if (!mapGenerator.GenerateMap()) return;
        mapGenerator.CreateMapBorder();
        mapGenerator.LoadMapVariables();

        // Adjust camera (for player spawn)
        AdjustCameraAndWater();

        // Bake navmesh
        mapGenerator.GetComponent<NavMeshSurface>().BuildNavMesh();
        Debug.Log("Navmesh baking done.");

        // Set correct gameobjects active and disable level creation UI
        Vector3 newPlayerPos = new Vector3(creatorCamera.position.x + 3f, gameCamera.transform.position.y, creatorCamera.position.z);
        player.transform.position = newPlayerPos;

        for (int k = 0; k < transform.childCount - 1; k++)
        {
            transform.GetChild(k).gameObject.SetActive(false);
        }

        // Animation
        StartCoroutine(AnimateCamera(creatorCamera, creatorCamera.position, creatorCamera.rotation.eulerAngles, newPlayerPos, gameCamera.rotation, 0, 2f));
    }

    private IEnumerator AnimateCamera(Transform camera, Vector3 sourcePosition, Vector3 sourceRotation, Vector3 targetPosition, Quaternion targetRotation, float currentTime, float totalTime)
    {
        camera.position = Vector3.Lerp(sourcePosition, targetPosition, currentTime / totalTime);
        camera.rotation = Quaternion.Lerp(Quaternion.Euler(sourceRotation), targetRotation, currentTime / totalTime);
        yield return new WaitForEndOfFrame();
        currentTime += Time.deltaTime;

        if ((currentTime / totalTime) > 1f) { StartCustomLevel(); }
        else StartCoroutine(AnimateCamera(camera, sourcePosition, sourceRotation, targetPosition, targetRotation, currentTime, totalTime));
    }

    /// <summary>
    /// Part of StartGame method.
    /// </summary>
    private void StartCustomLevel()
    {
        // Set correct gameobjects active and disable level creation
        createLevelObjects.SetActive(false);
        playLevelObjects.SetActive(true);

        // Difficulties
        ComputeAndDisplayDifficulties();

        // Save level difficulty
        GameManager.Singleton.waveSettings = GenerateWaveSettings();
        GameManager.Singleton.waveSettings.levelDifficulty = difficultyDropdown.value;

        // Generate level settings and initiate game manager
        GameManager.Singleton.Initiate();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private WaveSettings GenerateWaveSettings()
    {
        WaveSettings waveSettings = new WaveSettings();

        // Waves
        waveSettings.isEndless = endlessToggle.isOn;
        if (endlessToggle.isOn)
        {
            waveSettings.waves = 1000;
            
        } else
        {
            if (!int.TryParse(wavesInputField.text, out waveSettings.waves))
            {
                waveSettings.waves = 10;
            }
        }

        // Cardinal points and pause time
        waveSettings.cardinalPointSettings = new CardinalPointSetting[] { new CardinalPointSetting(), new CardinalPointSetting(), new CardinalPointSetting(), new CardinalPointSetting() };
        int difficulty = difficultyDropdown.value;

        waveSettings.cardinalPointSettings[0].cardinalPoint = CardinalPoint.North;
        waveSettings.cardinalPointSettings[1].cardinalPoint = CardinalPoint.East;
        waveSettings.cardinalPointSettings[2].cardinalPoint = CardinalPoint.South;
        waveSettings.cardinalPointSettings[3].cardinalPoint = CardinalPoint.West;

        // Last wave when all enemy types are spawned
        int waveCopy = 20;

        switch (difficulty)
        {
            case 0: // Easy
                waveSettings.cardinalPointSettings[1].openFromWave = waveCopy / 2;
                waveSettings.cardinalPointSettings[2].openFromWave = int.MaxValue;
                waveSettings.cardinalPointSettings[3].openFromWave = int.MaxValue;
                waveSettings.maxPauseTime = 60f;
                break;
            case 1: // Medium
                waveSettings.cardinalPointSettings[1].openFromWave = waveCopy / 4;
                waveSettings.cardinalPointSettings[2].openFromWave = waveCopy / 3;
                waveSettings.cardinalPointSettings[3].openFromWave = waveCopy / 2;
                waveSettings.maxPauseTime = 40f;
                break;
            case 2: // Hard
                waveSettings.cardinalPointSettings[1].openFromWave = 0;
                waveSettings.cardinalPointSettings[2].openFromWave = waveCopy / 4;
                waveSettings.cardinalPointSettings[3].openFromWave = waveCopy / 2;
                waveSettings.maxPauseTime = 20f;
                break;
            case 3: // Ultra
                waveSettings.cardinalPointSettings[1].openFromWave = 0;
                waveSettings.cardinalPointSettings[2].openFromWave = 0;
                waveSettings.cardinalPointSettings[3].openFromWave = 0;
                waveSettings.maxPauseTime = 10f;
                break;
        }

        // Create enemy batches
        waveSettings.enemySettings = new List<EnemySettings>();

        // For each enemy type
        foreach (Enemy enemy in enemyTypes)
        {
            EnemySettings enemySetting = new EnemySettings();
            float enenemyEasyness = EnemyEasyness(enemy);
            enemySetting.enemyPrefab = enemy;
            enemySetting.lastWave = waveSettings.waves;
            enemySetting.burstCount = Mathf.Max(1, 1 + (int)(difficulty * enemy.difficulty) - (int)(10 * enemy.difficulty * enemy.difficulty));
            enemySetting.burstStrength = Mathf.Max(1, (int)(0.75f * enenemyEasyness + difficulty - (int)(10 * enemy.difficulty)));
            enemySetting.wavePeriod = 1;
            enemySetting.burstPeriod = Mathf.Max(0, 10 - difficulty);
            enemySetting.burstStrengthIncrease = 0.5f * (1 + difficulty) * enenemyEasyness;
            enemySetting.burstDelay = 10f * enemy.difficulty * enemy.difficulty;

            // From when should this enemy be spawned?
            enemySetting.firstWave = Mathf.Max(0, (int)(waveCopy * enemy.difficulty));

            // Add to wave settings
            waveSettings.enemySettings.Add(enemySetting);
        }

        // Event waves
        // TODO: bosses and special waves (only fast enemies etc.)
        for (int k = 4; k < waveSettings.waves; k += 5)
        {
            float random = Random.Range(0, 1f);
            float threshold = Mathf.Min(1f, 0.2f * (1 + difficulty));

            if (random < threshold)
            {
                waveSettings.enemySettings.Add(RandomSpecialWave(k, difficulty));
            }
        }

        return waveSettings;
    }

    private float EnemyEasyness(Enemy enemy)
    {
        return (1 / Mathf.Max(0.2f, 2 * enemy.difficulty));
    }

    private EnemySettings RandomSpecialWave(int wave, int difficulty)
    {
        EnemySettings enemySettings = new EnemySettings();
        int random = Random.Range(0, 1);

        switch (random)
        {
            case 0:
                enemySettings.enemyPrefab = enemyTypes[Random.Range(0, enemyTypes.Count)];
                float enenemyEasyness = EnemyEasyness(enemySettings.enemyPrefab);
                enemySettings.burstStrength = Mathf.Max(1, (int)(difficulty * enemySettings.enemyPrefab.difficulty * 0.5f) + (int)(enenemyEasyness));
                enemySettings.burstCount = 1;
                break;
        }

        enemySettings.firstWave = wave;
        enemySettings.lastWave = wave;
        enemySettings.burstDelay = 30f;

        return enemySettings;
    }

    public void ComputeAndDisplayDifficulties()
    {
        // Compute and display map difficulty
        mapGenerator.ComputeMapDifficulty(sizeSlider.maxValue, spawnsSlider.maxValue);
        mapDifficultySlider.value = mapGenerator.mapDifficulty;
    }

    #region UI Buttons
    public void AdjustCameraAndWater()
    {
        if (mapGenerator.playerBase != null)
        {
            creatorCamera.position = new Vector3(mapGenerator.playerBase.transform.position.x, 32 * mapGenerator.size, mapGenerator.playerBase.transform.position.z);
            water.position = new Vector3(mapGenerator.playerBase.transform.position.x, water.position.y, mapGenerator.playerBase.transform.position.z);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    #endregion
}