using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private TextMeshProUGUI buildingLevelText;
    [SerializeField] private TextMeshProUGUI buildingCostText;
    [SerializeField] private TextMeshProUGUI buildingRequirementsText;
    //[SerializeField] private TextMeshProUGUI buildingDescriptionText;
    [SerializeField] private Button buildOrUpgradeButton;

    private BuildingData buildingData;

    public event Action<BuildingData> OnBuildOrUpgrade;

    public void SetBuildingData(BuildingData building)
    {
        buildingData = building;

        if (buildingNameText != null)
        {
            buildingNameText.text = building.buildingName;
        }

        if (buildingLevelText != null)
        {
            if (building.isConstructed)
            {
                buildingLevelText.text = "Building Level: " + building.level;
            }
            else
            {
                buildingLevelText.text = "Not built yet";
            }
        }

        if (buildingCostText != null)
        {
            float cost = building.isConstructed ? building.CalculateUpgradeCost() : building.constructionCost;
            buildingCostText.text = "Cost: $" + cost.ToString("N0");
        }

        if (buildingRequirementsText != null)
        {
            if (building.isConstructed)
            {
                int nextLevel = building.level + 1;
                ResourceSystem resourceSystem = GameManager.Instance.GetComponent<ResourceSystem>();
                int playerLevel = resourceSystem.GetLevel();

                if (playerLevel < nextLevel)
                {
                    buildingRequirementsText.text = $"Required player level: {nextLevel} (Your level: {playerLevel})";
                    buildingRequirementsText.color = Color.red;
                }
                else
                {
                    buildingRequirementsText.gameObject.SetActive(false);
                }

                buildingRequirementsText.gameObject.SetActive(true);
            }
            else
            {
                buildingRequirementsText.gameObject.SetActive(false);
            }
        }



        if (buildOrUpgradeButton != null)
        {
            TextMeshProUGUI buttonText = buildOrUpgradeButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = building.isConstructed ? "Upgrade" : "Build";
            }

            buildOrUpgradeButton.onClick.RemoveAllListeners();
            buildOrUpgradeButton.onClick.AddListener(() => OnBuildOrUpgrade?.Invoke(building));

            ResourceSystem resourceSystem = GameManager.Instance.GetComponent<ResourceSystem>();
            float cost = building.isConstructed ? building.CalculateUpgradeCost() : building.constructionCost;
            bool canAfford = resourceSystem.CanAfford(cost);

            bool meetsLevelRequirement = true;
            if (building.isConstructed)
            {
                int nextLevel = building.level + 1;
                int playerLevel = resourceSystem.GetLevel();
                meetsLevelRequirement = playerLevel >= nextLevel;
            }

            buildOrUpgradeButton.interactable = canAfford && meetsLevelRequirement;
        }
    }
}