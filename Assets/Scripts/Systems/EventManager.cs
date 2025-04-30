using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    public event Action<CarData> OnCarPurchased;
    public event Action<CarData> OnCarSold;
    public event Action<CarData> OnCarRepaired;
    public event Action<float> OnMoneyChanged;
    public event Action<int> OnLevelUp;
    public event Action OnInventoryChanged;
    public event Action OnCarShopRefreshed;
    public event Action<CustomerData> OnCustomerArrived;
    public event Action<CustomerData, bool> OnCustomerLeft; 
    public event Action<BuildingData> OnBuildingSelected;
    public event Action<BuildingData> OnBuildingConstructed;
    public event Action<BuildingData> OnBuildingUpgraded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TriggerCarPurchased(CarData car)
    {
        OnCarPurchased?.Invoke(car);
    }

    public void TriggerCarSold(CarData car)
    {
        OnCarSold?.Invoke(car);
    }

    public void TriggerCarRepaired(CarData car)
    {
        OnCarRepaired?.Invoke(car);
    }

    public void TriggerMoneyChanged(float amount)
    {
        OnMoneyChanged?.Invoke(amount);
    }

    public void TriggerLevelUp(int newLevel)
    {
        OnLevelUp?.Invoke(newLevel);
    }

    public void TriggerInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    public void TriggerCarShopRefreshed()
    {
        OnCarShopRefreshed?.Invoke();
    }

    public void TriggerCustomerArrived(CustomerData customer)
    {
        OnCustomerArrived?.Invoke(customer);
    }

    public void TriggerCustomerLeft(CustomerData customer, bool purchased)
    {
        OnCustomerLeft?.Invoke(customer, purchased);
    }

    public void TriggerBuildingSelected(BuildingData building)
    {
        OnBuildingSelected?.Invoke(building);
    }

    public void TriggerBuildingConstructed(BuildingData building)
    {
        OnBuildingConstructed?.Invoke(building);
    }

    public void TriggerBuildingUpgraded(BuildingData building)
    {
        OnBuildingUpgraded?.Invoke(building);
    }
}