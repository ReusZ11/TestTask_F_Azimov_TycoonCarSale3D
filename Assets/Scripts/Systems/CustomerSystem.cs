using System.Collections.Generic;
using UnityEngine;

public class CustomerSystem : BaseSystem
{
    [SerializeField] private float customerGenerationInterval = 180f; 
    private float lastCustomerGenerationTime;

    [SerializeField] private List<CustomerData> customerTemplates = new List<CustomerData>();
    private CustomerData currentCustomer;

    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform customerSpawnPoint;
    [SerializeField] private Transform customerDestinationPoint;
    private GameObject activeCustomerObject;

    private Queue<CustomerData> customerQueue = new Queue<CustomerData>();

    [SerializeField] private int maxQueueLength = 3;

    private bool isProcessingCustomer = false;

    public override void Initialize()
    {
        base.Initialize();

        lastCustomerGenerationTime = Time.time;

        if (customerTemplates.Count == 0)
        {
            InitializeCustomerTemplates();
        }
    }

    private void Update()
    {
        if (Time.time - lastCustomerGenerationTime > customerGenerationInterval)
        {
            GenerateCustomer();
            lastCustomerGenerationTime = Time.time;
        }
    }

    private void InitializeCustomerTemplates()
    {
        CustomerData regularCustomer = new CustomerData();
        regularCustomer.customerType = CustomerData.CustomerType.Regular;
        regularCustomer.budgetMultiplier = 0.95f;
        regularCustomer.preferredCarTypes = new List<CarData.CarType> { CarData.CarType.Sedan };
        regularCustomer.minAcceptableCondition = CarData.CarCondition.Poor;
        customerTemplates.Add(regularCustomer);

        CustomerData businessCustomer = new CustomerData();
        businessCustomer.customerType = CustomerData.CustomerType.Business;
        businessCustomer.budgetMultiplier = 1.2f;
        businessCustomer.preferredCarTypes = new List<CarData.CarType> { CarData.CarType.Sedan, CarData.CarType.Luxury };
        businessCustomer.minAcceptableCondition = CarData.CarCondition.Average;
        customerTemplates.Add(businessCustomer);

        CustomerData familyCustomer = new CustomerData();
        familyCustomer.customerType = CustomerData.CustomerType.Family;
        familyCustomer.budgetMultiplier = 1f;
        familyCustomer.preferredCarTypes = new List<CarData.CarType> { CarData.CarType.Sedan, CarData.CarType.SUV };
        familyCustomer.minAcceptableCondition = CarData.CarCondition.Average;
        customerTemplates.Add(familyCustomer);

        CustomerData sportsCustomer = new CustomerData();
        sportsCustomer.customerType = CustomerData.CustomerType.Sports;
        sportsCustomer.budgetMultiplier = 1.15f;
        sportsCustomer.preferredCarTypes = new List<CarData.CarType> { CarData.CarType.SportsCar };
        sportsCustomer.minAcceptableCondition = CarData.CarCondition.Good;
        customerTemplates.Add(sportsCustomer);

        CustomerData wealthyCustomer = new CustomerData();
        wealthyCustomer.customerType = CustomerData.CustomerType.Wealthy;
        wealthyCustomer.budgetMultiplier = 1.25f;
        wealthyCustomer.preferredCarTypes = new List<CarData.CarType> { CarData.CarType.Luxury, CarData.CarType.SportsCar };
        wealthyCustomer.minAcceptableCondition = CarData.CarCondition.Good;
        customerTemplates.Add(wealthyCustomer);
    }

    private void GenerateCustomer()
    {
        if (customerQueue.Count >= maxQueueLength)
        {
            return;
        }

        if (isProcessingCustomer && customerQueue.Count > 0)
        {
            return;
        }

        InventorySystem inventorySystem = GameManager.Instance.GetComponent<InventorySystem>();
        if (inventorySystem.GetCarCount() == 0)
        {
            return;
        }

        CarData carToSell = SelectRandomCarFromInventory(inventorySystem);
        if (carToSell == null)
        {
            return;
        }

        CustomerData template = SelectCustomerTemplateForCar(carToSell);
        if (template == null)
        {
            template = customerTemplates[0];
        }

        CustomerData newCustomer = new CustomerData();
        newCustomer.customerType = template.customerType;
        newCustomer.budgetMultiplier = template.budgetMultiplier;
        newCustomer.preferredCarTypes = new List<CarData.CarType>(template.preferredCarTypes);
        newCustomer.minAcceptableCondition = template.minAcceptableCondition;

        string[] firstNames = { "Oleg", "Farukh", "Alex", "Nikita", "Lena", "Papich", "Vlad", "Emil" };
        string[] lastNames = { "Azimov", "Johnson", "Ivanov", "Evelonov", "Golovach", "Boomer", "Gig", "Unitev" };

        int firstNameIndex = Random.Range(0, firstNames.Length);
        int lastNameIndex = Random.Range(0, lastNames.Length);
        newCustomer.customerName = firstNames[firstNameIndex] + " " + lastNames[lastNameIndex];

        ResourceSystem resourceSystem = GameManager.Instance.GetComponent<ResourceSystem>();
        int playerLevel = resourceSystem.GetLevel();
        newCustomer.budget = CalculateOfferPrice(carToSell, template.budgetMultiplier, playerLevel);

        newCustomer.interestedCarId = carToSell.id;

        customerQueue.Enqueue(newCustomer);

        if (!isProcessingCustomer)
        {
            ProcessNextCustomer();
        }
    }

    private CarData SelectRandomCarFromInventory(InventorySystem inventorySystem)
    {
        List<CarData> cars = inventorySystem.GetCars();
        if (cars.Count == 0)
            return null;

        return cars[UnityEngine.Random.Range(0, cars.Count)];
    }

    private CustomerData SelectCustomerTemplateForCar(CarData car)
    {
        List<CustomerData> matchingTemplates = customerTemplates.FindAll(
            template => template.preferredCarTypes.Contains(car.carType) &&
                       (int)car.condition >= (int)template.minAcceptableCondition);

        if (matchingTemplates.Count > 0)
        {
            return matchingTemplates[UnityEngine.Random.Range(0, matchingTemplates.Count)];
        }

        matchingTemplates = customerTemplates.FindAll(
            template => (int)car.condition >= (int)template.minAcceptableCondition);

        if (matchingTemplates.Count > 0)
        {
            return matchingTemplates[UnityEngine.Random.Range(0, matchingTemplates.Count)];
        }

        return customerTemplates.Count > 0 ? customerTemplates[0] : null;
    }

    private float CalculateOfferPrice(CarData car, float budgetMultiplier, int playerLevel)
    {
        float basePrice = car.CalculateSellingPrice();
        float offer = basePrice * budgetMultiplier;

        offer *= 1.0f + (playerLevel - 1) * 0.025f;

        float randomFactor = UnityEngine.Random.Range(0.9f, 1.05f);
        offer *= randomFactor;

        return RoundValue(offer);
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

    private void ProcessNextCustomer()
    {
        if (customerQueue.Count == 0 || isProcessingCustomer)
        {
            return;
        }

        isProcessingCustomer = true;
        currentCustomer = customerQueue.Dequeue();

        SpawnCustomerVisual();

        if (activeCustomerObject != null)
        {
            CustomerVisualizer visualizer = activeCustomerObject.GetComponent<CustomerVisualizer>();
            if (visualizer != null)
            {
                visualizer.RegisterDestinationReachedCallback(OnCustomerReachedDestination);
                visualizer.MoveToDestination(customerDestinationPoint);
            }
        }
    }

    private void OnCustomerReachedDestination()
    {
        EventManager.Instance.TriggerCustomerArrived(currentCustomer);
    }

    private void SpawnCustomerVisual()
    {
        if (customerPrefab != null && customerSpawnPoint != null)
        {
            if (activeCustomerObject != null)
            {
                Destroy(activeCustomerObject);
            }

            activeCustomerObject = Instantiate(customerPrefab, customerSpawnPoint.position, customerSpawnPoint.rotation);

            CustomerVisualizer visualizer = activeCustomerObject.GetComponent<CustomerVisualizer>();
            if (visualizer != null)
            {
                visualizer.SetCustomerData(currentCustomer);
            }
        }
    }

    private void DespawnCustomerVisual()
    {
        if (activeCustomerObject != null)
        {
            Destroy(activeCustomerObject);
            activeCustomerObject = null;
        }

        isProcessingCustomer = false;

        if (customerQueue.Count > 0)
        {
            ProcessNextCustomer();
        }
    }

    public bool SellCarToCustomer()
    {
        if (currentCustomer == null)
        {
            Debug.LogWarning("No active customer to sell to");
            return false;
        }

        InventorySystem inventorySystem = GameManager.Instance.GetComponent<InventorySystem>();
        CarData car = inventorySystem.GetCarById(currentCustomer.interestedCarId);

        if (car == null)
        {
            Debug.LogWarning("Car is not found in inventory");
            return false;
        }

        if (inventorySystem.RemoveCar(car))
        {
            ResourceSystem resourceSystem = GameManager.Instance.GetComponent<ResourceSystem>();
            resourceSystem.AddMoney(currentCustomer.budget);

            float experienceForSale = currentCustomer.budget * 0.05f;
            resourceSystem.AddExperience(experienceForSale);

            PlayerData playerData = resourceSystem.GetPlayerData();
            playerData.totalCarsSold++;

            EventManager.Instance.TriggerCarSold(car);

            CustomerData soldToCustomer = currentCustomer;
            currentCustomer = null;
            DespawnCustomerVisual();

            EventManager.Instance.TriggerCustomerLeft(soldToCustomer, true);

            return true;
        }

        return false;
    }

    public void DeclineCustomer()
    {
        if (currentCustomer != null)
        {
            CustomerData declinedCustomer = currentCustomer;
            currentCustomer = null;
            DespawnCustomerVisual();

            EventManager.Instance.TriggerCustomerLeft(declinedCustomer, false);
        }
    }

    public int GetQueueLength()
    {
        return customerQueue.Count;
    }

    public CustomerData GetCurrentCustomer()
    {
        return currentCustomer;
    }
}