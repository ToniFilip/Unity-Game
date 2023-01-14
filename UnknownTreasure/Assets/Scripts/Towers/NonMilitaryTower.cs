using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonMilitaryTower : Tower
{
    public float woodPerSecond = 0f;
    public float stonePerSecond = 0f;
    public float magicPerSecond = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if(woodPerSecond != 0)
        {
            InvokeRepeating("GenerateWood", 0, (1.0f/woodPerSecond));
        }
        if(stonePerSecond != 0)
        {
            InvokeRepeating("GenerateStone", 0, (1.0f/stonePerSecond));
        }
        if(magicPerSecond != 0)
        {
            InvokeRepeating("GenerateMagic", 0, (1.0f/magicPerSecond));
        }
        
    }

    void GenerateWood()
    {
        if (isActive && GameManager.Singleton.CurrentPauseTime <= 0f)
        {
            if(GameManager.Singleton.isInTower)
            {
                GameManager.Singleton.wood += 2;
            }
            else
            {
                GameManager.Singleton.wood += 1;
            }
            
        }
    }

    void GenerateStone()
    {
        if (isActive && GameManager.Singleton.CurrentPauseTime <= 0f)
        {
            if (GameManager.Singleton.isInTower)
            {
                GameManager.Singleton.stone += 2;
            }
            else
            {
                GameManager.Singleton.stone += 1;
            }
        }
    }

    void GenerateMagic()
    {
        if (isActive && GameManager.Singleton.CurrentPauseTime <= 0f)
        {
            if (GameManager.Singleton.isInTower)
            {
                GameManager.Singleton.magic += 2;
            }
            else
            {
                GameManager.Singleton.magic += 1;
            }
        }
    }
}
