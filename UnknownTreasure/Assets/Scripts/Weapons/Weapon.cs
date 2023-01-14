using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float baseRange = 50f;
    public int baseDamage = 50;
    public float baseFireRate = 5f;

    [HideInInspector] public float range;
    [HideInInspector] public int damage;
    [HideInInspector] public float fireRate;

    [SerializeField] public AudioSource audioSource;
    protected AudioClip shootSound;

    /// <summary>
    /// This should not be overriden by subclasses.
    /// </summary>
    private void Awake()
    {
        range = baseRange;
        damage = baseDamage;
        fireRate = baseFireRate;

        SelectSound();
    }

    public bool preAim = false;

    public abstract void Shoot(Transform firePoint, Transform referencePoint);

    public abstract void Shoot(Vector3 from, Vector3 to);

    public abstract void SelectSound();
}