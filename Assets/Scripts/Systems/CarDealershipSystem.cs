using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDealershipSystem : BaseSystem
{
    [SerializeField] private float carShopRefreshInterval = 120f; 
    private float lastCarShopRefreshTime;

    [SerializeField] private List<CarData> carTemplates = new List<CarData>();
    private List<CarData> availableCarsForPurchase = new List<CarData>();
    [SerializeField] private CarDatabase carDatabase;

    [SerializeField] private int maxCarsInShop = 5;

    private bool isInitialized = false;

    public override void Initialize()
    {
        base.Initialize();

        lastCarShopRefreshTime = Time.time;

        StartCoroutine(DelayedInitialization());
    }

    private IEnumerator DelayedInitialization()
    {
        yield return null;

        if (carTemplates.Count == 0)
        {
            InitializeCarTemplates();
        }

        yield return null;

        if (GameManager.Instance != null && EventManager.Instance != null)
        {
            try
            {
                RefreshCarShop();
                isInitialized = true;
            }
            catch (System.Exception e)
            {
                StartCoroutine(RetryInitialization());
            }
        }
        else
        {
            StartCoroutine(RetryInitialization());
        }
    }

    private IEnumerator RetryInitialization()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DelayedInitialization());
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        if (Time.time - lastCarShopRefreshTime > carShopRefreshInterval)
        {
            RefreshCarShop();
            lastCarShopRefreshTime = Time.time;
        }
    }

    private void InitializeCarTemplates()
    {
        if (carDatabase == null)
        {
            carDatabase = Resources.Load<CarDatabase>("CarDatabase");
            if (carDatabase == null)
            {
                Debug.LogError("Car database not found!");
                return;
            }
        }

        carTemplates.Clear();
        foreach (var carTypeData in carDatabase.carTypes)
        {
            CarData template = new CarData();
            template.carType = carTypeData.carType;
            template.basePurchasePrice = carTypeData.basePurchasePrice;
            template.repairCost = carTypeData.repairCost;
            carTemplates.Add(template);
        }
    }

    public void RefreshCarShop()
    {
        try
        {
            if (carTemplates == null || carTemplates.Count == 0)
            {
                InitializeCarTemplates();

                if (carTemplates == null || carTemplates.Count == 0)
                {
                    return;
                }
            }

            if (availableCarsForPurchase == null)
                availableCarsForPurchase = new List<CarData>();
            else
                availableCarsForPurchase.Clear();

            for (int i = 0; i < maxCarsInShop; i++)
            {
                CarData newCar = GenerateCarForShop();
                if (newCar != null)
                    availableCarsForPurchase.Add(newCar);
            }

            if (EventManager.Instance != null)
                EventManager.Instance.TriggerCarShopRefreshed();
            else
                Debug.LogWarning("EventManager is not available");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error refreshing car shop: " + e.Message);
        }
    }

    private CarData GenerateCarForShop()
    {
        try
        {


            int templateIndex = Random.Range(0, carTemplates.Count);
            CarData template = carTemplates[templateIndex];

            CarData newCar = new CarData();
            newCar.id = System.Guid.NewGuid().ToString();
            newCar.carType = template.carType;
            newCar.repairCost = template.repairCost;

            string[] carBrands = { "Audi", "BMW", "Ford", "Honda", "Toyota", "Nissan", "Kia", "Hyundai" };
            string[] carModels = { "RS1", "M5", "Mustang", "Civic", "Camry", "Skyline R34", "K5", "Sonata" };

            int brandIndex = Random.Range(0, carBrands.Length);
            int modelIndex = Random.Range(0, carModels.Length);
            newCar.carName = carBrands[brandIndex] + " " + carModels[modelIndex];

            float conditionRandom = Random.Range(0f, 1f);
            if (conditionRandom < 0.4f)
                newCar.condition = CarData.CarCondition.Junk;
            else if (conditionRandom < 0.7f)
                newCar.condition = CarData.CarCondition.Poor;
            else
                newCar.condition = CarData.CarCondition.Average;

            float priceMultiplier = UnityEngine.Random.Range(0.8f, 1.2f);

            float levelPriceAdjustment = 1.0f;

            if (GameManager.Instance != null)
            {
                ResourceSystem resourceSystem = GameManager.Instance.GetComponent<ResourceSystem>();
                if (resourceSystem != null)
                {
                    int playerLevel = resourceSystem.GetLevel();
                    levelPriceAdjustment = 1.0f + (playerLevel - 1) * 0.1f;
                }
            }

            newCar.basePurchasePrice = RoundValue(template.basePurchasePrice * priceMultiplier * levelPriceAdjustment);
            newCar.currentValue = newCar.basePurchasePrice;

            return newCar;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error generating car: " + e.Message);
            return null;
        }
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

    public bool PurchaseCarFromShop(CarData car)
    {
        if (!availableCarsForPurchase.Contains(car))
        {
            Debug.LogWarning("Car is not available for purchase in the shop");
            return false;
        }

        ResourceSystem resourceSystem = GameManager.Instance.GetComponent<ResourceSystem>();
        if (!resourceSystem.CanAfford(car.basePurchasePrice))
        {
            Debug.LogWarning("Not enough money to purchase car");
            return false;
        }

        InventorySystem inventorySystem = GameManager.Instance.GetComponent<InventorySystem>();
        if (inventorySystem.GetFreeCarsSpace() <= 0)
        {
            Debug.LogWarning("Not enough space in inventory for car");
            return false;
        }

        if (resourceSystem.SpendMoney(car.basePurchasePrice) && inventorySystem.AddCar(car))
        {
            availableCarsForPurchase.Remove(car);

            BuildSystem buildSystem = GameManager.Instance.GetComponent<BuildSystem>();
            buildSystem.UpdateParkedCars();

            EventManager.Instance.TriggerCarPurchased(car);

            return true;
        }

        return false;
    }

    public float GetTimeUntilNextShopRefresh()
    {
        float timeSinceLastRefresh = Time.time - lastCarShopRefreshTime;
        return Mathf.Max(0, carShopRefreshInterval - timeSinceLastRefresh);
    }

    public void ResetShopRefreshTimer()
    {
        lastCarShopRefreshTime = Time.time;
    }

    public List<CarData> GetAvailableCarsForPurchase()
    {
        return availableCarsForPurchase;
    }
}