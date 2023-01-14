using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI highscore;
    public Options options;

    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey("highscore")) highscore.text = "0";
        else highscore.text = "" + PlayerPrefs.GetInt("highscore");

        SoundController.Singleton.ChangeMusic(SoundController.Singleton.musicMenu);

        options.gameObject.SetActive(true);
        options.InitialOptionValues();
        options.gameObject.SetActive(false);

        SoundController.Singleton.StartMusic();
    }


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadPlayScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
