using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CarData
{
    public string id;
    public string carName;
    public float basePurchasePrice;
    public float currentValue;
    public string modelPrefabPath;
    //public List<PartData> installedParts = new List<PartData>();
    public CarCondition condition;
    public float repairCost;
    public CarType carType;

    [System.NonSerialized]
    private CarTypeData typeData;


    [System.Serializable]
    public enum CarCondition
    {
        Junk,
        Poor,
        Average,
        Good,
        Excellent
    }

    [System.Serializable]

    public enum CarType
    {
        Sedan,
        SUV,
        SportsCar,
        Luxury,
        Truck
    }

    //Methods for calculate cost for sell orr upgrade cars


    public CarTypeData GetTypeData()
    {
        if (typeData == null)
        {
            CarDatabase database = Resources.Load<CarDatabase>("CarDatabase");
            if (database != null)
            {
                typeData = database.GetCarTypeData(carType);
            }
        }
        return typeData;
    }

    public GameObject GetCarPrefab()
    {
        CarTypeData data = GetTypeData();
        return data != null ? data.carPrefab : null;
    }

    public Sprite GetCarSprite()
    {
        CarTypeData data = GetTypeData();
        return data != null ? data.carSprite : null;
    }

    public float CalculateSellingPrice()
    {
        float basePrice = currentValue;
        float contitonalMultiplier = GetConditionMultiplier();
        return RoundValue(basePrice * contitonalMultiplier);
    }

    private float RoundValue(float originalOffer)
    {
        if (originalOffer < 1000f)
        {
            return Mathf.Round(originalOffer / 10f) * 10f;
        }

        else if (originalOffer < 10000f)
        {
            return Mathf.Round(originalOffer / 100f) * 100f;
        }
        else if (originalOffer < 100000f)
        {
            return Mathf.Round(originalOffer / 1000f) * 1000f;
        }
        else
        {
            return Mathf.Round(originalOffer / 5000f) * 5000f;
        }
    }

    private float GetConditionMultiplier()
    {
        switch (condition)
        {
            case CarCondition.Junk: return 0.9f;
            case CarCondition.Poor: return 0.95f;
            case CarCondition.Average: return 1f;
            case CarCondition.Good: return 1.5f;
            case CarCondition.Excellent: return 2f;
            default: return 1f;

        }
    }

    public float GetRepairCost()
    {
        return repairCost * (5 - (int)condition) / 5f;
    }

    public bool RepairCar()
    {
        if (condition == CarCondition.Excellent)
        {
            return false;
        }

        condition = (CarCondition)((int)condition + 1);
        return true;
    }
}