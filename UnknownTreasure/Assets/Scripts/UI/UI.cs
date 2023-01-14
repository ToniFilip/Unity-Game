using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using HudElements;
using UIMenues;
using TMPro;

public class UI : MonoBehaviour
{
    #region Singleton
    private static UI _instance;

    public static UI Singleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<UI>();
            }

            return _instance;
        }
    }

    // void Awake()
    // {
    //     DontDestroyOnLoad(gameObject);
    // }
    #endregion

    public UIDocument hud;
    public UIDocument buildingMenu;
    public UIDocument gameOverScreen;
    public UIDocument pauseMenu;

    private HealthBar healthbar;
    private ResourceCount resCount1;
    private ResourceCount resCount2;
    private ResourceCount resCount3;
    private InventorySlot invSlot1;
    private InventorySlot invSlot2;
    private Label label1;
    private Label label2;

    // private GameOverScreen gameOverScreenElement;

    // private BuildingMenu bm;


    public Texture2D woodImg;
    public Texture2D stoneImg;
    public Texture2D magicImg;
    /*public string imgSource1 = "Placeholder/ph_pickaxe";
    public string imgSource2 = "Placeholder/ph_pickaxe";*/
    public bool selected1 = true;
    public bool selected2 = false;


    // State variables
    public bool gameOverScreenOpen = false;
    public bool pauseMenuOpen = false;
    public bool buildingMenuOpen = false;

    public TextMeshProUGUI interactDisplay;

    void OnEnable()
    {
        BindUI();

        // var rootBM = buildingMenu.rootVisualElement;
        // bm = rootBM.Q<BuildingMenu>();

        // var rootGO = gameOverScreen.rootVisualElement;
        // gameOverScreenElement = rootGO.Q<GameOverScreen>("gameOverScreen");
    }

    private void Update()
    {
        if(healthbar != null)
        {
            healthbar.value = GameManager.Singleton.treasureTowerHealth / 100f;
        }
        if (resCount1 != null)
        {
            resCount1.value = GameManager.Singleton.wood;
        }
        if (resCount2 != null)
        {
            resCount2.value = GameManager.Singleton.stone;
        }
        if (resCount3 != null)
        {
            resCount3.value = GameManager.Singleton.magic;
        }
        // if (invSlot1 != null)
        // {
        //     invSlot1.value = (selected1, imgSource1);
        // }
        // if (invSlot2 != null)
        // {
        //     invSlot2.value = (selected2, imgSource2);
        // }
        if (label1 != null && label2 != null)
        {
            UpdateWaveLabel();
        }
    }

    public void BindUI()
    {
        var root = hud.rootVisualElement;
        healthbar = root.Q<HealthBar>();
        healthbar.value = 1; //GameManager.Singleton.player.healthPercent; TODO: Why does it not work?
        resCount1 = root.Q<ResourceCount>("wood");
        resCount1.value = GameManager.Singleton.wood;
        resCount2 = root.Q<ResourceCount>("stone");
        resCount2.value = GameManager.Singleton.stone;
        resCount3 = root.Q<ResourceCount>("magic");
        resCount3.value = GameManager.Singleton.magic;
        // invSlot1 = root.Q<InventorySlot>("inventoryslot1");
        // invSlot1.value = (selected1, imgSource1);
        // invSlot2 = root.Q<InventorySlot>("inventoryslot2");
        // invSlot2.value = (selected2, imgSource2);

        // Top right corner
        label1 = root.Q<Label>("label1");
        label2 = root.Q<Label>("label2");

        SetResourceImages();
    }

    private void SetResourceImages()
    {
        var root = hud.rootVisualElement;
        resCount1 = root.Q<ResourceCount>("wood");
        resCount2 = root.Q<ResourceCount>("stone");
        resCount3 = root.Q<ResourceCount>("magic");

        VisualElement resCount1Img = resCount1.Q<VisualElement>("img");
        VisualElement resCount2Img = resCount2.Q<VisualElement>("img");
        VisualElement resCount3Img = resCount3.Q<VisualElement>("img");

        /*resCount1Img.style.width = 80;
        resCount1Img.style.height = 80;
        resCount2Img.style.width = 80;
        resCount2Img.style.height = 80;
        resCount3Img.style.width = 80;
        resCount3Img.style.height = 80;*/

        if (woodImg!=null && stoneImg!=null && magicImg!=null)
        {
            Color transparent = new Color(0, 0, 0, 0);
            resCount1Img.style.backgroundColor = transparent;
            resCount2Img.style.backgroundColor = transparent;
            resCount3Img.style.backgroundColor = transparent;
            resCount1Img.style.backgroundImage = woodImg;
            resCount2Img.style.backgroundImage = stoneImg;
            resCount3Img.style.backgroundImage = magicImg;
        }
    }

    public void ToggleBuildingMenu()
    {
        // if(gameOverScreenOpen) return;
        if(GameManager.Singleton.isMenuOpen)
        {
            GameManager.Singleton.isMenuOpen = false;
            buildingMenu.gameObject.SetActive(false);
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            // Destroy(buildingMenu);
        }
        else{
            // Instantiate(buildingMenu);
            GameManager.Singleton.isMenuOpen = true;
            buildingMenu.gameObject.SetActive(true);
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OpenGameOverScreen(bool isWin)
    {
        GameManager.Singleton.isMenuOpen = true;
        gameOverScreenOpen = true;
        buildingMenu.gameObject.SetActive(false);
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        hud.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(true);
    }

    void UpdateWaveLabel()
    {
        if(GameManager.Singleton.CurrentPauseTime <= 0)
        {
            label1.text = (GameManager.Singleton.enemiesOnMap).ToString() + " enemies left";

            if (GameManager.Singleton.waveSettings.isEndless)
            {
                label2.text = "Wave " + (GameManager.Singleton.CurrentWave + 1).ToString();
            } else
            {
                label2.text = "Wave " + (GameManager.Singleton.CurrentWave + 1).ToString() + "/" + GameManager.Singleton.waveSettings.waves;
            }
        }
        else
        {
            if (GameManager.Singleton.waveSettings.isEndless)
            {
                label1.text = ((int)GameManager.Singleton.CurrentPauseTime + 1).ToString() + "s until Wave " + (GameManager.Singleton.CurrentWave + 2).ToString();
            } else
            {
                label1.text = ((int)GameManager.Singleton.CurrentPauseTime + 1).ToString() + "s until Wave " + (GameManager.Singleton.CurrentWave + 2).ToString() + "/" + GameManager.Singleton.waveSettings.waves;
            }
            label2.text = "Press C to continue";
        }
        
    }

    public void SetScore(int value)
    {
        var root = gameOverScreen.rootVisualElement;
        Label gameOverLabelScore = root.Q<Label>("score");
        gameOverLabelScore.text = "Score: " + value.ToString();
    }
}