using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;  

namespace MenuElements
{
    public class PauseMenuController : MonoBehaviour
    {
        public UIDocument pauseMenu;

        void OnEnable()
        {
            SoundController.Singleton.playSoundeffect(SoundController.Singleton.pauseMenu, SoundController.Singleton.controllerAudioSource, 0.5f);
            SoundController.Singleton.musicAudioSource.Pause();
            UI.Singleton.hud.gameObject.SetActive(false);
            UI.Singleton.pauseMenuOpen = true;
            GameManager.Singleton.isMenuOpen = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            var root = pauseMenu.rootVisualElement;
            // Button
            Label gameOverLabel = root.Q<Label>("gameOverLabel");
            Label gameOverLabelSideText = root.Q<Label>("sideText");
            Button continueButton = root.Q<Button>("replay");
            Button menuButton = root.Q<Button>("menu");
            
            continueButton.clicked += () => ContinueLevel();
            continueButton.clicked += () => SoundController.Singleton.playSoundeffect(SoundController.Singleton.uiClick, SoundController.Singleton.controllerAudioSource, 0.1f);
            
            continueButton.text = "Continue";

            menuButton.clicked += () => BackToMenu();
            menuButton.clicked += () => SoundController.Singleton.playSoundeffect(SoundController.Singleton.uiClick, SoundController.Singleton.controllerAudioSource, 0.1f);
            menuButton.text = "Back To Menu";

            gameOverLabel.text = "Pause";
            gameOverLabelSideText.text = "";

            // Button Hover
            Color colorUnselected = new Color((float) 47/255, (float) 54/255, (float) 64/255);
            Color colorSelected = new Color((float) 127/255, (float) 143/255, (float) 166/255);
            Button[] buttons = { continueButton, menuButton };
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

        void ContinueLevel()
        {
            pauseMenu.gameObject.SetActive(false);            
        }

        void BackToMenu()
        {
            pauseMenu.gameObject.SetActive(false);
            // GameManager.Singleton.LoadNewScene("MainMenu", false, false);
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MainMenu");
        }

        void OnDisable()
        {
            UI.Singleton.pauseMenuOpen = false;
            GameManager.Singleton.isMenuOpen = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale =  1;
            UI.Singleton.hud.gameObject.SetActive(true);
            UI.Singleton.BindUI();
            SoundController.Singleton.musicAudioSource.Play();
        }
    }
}