using System;
using UnityEngine;

[System.Serializable]
public class BuildingData
{
    public string id;
    public string buildingName = "Car Slot";
    public int level = 1;
    public float constructionCost = 2500f; 
    public float upgradeCost = 2500f;
    public bool isConstructed = false;
    public Vector3 position;
    public Quaternion rotation;


    public float GetCost()
    {
        if (!isConstructed)
        {
            return constructionCost;
        }
        else
        {
            return CalculateUpgradeCost();
        }
    }

    public float CalculateUpgradeCost()
    {
        return upgradeCost * (float)Math.Pow(1.5f, level - 1);
    }

    public void Construct()
    {
        isConstructed = true;
    }

    public void Upgrade()
    {
        level++;
    }


    public float GetRepairSpeedBonus()
    {
        return 1.0f + (level - 1) * 0.1f;
    }
}