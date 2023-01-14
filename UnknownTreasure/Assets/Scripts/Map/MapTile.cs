using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapTile : MonoBehaviour
{
    public MapTileType type;

    public bool N;
    public bool E;
    public bool S;
    public bool W;

    #region Map Generation
    public bool wasVisited = false;
    public float costs;
    public float f;
    public float g;

    public MapTile predecessor;
    public MapTile successor;

    public int x;
    public int y;

    public MapTile target;
    public CardinalPoint cardinalPoint;
    #endregion

    public void Rotation(int rotations)
    {
        transform.localRotation = Quaternion.Euler(0, 90 * rotations, 0);

        for (int k = 0; k < rotations; k++)
        {
            
            bool _N = N;
            N = W;
            W = S;
            S = E;
            E = _N;
            
        }
    }
}

[System.Serializable]
public enum MapTileType { I, L, T, X, EMPTY, SPAWN, BASE }

[System.Serializable]
public enum CardinalPoint { North, East, South, West }