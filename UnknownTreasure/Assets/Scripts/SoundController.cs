using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    #region Singleton
    private static SoundController _instance;

    public static SoundController Singleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<SoundController>();
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance != null)
        {
            GameObject.DestroyImmediate(gameObject);
        }
        else
            instance = gameObject.GetComponent<SoundController>();
    }

    static bool doneOnce;

    public static SoundController Instance
    {
        [System.Diagnostics.DebuggerStepThrough]
        get
        {
            if (instance == null)
            {
                instance = (SoundController)GameObject.FindObjectOfType(typeof(SoundController)); // not really any point to doing this (it will fail), Awake would have happened but it's possible your code got here before Awake

                if (instance == null && !doneOnce)
                {
                    doneOnce = true;
                    Debug.LogError($"!!! An instance of type {typeof(SoundController)} is needed in the scene, but there is none !!!");
                }
            }

            return instance;
        }
    }

    private static SoundController instance;
    #endregion

    private float soundEffectVolume;
    private float musicVolume;

    [SerializeField] public AudioSource controllerAudioSource;
    [SerializeField] public AudioSource musicAudioSource;
    public AudioClip buildTower;
    public AudioClip enemyHit;
    public AudioClip enemyDie;
    public AudioClip damageTower;
    public AudioClip pickUpMagic;
    public AudioClip woodHit;
    public AudioClip stoneHit;
    public AudioClip enterTower;
    public AudioClip leaveTower;
    public AudioClip footsteps;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip denyBuild;
    public AudioClip gameOver;
    public AudioClip youWon;
    public AudioClip startGame;
    public AudioClip waveStart;
    public AudioClip pauseMenu;
    public AudioClip buildingMenu;
    public AudioClip uiClick;
    public AudioClip weaponCanon;
    public AudioClip weaponArrow;
    public AudioClip weaponLaser;
    public AudioClip weaponGravi;
    public AudioClip projectileCanon;
    public AudioClip projectileGravi;

    public AudioClip musicMenu;
    public List<AudioClip> musicWaveList;
    // public AudioClip musicWave;
    public AudioClip musicPause;

    public SoundFX soundObjectPrefab;

    public void StartMusic()
    {
        UpdateVolumes();

        musicAudioSource.clip = musicPause;
        musicAudioSource.Play();
    }

    public void UpdateVolumes()
    {
        if (!PlayerPrefs.HasKey("musicVolume")) return;

        soundEffectVolume = PlayerPrefs.GetFloat("soundEffectsVolume");
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        musicAudioSource.volume = musicVolume;
    }

    public void playSoundeffect(AudioClip audioClip, AudioSource audioSource, float volume = 1)
    {
        audioSource.clip = audioClip;
        audioSource.PlayOneShot(audioSource.clip, soundEffectVolume * volume );
    }

    public void createSoundObject(AudioClip audioClip,  Vector3 position, Quaternion  rotation, float volume = 1)
    {
        SoundFX soundFX = Instantiate(soundObjectPrefab, position, rotation, null);
        playSoundeffect(audioClip, soundFX.audioSource);
    }

    public void ChangeMusic(AudioClip music)
    {
        musicAudioSource.clip = music;
        UpdateVolumes();
        musicAudioSource.Play();
    }

    public AudioClip GetWaveMusic()
    {
        return musicWaveList[(GameManager.Singleton.CurrentWave % musicWaveList.Count)];
    }
}
