using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using HudElements;

public class Player : MonoBehaviour
{
    public int currentHealth = 100;
    public int maxHealth = 100;
    [Range(0, 1)]
    public float healthPercent = 1;
    public Weapon weapon;
    private float lastShotTime = 0;
    GameObject currentlyBuilding;
    LayerMask towerLayer;
    float minTowerDistance = 15;
    public float interactRange = 15f;
    private I_Interactable interactable = null;
    public Transform firePoint;
    [SerializeField] public AudioSource audioSource;


    private void OnEnable()
    {
        currentHealth = maxHealth;
        towerLayer = LayerMask.GetMask("Tower");
        weapon.range = interactRange;
    }

    private void FixedUpdate()
    {
        // Check for interactable gameobjects
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactRange, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            interactable = hit.transform.GetComponent<I_Interactable>();
            if (interactable != null) UI.Singleton.interactDisplay.text = interactable.Name();
            else UI.Singleton.interactDisplay.text = "";
        } else if (interactable != null)
        {
            interactable = null;
            UI.Singleton.interactDisplay.text = "";
        }

        if (currentlyBuilding != null)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, MyLayer.groundLayerMask + MyLayer.groundPathLayerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.normal == Vector3.up && hit.collider.gameObject.layer == MyLayer.groundLayer)
                {
                    currentlyBuilding.transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                    Vector3 bottomPoint = hit.point;
                    currentlyBuilding.transform.position = bottomPoint;

                    Tower tower = currentlyBuilding.gameObject.GetComponent(typeof(Tower)) as Tower;
                    Vector3 topPoint = bottomPoint + new Vector3(0, tower.topOfTower.position.y - tower.entrance.position.y, 0);
                    float radius = (tower.entrance.position.x - tower.topOfTower.position.x);

                    // Check if there is space to build
                    if (Physics.OverlapCapsule(bottomPoint, topPoint, radius, ~(MyLayer.groundLayerMask + MyLayer.towerLayerMask), QueryTriggerInteraction.Ignore).Length > 0)
                    {
                        tower.validPosition = false;
                    }
                    else
                    {
                        // Check for min tower distance
                        if (Physics.OverlapSphere(bottomPoint, minTowerDistance, towerLayer, QueryTriggerInteraction.Ignore).Length > 1)
                        {
                            tower.validPosition = false;
                            
                        }
                        else
                        {
                            tower.validPosition = true;
                        }
                    }
                    
                }
            }
        }
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Debug.LogError("Player died.");
        }
    }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            OnFire();
        }
    }

    public void TeleportTo(Vector3 newPosition)
    {
        //GameManager.Singleton.isInTower = true;
        GetComponent<CharacterController>().enabled = false;
        transform.position = newPosition;
        GetComponent<CharacterController>().enabled = true;
    }

    /// <summary>
    /// The input action Fire.
    /// </summary>
    public void OnFire()
    {
        if(GameManager.Singleton.isMenuOpen) return;

        if(currentlyBuilding != null)
        {
            // Place tower
            FinishBuilding();
        } else if (weapon != null && (Time.time - lastShotTime) > 1f / weapon.fireRate)
        {
            // Shoot weapon
            weapon.Shoot(firePoint,Camera.main.transform);
            lastShotTime = Time.time;

            // Damage resources (only the player damages/mines resources)
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactRange, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                MapResource resource = hit.collider.GetComponent<MapResource>();
                if (resource != null)
                {
                    resource.Damage(1);
                    SoundController.Singleton.playSoundeffect(SoundController.Singleton.woodHit, audioSource);
                }
            }
        }
    }

    public void OnSkipPause()
    {
        GameManager.Singleton.SkipPause();
    }

    public void OnToggleBuildingMenu()
    {
        Debug.Log("Open building Menu");
        UI.Singleton.ToggleBuildingMenu();

        if(currentlyBuilding != null)
        {
            AbortBuilding();
        }
    }

    /// <summary>
    /// The input action Interact.
    /// </summary>
    public void OnInteract()
    {
        Debug.Log("TRY INTERACT");
        if (interactable != null)
        {
            Debug.Log("INTERACT");
            interactable.Interact(this);
        }
    }

    public void OnPause()
    {
        Debug.Log("Paused");
        if(UI.Singleton.gameOverScreenOpen) return;
        if(UI.Singleton.buildingMenuOpen)
        {
            UI.Singleton.ToggleBuildingMenu();
        }
        else
        {
            if(UI.Singleton.pauseMenuOpen)
            {
                UI.Singleton.pauseMenu.gameObject.SetActive(false);
            }
            else{
                UI.Singleton.pauseMenu.gameObject.SetActive(true);
            }
        }
    }

    public void BuildTower(GameObject towerPrefab)
    {
        if (currentlyBuilding == null)
        {
            currentlyBuilding = Instantiate(towerPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    public void FinishBuilding()
    {
        Tower tower = currentlyBuilding.gameObject.GetComponent(typeof(Tower)) as Tower;
        if (tower.IsAffordable() && tower.validPosition)
        {
            tower.Build();
            currentlyBuilding = null;
            SoundController.Singleton.playSoundeffect(SoundController.Singleton.buildTower, audioSource);
        }
    }

    public void AbortBuilding()
    {
        Destroy(currentlyBuilding);
        currentlyBuilding = null;
    }
}