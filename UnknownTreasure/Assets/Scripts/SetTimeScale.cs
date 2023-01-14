using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTimeScale : MonoBehaviour
{
    [Range(1f, 10f)]
    public float timeScale = 1f;

    private void Update()
    {
        Time.timeScale = timeScale;
    }
}
