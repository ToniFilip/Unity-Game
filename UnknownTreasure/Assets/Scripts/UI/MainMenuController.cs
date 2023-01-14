using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;  

namespace MenuElements
{
    public class MainMenuController : MonoBehaviour
    {
        public UIDocument mainMenuScreen;

        VisualTreeAsset itemTemplate;

        void OnEnable()
        {
            Debug.Log("GameOver Screen enabled");
            var root = mainMenuScreen.rootVisualElement;
            // Button
            VisualElement background = root.Q<VisualElement>("gameOverScreen");
            Label gameOverLabel = root.Q<Label>("gameOverLabel");
            Label gameOverLabelSideText = root.Q<Label>("sideText");
            Button replayButton = root.Q<Button>("replay");
            Button menuButton = root.Q<Button>("menu");
            
            Color backgroundColor = new Color((float) 47/255, (float) 54/255, (float) 64/255, 1);
            background.style.backgroundColor = backgroundColor;

            gameOverLabel.text = "Unknown Treasure";

            gameOverLabelSideText.text = "";

            replayButton.clicked += () => StartLevel();
            replayButton.clicked += () => SoundController.Singleton.playSoundeffect(SoundController.Singleton.uiClick, SoundController.Singleton.controllerAudioSource, 0.1f);
            
            replayButton.text = "Start";
            menuButton.clicked += () => QuitGame();
            menuButton.clicked += () => SoundController.Singleton.playSoundeffect(SoundController.Singleton.uiClick, SoundController.Singleton.controllerAudioSource, 0.1f);
            menuButton.text = "Quit";


            // Button Hover
            Color colorUnselected = new Color((float) 47/255, (float) 54/255, (float) 64/255);
            Color colorSelected = new Color((float) 127/255, (float) 143/255, (float) 166/255);
            Button[] buttons = { replayButton, menuButton };
            foreach(Button btn in buttons)
            {
                btn.RegisterCallback<MouseOverEvent>((type) =>
                {
                    btn.style.borderLeftColor = colorSelected;
                    btn.style.borderRightColor = colorSelected;
                    btn.style.borderTopColor = colorSelected;
                    btn.style.borderBottomColor = colorSelected;
                });
                btn.RegisterCallback<MouseOutEvent>((type) =>
                {
                    btn.style.borderLeftColor = colorUnselected;
                    btn.style.borderRightColor = colorUnselected;
                    btn.style.borderTopColor = colorUnselected;
                    btn.style.borderBottomColor = colorUnselected;
                });
            }
        }

        void StartLevel()
        {
            mainMenuScreen.gameObject.SetActive(false);
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            SceneManager.LoadScene("Level_1");
        }

        void QuitGame()
        {
            Application.Quit();
        }
    }
}
