using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;  


namespace MenuElements
{
    public class GameOverScreenController : MonoBehaviour
    {
        public UIDocument gameOverScreen;

        VisualTreeAsset itemTemplate;

        void OnEnable()
        {
            var root = gameOverScreen.rootVisualElement;
            // Button
            Label gameOverLabel = root.Q<Label>("gameOverLabel");
            Label gameOverLabelSideText = root.Q<Label>("sideText");
            Label gameOverLabelScore = root.Q<Label>("score");
            Button replayButton = root.Q<Button>("replay");
            Button menuButton = root.Q<Button>("menu");
            
            replayButton.clicked += () => Replay();
            replayButton.clicked += () => SoundController.Singleton.playSoundeffect(SoundController.Singleton.uiClick, SoundController.Singleton.controllerAudioSource, 0.1f);
            replayButton.text = "Replay Level";

            menuButton.clicked += () => BackToMenu();
            menuButton.clicked += () => SoundController.Singleton.playSoundeffect(SoundController.Singleton.uiClick, SoundController.Singleton.controllerAudioSource, 0.1f);
            menuButton.text = "Back To Menu";

            if(GameManager.Singleton.treasureTowerHealth > 0)
            {
                gameOverLabel.text = "Congratulations!";
            }
            else
            {
                gameOverLabel.text = "Game over!";
            }
            gameOverLabelSideText.text = "You killed " + (GameManager.Singleton.getKillCounter()).ToString() + " enemies.";

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

        void Replay()
        {
            gameOverScreen.gameObject.SetActive(false);
            // GameManager.Singleton.LoadNewScene("UITest", true, true);
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale =  1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        void BackToMenu()
        {
            gameOverScreen.gameObject.SetActive(false);
            // GameManager.Singleton.LoadNewScene("MainMenu", false, false);
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MainMenu");
        }

        void OnDisable()
        {
            Debug.Log("End Gameover screen");
            GameManager.Singleton.isMenuOpen = false;
            UI.Singleton.gameOverScreenOpen = false;
        }
    }
}
