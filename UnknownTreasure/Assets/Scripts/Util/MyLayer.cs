using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class can be used to get the gameobject layers globally.
/// </summary>
public class MyLayer : MonoBehaviour
{
    public static int groundLayer = 7;
    public static int groundLayerMask = 1 << groundLayer;

    public static int enemyLayer = 8;
    public static int enemyLayerMask = 1 << enemyLayer;

    public static int magicLayer = 9;
    public static int magicLayerMask = 1 << magicLayer;

    public static int towerLayer = 12;
    public static int towerLayerMask = 1 << towerLayer;

    public static int projectileLayer = 13;
    public static int projectileLayerMask = 1 << projectileLayer;

    public static int groundPathLayer = 14;
    public static int groundPathLayerMask = 1 << groundPathLayer;
}