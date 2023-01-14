using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TowerType { TreasureTower, ArrowTower, CanonTower, LaserTower, WoodTower, StoneTower, MagicTower, HealingTower, UpgradeTower, CartographyTower, GraviTower, LadderTower, RapidLaserTower };

public class Tower : MonoBehaviour, I_Interactable
{
    public TowerType type;
    public Transform topOfTower;
    public Transform entrance;
    public int woodCost;
    public int stoneCost;
    public int magicCost;
    public bool isActive = false;
    public bool validPosition = true;
    public string towerInfo = "Info";
    public Material baseMaterial;
    public Material validMaterial;
    public Material invalidMaterial;
    public MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer.material = invalidMaterial;
    }

    private void Update()
    {
        if(!isActive)
        {
            if(validPosition)
            {
                meshRenderer.material = validMaterial;
            }
            else
            {
                meshRenderer.material = invalidMaterial;
            }
        }

        SubUpdate();
    }

    /// <summary>
    /// To add update functionality to subclasses.
    /// </summary>
    protected virtual void SubUpdate() { }

    /// <summary>
    /// To add build functionality to subclasses.
    /// </summary>
    protected virtual void SubBuild() { }

    public bool IsAffordable()
    {
        return (GameManager.Singleton.wood >= woodCost && GameManager.Singleton.stone >= stoneCost && GameManager.Singleton.magic >= magicCost);
    }

    public virtual void Build()
    {
        if (!IsAffordable()) { return; }

        GameManager.Singleton.wood -= woodCost;
        GameManager.Singleton.stone -= stoneCost;
        GameManager.Singleton.magic -= magicCost;

        meshRenderer.material = baseMaterial;
        if (type == TowerType.LadderTower)
        {
            gameObject.transform.GetChild(2).gameObject.SetActive(true);
        }
        isActive = true;

        SubBuild();
    }

    public void Leave(Player player)
    {
        player.TeleportTo(entrance.position);
    }

    public virtual void Interact(Player player)
    {
        if (GameManager.Singleton.isInTower == true)
        {
            player.TeleportTo(entrance.position);
            GameManager.Singleton.isInTower = false;
            // SoundController.Singleton.playSoundeffect(SoundController.Singleton.leaveTower, GameManager.Singleton.player.audioSource);
        }
        else
        {
            player.TeleportTo(topOfTower.position);
            if (type == TowerType.LadderTower) { Debug.Log("Ladder"); return; }
            GameManager.Singleton.isInTower = true;
            // SoundController.Singleton.playSoundeffect(SoundController.Singleton.enterTower, GameManager.Singleton.player.audioSource);
        }
    }

    public string Name()
    {
        // Just return the type as string with spaces
        return string.Concat(type.ToString().Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
    }
}
