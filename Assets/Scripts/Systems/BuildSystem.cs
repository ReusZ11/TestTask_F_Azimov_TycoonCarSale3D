using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : BaseSystem
{
    [SerializeField] private GameObject carSlotPrefab; 
    [SerializeField] private GameObject defaultSlotPrefab;

    private BuildingData parkingLotData = new BuildingData();

    private List<GameObject> carSlotObjects = new List<GameObject>();

    private List<GameObject> defaultSlotObjects = new List<GameObject>();

    [SerializeField] private int maxParkingLevel = 10;

    [SerializeField] private List<Transform> carSlotPositions = new List<Transform>();

    [SerializeField] private List<Transform> defaultSlotPositions = new List<Transform>();

    private Dictionary<string, GameObject> parkedCarObjects = new Dictionary<string, GameObject>();

    [SerializeField] private int defaultParkingSlots = 2;


    public override void Initialize()
    {
        base.Initialize();

        if (carSlotPositions.Count == 0)
        {
            Debug.LogError("No car slot positions defined. Please add transforms to carSlotPositions array");
            return;
        }

        if (defaultSlotPositions.Count < defaultParkingSlots)
        {
            Debug.LogError("Not enough default parking slot positions defined");
            return;
        }

        if (parkingLotData == null)
        {
            parkingLotData = new BuildingData();
            parkingLotData.id = "ParkingLot";
            parkingLotData.buildingName = "Parking Lot";
            parkingLotData.isConstructed = false;
            parkingLotData.level = 0;
            parkingLotData.constructionCost = 5000f;
            parkingLotData.upgradeCost = 2500f;
        }

        CreateDefaultParkingSlots();

        UpdateParkingVisuals();

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        EventManager.Instance.OnCarPurchased += OnInventoryChanged;
        EventManager.Instance.OnCarSold += OnInventoryChanged;
        EventManager.Instance.OnInventoryChanged += UpdateParkedCars;
    }

    private void OnInventoryChanged(CarData car = null)
    {
        UpdateParkedCars();
    }

    private void CreateDefaultParkingSlots()
    {
        foreach (var obj in defaultSlotObjects)
        {
            Destroy(obj);
        }
        defaultSlotObjects.Clear();

        for (int i = 0; i < defaultParkingSlots; i++)
        {
            if (i < defaultSlotPositions.Count)
            {
                GameObject slotObject = Instantiate(defaultSlotPrefab, defaultSlotPositions[i].position, defaultSlotPositions[i].rotation);
                slotObject.name = "DefaultSlot_" + i;
                defaultSlotObjects.Add(slotObject);
            }
        }

        InventorySystem inventorySystem = GameManager.Instance.GetComponent<InventorySystem>();
        if (inventorySystem != null)
        {
            inventorySystem.SetMaxCars(defaultParkingSlots + (parkingLotData.isConstructed ? parkingLotData.level : 0));
        }
    }



    private void UpdateParkingVisuals()
    {
        foreach (var obj in carSlotObjects)
        {
            Destroy(obj);
        }
        carSlotObjects.Clear();

        if (!parkingLotData.isConstructed)
        {
            return;
        }

        int slotsToShow = Mathf.Min(parkingLotData.level, carSlotPositions.Count);

        for (int i = 0; i < slotsToShow; i++)
        {
            GameObject slotObject = Instantiate(carSlotPrefab, carSlotPositions[i].position, carSlotPositions[i].rotation);
            slotObject.name = "CarSlot_" + i;

            carSlotObjects.Add(slotObject);
        }

        UpdateParkedCars();
    }

    public bool ConstructBuilding()
    {
        if (parkingLotData.isConstructed)
        {
            Debug.LogWarning("Parking uzhe est");
            return false;
        }

        ResourceSystem resourceSystem = GameManager.Instance.GetComponent<ResourceSystem>();
        if (!resourceSystem.CanAfford(parkingLotData.constructionCost))
        {
            Debug.LogWarning("net denyag");
            return false;
        }


        if (resourceSystem.SpendMoney(parkingLotData.constructionCost))
        {
            parkingLotData.isConstructed = true;
            parkingLotData.level = 1; 

            InventorySystem inventorySystem = GameManager.Instance.GetComponent<InventorySystem>();
            inventorySystem.IncreaseMaxCars(1);

            UpdateParkingVisuals();

            EventManager.Instance.TriggerBuildingConstructed(parkingLotData);

            return true;
        }

        return false;
    }

    public bool UpgradeBuilding()
    {
        if (!parkingLotData.isConstructed)
        {
            return false;
        }

        if (parkingLotData.level >= maxParkingLevel || parkingLotData.level >= carSlotPositions.Count)
        {
            Debug.LogWarning("Parking is max level");
            return false;
        }

        ResourceSystem resourceSystem = GameManager.Instance.GetComponent<ResourceSystem>();
        int playerLevel = resourceSystem.GetLevel();

        int nextParkingLevel = parkingLotData.level + 1;
        if (playerLevel < nextParkingLevel)
        {
            return false;
        }

        float upgradeCost = parkingLotData.CalculateUpgradeCost();

        if (!resourceSystem.CanAfford(upgradeCost))
        {
            Debug.LogWarning("Net denyag");
            return false;
        }

        if (resourceSystem.SpendMoney(upgradeCost))
        {
            parkingLotData.level++;

            InventorySystem inventorySystem = GameManager.Instance.GetComponent<InventorySystem>();
            inventorySystem.IncreaseMaxCars(1);

            UpdateParkingVisuals();

            EventManager.Instance.TriggerBuildingUpgraded(parkingLotData);

            return true;
        }

        return false;
    }

    public void UpdateParkedCars()
    {
        foreach (var carObj in parkedCarObjects.Values)
        {
            Destroy(carObj);
        }
        parkedCarObjects.Clear();

        InventorySystem inventorySystem = GameManager.Instance.GetComponent<InventorySystem>();
        List<CarData> cars = inventorySystem.GetCars();

        int carIndex = 0;
        foreach (var slot in defaultSlotObjects)
        {
            if (carIndex < cars.Count)
            {
                SpawnCarOnSlot(cars[carIndex], slot);
                carIndex++;
            }
        }

        foreach (var slot in carSlotObjects)
        {
            if (carIndex < cars.Count)
            {
                SpawnCarOnSlot(cars[carIndex], slot);
                carIndex++;
            }
        }
    }

    private void SpawnCarOnSlot(CarData car, GameObject slotObject)
    {
        GameObject carPrefab = car.GetCarPrefab();
        if (carPrefab == null)
        {
            return;
        }

        Transform carPosition = slotObject.transform.Find("CarPosition");
        Vector3 position = carPosition != null ? carPosition.position : slotObject.transform.position;
        Quaternion rotation = carPosition != null ? carPosition.rotation : slotObject.transform.rotation;

        GameObject carObject = Instantiate(carPrefab, position, rotation);
        carObject.name = "Car_" + car.id;

        parkedCarObjects[car.id] = carObject;

        CarVisualizer carVisualizer = carObject.GetComponent<CarVisualizer>();
        if (carVisualizer != null)
        {
            carVisualizer.UpdateVisuals(car);
        }
    }

    public BuildingData GetParkingData()
    {
        return parkingLotData;
    }

    public void SetParkingData(BuildingData data)
    {
        parkingLotData = data;
        UpdateParkingVisuals();
    }

    public int GetCurrentMaxCars()
    {
        return defaultParkingSlots + (parkingLotData.isConstructed ? parkingLotData.level : 0);
    }

    public int GetBuiltParkingSlots()
    {
        return parkingLotData.isConstructed ? parkingLotData.level : 0;
    }
}