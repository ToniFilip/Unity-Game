using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFX : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3);
    }
}
