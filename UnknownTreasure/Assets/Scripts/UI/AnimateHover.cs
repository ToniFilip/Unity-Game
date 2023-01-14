using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateHover : MonoBehaviour
{
    public float speed;
    public float amplitude;

    private float localTime;

    private void Update()
    {
        localTime += Time.deltaTime * speed;
        transform.position += amplitude * Vector3.up * Mathf.Sin(localTime) * Time.deltaTime;
    }
}
