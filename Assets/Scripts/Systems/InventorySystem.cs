using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : BaseSystem
{
    private InventoryData inventoryData;

    public override void Initialize()
    {
        base.Initialize();

        if (inventoryData == null)
        {
            inventoryData = new InventoryData();
        }
    }

    public bool AddCar(CarData car)
    {
        bool success = inventoryData.AddCar(car);

        if (success)
        {
            EventManager.Instance.TriggerInventoryChanged();
        }
        else
        {
        }

        return success;
    }

    public bool RemoveCar(CarData car)
    {
        bool success = inventoryData.RemoveCar(car);

        if (success)
        {
            EventManager.Instance.TriggerInventoryChanged();
        }

        return success;
    }

    public bool RepairCar(CarData car, float repairSpeedMultiplier = 1.0f)
    {
        if (!inventoryData.cars.Contains(car))
        {
            return false;
        }

        if (car.condition == CarData.CarCondition.Excellent)
        {
            return false;
        }

        float repairCost = car.GetRepairCost();

        ResourceSystem resourceSystem = GameManager.Instance.GetComponent<ResourceSystem>();
        if (!resourceSystem.CanAfford(repairCost))
        {
            Debug.LogWarning("Failed to repair car: Not enough money");
            return false;
        }

        if (resourceSystem.SpendMoney(repairCost))
        {
            car.RepairCar();

            float experienceForRepair = repairCost * 0.1f;
            resourceSystem.AddExperience(experienceForRepair);

            EventManager.Instance.TriggerInventoryChanged();

            return true;
        }

        return false;
    }

    public bool SellCar(CarData car)
    {
        if (!inventoryData.cars.Contains(car))
        {
            return false;
        }

        float sellingPrice = car.CalculateSellingPrice();

        if (RemoveCar(car))
        {
            ResourceSystem resourceSystem = GameManager.Instance.GetComponent<ResourceSystem>();
            resourceSystem.AddMoney(sellingPrice);

            float experienceForSale = sellingPrice * 0.05f;
            resourceSystem.AddExperience(experienceForSale);

            PlayerData playerData = resourceSystem.GetPlayerData();
            playerData.totalCarsSold++;

            EventManager.Instance.TriggerCarSold(car);

            return true;
        }

        return false;
    }

    public void IncreaseMaxCars(int amount)
    {
        inventoryData.IncreaseMaxCars(amount);

        EventManager.Instance.TriggerInventoryChanged();
    }

    public InventoryData GetInventoryData()
    {
        return inventoryData;
    }

    public CarData GetCarById(string id)
    {
        return inventoryData.cars.Find(car => car.id == id);
    }

    public void SetMaxCars(int count)
    {
        inventoryData.maxCars = count;

        EventManager.Instance.TriggerInventoryChanged();
    }

    public void SetInventoryData(InventoryData data)
    {
        inventoryData = data;

        EventManager.Instance.TriggerInventoryChanged();
    }

    public List<CarData> GetCars()
    {
        return inventoryData.cars;
    }

    public int GetCarCount()
    {
        return inventoryData.cars.Count;
    }

    public int GetMaxCars()
    {
        return inventoryData.maxCars;
    }

    public int GetFreeCarsSpace()
    {
        return inventoryData.GetFreeCarsSpace();
    }
}