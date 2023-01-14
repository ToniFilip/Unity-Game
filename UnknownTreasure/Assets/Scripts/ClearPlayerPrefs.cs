using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPlayerPrefs : MonoBehaviour
{
    public bool clear;

    private void Update()
    {
        if (clear)
        {
            Debug.Log("Deleted all player prefs.");
            PlayerPrefs.DeleteAll();
            clear = false;
        }
    }
}
