using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieFX : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        SoundController.Singleton.playSoundeffect(SoundController.Singleton.enemyDie, audioSource, 0.5f);
    }
}
