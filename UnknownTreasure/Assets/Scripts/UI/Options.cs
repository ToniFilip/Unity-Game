using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Slider musicVolume;
    public Slider soundEffectsVolume;
    public Slider mouseSensitivity;

    public void InitialOptionValues()
    {
        Debug.Log("Initializing option values.");
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume.value = PlayerPrefs.GetFloat("musicVolume");
            soundEffectsVolume.value = PlayerPrefs.GetFloat("soundEffectsVolume");
            mouseSensitivity.value = PlayerPrefs.GetFloat("mouseSensitivity");
        }
        else
        {
            PlayerPrefs.SetFloat("musicVolume", .1f);
            PlayerPrefs.SetFloat("soundEffectsVolume", .5f);
            PlayerPrefs.SetFloat("mouseSensitivity", 5f);
        }
    }

    private void Update()
    {
        PlayerPrefs.SetFloat("musicVolume", musicVolume.value);
        PlayerPrefs.SetFloat("soundEffectsVolume", soundEffectsVolume.value);
        PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity.value);
        SoundController.Singleton.UpdateVolumes();
    }
}