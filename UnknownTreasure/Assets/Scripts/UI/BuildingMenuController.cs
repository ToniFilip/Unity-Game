using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace HudElements
{
    public class BuildingMenuController : MonoBehaviour
    {
        public List<Tower> buildingTowers;
        public UIDocument buildingMenu;

        VisualTreeAsset itemTemplate;

        void OnEnable()
        {
            UI.Singleton.buildingMenuOpen = true;
            var root = buildingMenu.rootVisualElement;
            // Button
            Button exitButton = root.Q<Button>("exit");
            exitButton.text = "X";
            exitButton.clicked += () => UI.Singleton.ToggleBuildingMenu();

            // Elements
            ScrollView scrollMenu = root.Q<ScrollView>("scrollMenu");
            VisualElement listElement = root.Q<VisualElement>("menuContainer");
            foreach (Tower tower in buildingTowers) {
                // Load BMElement
                VisualTreeAsset vt = Resources.Load<VisualTreeAsset>("UI Documents/BMElement");
                VisualElement bmElement = vt.Instantiate();

                // Label
                Label nameLabel = bmElement.Q<Label>("towerName");
                nameLabel.text = (tower as I_Interactable).Name();

                // Button
                Button bmElementButton = bmElement.Q<Button>("bmelementButton");
                bmElementButton.clicked += () => ElementClicked(tower);

                // Button Hover
                Color colorUnselected = new Color((float) 47/255, (float) 54/255, (float) 64/255);
                Color colorSelected = new Color((float) 127/255, (float) 143/255, (float) 166/255);
                VisualElement background = bmElement.Q<VisualElement>("background");
                bmElementButton.RegisterCallback<MouseOverEvent>((type) =>
                {
                    background.style.borderLeftColor = colorSelected;
                    background.style.borderRightColor = colorSelected;
                    background.style.borderTopColor = colorSelected;
                    background.style.borderBottomColor = colorSelected;
                });
                bmElementButton.RegisterCallback<MouseOutEvent>((type) =>
                {
                    background.style.borderLeftColor = colorUnselected;
                    background.style.borderRightColor = colorUnselected;
                    background.style.borderTopColor = colorUnselected;
                    background.style.borderBottomColor = colorUnselected;
                });

                // ResourceCost Icons
                VisualElement towerCosts = bmElement.Q<VisualElement>("towerCosts");

                ResourceCount iconElement = towerCosts.Q<ResourceCount>("woodCosts");
                VisualElement icon = iconElement.Q<VisualElement>("img");
                icon.style.backgroundColor = new Color(0, 0, 0, 0);
                icon.style.backgroundImage = UI.Singleton.woodImg;
                /*icon.style.width = 80;
                icon.style.height = 80;*/

                iconElement = towerCosts.Q<ResourceCount>("stoneCosts");
                icon = iconElement.Q<VisualElement>("img");
                icon.style.backgroundColor = new Color(0, 0, 0, 0);
                icon.style.backgroundImage = UI.Singleton.stoneImg;
                /*icon.style.width = 80;
                icon.style.height = 80;*/

                iconElement = towerCosts.Q<ResourceCount>("magicCosts");
                icon = iconElement.Q<VisualElement>("img");
                icon.style.backgroundColor = new Color(0, 0, 0, 0);
                icon.style.backgroundImage = UI.Singleton.magicImg;
                /*icon.style.width = 80;
                icon.style.height = 80;*/

                bmElement.name = tower.type.ToString();
                scrollMenu.Add(bmElement);

                // Tower Info
                Label towerInfo = bmElement.Q<Label>("towerInfo");
                towerInfo.text = tower.towerInfo;

                // Tower stats
                Label attributesLabel = bmElement.Q<Label>("attributes");
                Label statsLabel = bmElement.Q<Label>("stats");
                Label buffsLabel = bmElement.Q<Label>("buffs");

                if(tower.GetType() == typeof(MilitaryTower))
                {
                    attributesLabel.text = "\nRange\nDamage\nFirerate";
                    statsLabel.text = "Base\n" + ((MilitaryTower) tower).weapon.baseRange.ToString() + "\n" + ((MilitaryTower)tower).weapon.baseDamage.ToString() + "\n" + ((MilitaryTower)tower).weapon.baseFireRate.ToString();
                    buffsLabel.text = "Buff\n+" + ((MilitaryTower)tower).rangeBuff.ToString() + "\n+" + ((MilitaryTower)tower).damageBuff.ToString() + "\n+" + ((MilitaryTower)tower).fireRateBuff.ToString();
                }
                else
                {
                    // attributesLabel.text = "Rate";
                    attributesLabel.text = "";
                    statsLabel.text = "";
                    buffsLabel.text = "";
                }

                // Tower image
                VisualElement towerImage = bmElement.Q<VisualElement>("towerImage");
                // towerImage.style.width = 0;
                Material towerMaterial = tower.baseMaterial;
                towerImage.style.backgroundColor = towerMaterial.GetColor("_Color");

                // Scrollbar
                float ScrollFactor = 50;
  
                scrollMenu.RegisterCallback<WheelEvent>((evt) =>
                    {
                        scrollMenu.scrollOffset = new Vector2(0, scrollMenu.scrollOffset.y + ScrollFactor * evt.delta.y);
                        evt.StopPropagation();
                    }
                );
            }
        }

        // Update is called once per frame
        void Update()
        {
            var root = buildingMenu.rootVisualElement;

            foreach (Tower tower in buildingTowers) {
                // Costs
                VisualElement towerElement = root.Q<VisualElement>(tower.type.ToString());
                ResourceCount woodCostsElement = towerElement.Q<ResourceCount>("woodCosts");
                Label woodCostsLabel = woodCostsElement.Q<Label>("label");
                ResourceCount stoneCostsElement = towerElement.Q<ResourceCount>("stoneCosts");
                Label stoneCostsLabel = stoneCostsElement.Q<Label>("label");
                ResourceCount magicCostsElement = towerElement.Q<ResourceCount>("magicCosts");
                Label magicCostsLabel = magicCostsElement.Q<Label>("label");
                woodCostsLabel.text = tower.woodCost.ToString("D" + 3);
                stoneCostsLabel.text = tower.stoneCost.ToString("D" + 3);
                magicCostsLabel.text = tower.magicCost.ToString("D" + 3);

                Label labelAffordable = towerElement.Q<Label>("expensiveNotifier");

                if(!tower.IsAffordable())
                {
                    labelAffordable.text = "Tower too expensive.";
                }
                else
                {
                    labelAffordable.text = "";
                }
            }
        }

        void ElementClicked(Tower tower)
        {
            GameObject towerPrefab = tower.gameObject;
            if(tower.IsAffordable())
            {
                SoundController.Singleton.playSoundeffect(SoundController.Singleton.uiClick, SoundController.Singleton.controllerAudioSource);
                UI.Singleton.ToggleBuildingMenu();
                GameManager.Singleton.player.BuildTower(towerPrefab);
            }
            else {
                SoundController.Singleton.playSoundeffect(SoundController.Singleton.denyBuild, SoundController.Singleton.controllerAudioSource, 0.3f);
                Debug.Log("Cannot Build Tower " + (tower as I_Interactable).Name());
            }
        }

        void OnDisable()
        {
            UI.Singleton.buildingMenuOpen = false;
        }
    }
}
