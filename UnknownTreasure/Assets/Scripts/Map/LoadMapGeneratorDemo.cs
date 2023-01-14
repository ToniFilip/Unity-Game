using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMapGeneratorDemo : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            SceneManager.LoadScene("MapGeneratorDemo");
        }
    }
}
