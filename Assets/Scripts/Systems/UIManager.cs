using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    //private AISystem aiSystem;
    private CarDealershipSystem carDealershipSystem;
    private CustomerSystem customerSystem;
    private ResourceSystem resourceSystem;
    private InventorySystem inventorySystem;
    private BuildSystem buildSystem;

    [Header("Panels UI")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject buildingPanel;
    [SerializeField] private GameObject carShopPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject customerPanel;

    [Header("Player Data UI")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider experienceSlider;

    [Header("Car Shop UI")]
    [SerializeField] private Transform carShopContainer;
    [SerializeField] private GameObject carItemPrefab;
    [SerializeField] private TextMeshProUGUI shopRefreshTimerText;


    [Header("Inventory UI")]
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private GameObject inventoryItemPrefab;

    [Header("Customer UI")]
    [SerializeField] private TextMeshProUGUI customerNameText;
    [SerializeField] private TextMeshProUGUI customerOfferText;
    [SerializeField] private TextMeshProUGUI carInfoText;
    [SerializeField] private Image carImage;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;

    [Header("Loading UI")]
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingProgressText;
    [SerializeField] private Button startGameButton;
    private bool isLoading = false;


    private float uiUpdateTimer = 0f;
    private const float UI_UPDATE_INTERVAL = 0.5f;

    [SerializeField] private Transform buildingContainer;
    [SerializeField] private GameObject buildingItemPrefab;

    private BuildingData selectedBuilding;
    private CarData selectedCar;

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

        customerSystem = GameManager.Instance.GetComponent<CustomerSystem>();
        carDealershipSystem = GameManager.Instance.GetComponent<CarDealershipSystem>();
        //aiSystem = GameManager.Instance.GetComponent<AISystem>();
        resourceSystem = GameManager.Instance.GetComponent<ResourceSystem>();
        inventorySystem = GameManager.Instance.GetComponent<InventorySystem>();
        buildSystem = GameManager.Instance.GetComponent<BuildSystem>();
    }

    private void Start()
    {
        SubscribeToEvents();

        ShowMainMenu();
    }

    private void SubscribeToEvents()
    {
        EventManager.Instance.OnMoneyChanged += UpdateMoneyDisplay;
        EventManager.Instance.OnLevelUp += UpdateLevelDisplay;
        EventManager.Instance.OnBuildingConstructed += (building) => UpdateBuildingsDisplay();
        EventManager.Instance.OnBuildingUpgraded += (building) => UpdateBuildingsDisplay();
        EventManager.Instance.OnCarPurchased += (car) => UpdateInventoryDisplay();
        EventManager.Instance.OnCarSold += (car) => UpdateInventoryDisplay();
        EventManager.Instance.OnCarRepaired += (car) => UpdateInventoryDisplay();
        EventManager.Instance.OnInventoryChanged += UpdateInventoryDisplay;
        EventManager.Instance.OnCarShopRefreshed += UpdateCarShopDisplay;
        EventManager.Instance.OnCustomerArrived += ShowCustomerPanel;
        EventManager.Instance.OnCustomerLeft += HideCustomerPanel;
    }

    private void Update()
    {
        uiUpdateTimer += Time.deltaTime;
        if (uiUpdateTimer >= UI_UPDATE_INTERVAL)
        {
            uiUpdateTimer = 0f;

            if (carShopPanel != null && carShopPanel.activeSelf)
            {
                UpdateShopRefreshTimer();
            }
        }
    }

    private void UpdateMoneyDisplay(float money)
    {
        if (moneyText != null)
        {
            moneyText.text = "$" + money.ToString("N0");
        }
    }

    private void UpdateLevelDisplay(int level)
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + level;
        }

        UpdateExperienceSlider();
    }

    private void UpdateExperienceSlider()
    {
        if (experienceSlider != null)
        {
            float experience = resourceSystem.GetExperience();
            float maxExperience = resourceSystem.GetExperienceToNextLevel();

            experienceSlider.value = experience / maxExperience;
        }
    }

    public void ShowBuildingsPanel()
    {
        HideAllPanels();
        buildingPanel.SetActive(true);

        UpdateBuildingsDisplay();
    }

    private void UpdateBuildingsDisplay()
    {
        foreach (Transform child in buildingContainer)
        {
            Destroy(child.gameObject);
        }

        BuildingData parkingData = buildSystem.GetParkingData();

        GameObject buildingItem = Instantiate(buildingItemPrefab, buildingContainer);
        BuildingItemUI buildingItemUI = buildingItem.GetComponent<BuildingItemUI>();

        if (buildingItemUI != null)
        {
            buildingItemUI.SetBuildingData(parkingData);
            buildingItemUI.OnBuildOrUpgrade += BuildOrUpgradeBuilding;
        }
    }

    private void UpdateShopRefreshTimer()
    {
        if (shopRefreshTimerText != null && carDealershipSystem != null)
        {
            float timeLeft = carDealershipSystem.GetTimeUntilNextShopRefresh();

            int minutes = Mathf.FloorToInt(timeLeft / 60);
            int seconds = Mathf.FloorToInt(timeLeft % 60);

            shopRefreshTimerText.text = $"Next refresh: {minutes:00}:{seconds:00}";
        }
    }

        private void BuildOrUpgradeBuilding(BuildingData building)
        {
            if (!building.isConstructed)
            {
                if (buildSystem.ConstructBuilding())
                {
                    UpdateBuildingsDisplay();
                }
            }
            else
            {
                if (buildSystem.UpgradeBuilding())
                {
                    UpdateBuildingsDisplay();
                }
            }
        }

    private void UpdateCarShopDisplay()
    {
        foreach (Transform child in carShopContainer)
        {
            Destroy(child.gameObject);
        }

        List<CarData> availableCars = carDealershipSystem.GetAvailableCarsForPurchase();

        bool hasParkingLot = buildSystem.GetCurrentMaxCars() > 0;

        foreach (var car in availableCars)
        {
            GameObject carItem = Instantiate(carItemPrefab, carShopContainer);
            CarItemUI carItemUI = carItem.GetComponent<CarItemUI>();

            if (carItemUI != null)
            {
                carItemUI.SetCarData(car);
                carItemUI.OnCarSelected += SelectCarInShop;
                carItemUI.buyButton.onClick.AddListener(() => PurchaseCar(car));


                bool canAfford = resourceSystem.CanAfford(car.basePurchasePrice);
                bool hasSpace = inventorySystem.GetFreeCarsSpace() > 0;

                carItemUI.buyButton.interactable = canAfford && hasSpace && hasParkingLot;

                if (!hasParkingLot)
                {
                    carItemUI.SetStatusMessage("Build a parking lot first!");
                }
                else if (!hasSpace)
                {
                    carItemUI.SetStatusMessage("No free space in inventory!");
                }
                else if (!canAfford)
                {
                    carItemUI.SetStatusMessage("Not enough money!");
                }
                else
                {
                    carItemUI.SetStatusMessage("");
                }
            }
        }
    }

    private void SelectCarInShop(CarData car)
    {
        selectedCar = car;
    }

    private void PurchaseCar(CarData car)
    {
        if (carDealershipSystem.PurchaseCarFromShop(car))
        {
            UpdateCarShopDisplay();
        }
        else
        {
            ShowErrorMessage("Failed to purchase car");
        }
    }

    private void UpdateInventoryDisplay()
    {
        foreach (Transform child in inventoryContainer)
        {
            Destroy(child.gameObject);
        }

        List<CarData> cars = inventorySystem.GetCars();

        foreach (var car in cars)
        {
            GameObject carItem = Instantiate(inventoryItemPrefab, inventoryContainer);
            CarItemUI carItemUI = carItem.GetComponent<CarItemUI>();

            if (carItemUI != null)
            {
                carItemUI.SetCarData(car);
                carItemUI.repairButton.gameObject.SetActive(true);
                carItemUI.repairButton.onClick.AddListener(() => RepairCar(car));

                bool canRepair = car.condition != CarData.CarCondition.Excellent;
                carItemUI.repairButton.interactable = canRepair && resourceSystem.CanAfford(car.GetRepairCost());

                CustomerData customer = customerSystem.GetCurrentCustomer();
/*
                carItemUI.sellButton.gameObject.SetActive(customer != null);
                if (customer != null)
                {
                    carItemUI.sellButton.onClick.AddListener(() => SellCarToCustomer(car));

                    bool isAcceptable = IsCarAcceptableForCustomer(car, customer);
                    carItemUI.sellButton.interactable = isAcceptable;
                }*/
            }
        }
    }

    private bool IsCarAcceptableForCustomer(CarData car, CustomerData customer)
    {
        if ((int)car.condition < (int)customer.minAcceptableCondition)
        {
            return false;
        }

        bool matchesPreferredType = customer.preferredCarTypes.Contains(car.carType);

        if (!matchesPreferredType && car.condition != CarData.CarCondition.Excellent)
        {
            return false;
        }

        float carPrice = car.CalculateSellingPrice();
        return carPrice <= customer.budget;
    }

/*    
    private void SelectCarInInventory(CarData car)
    {
        selectedCar = car;

        AISystem aiSystem = GameManager.Instance.GetComponent<AISystem>();
        CustomerData customer = aiSystem.GetCurrentCustomer();

        if (customer != null && sellCarButton != null)
        {
            sellCarButton.onClick.RemoveAllListeners();
            sellCarButton.onClick.AddListener(() => SellCarToCustomer(car));

            bool isAcceptable = IsCarAcceptableForCustomer(car, customer);
            sellCarButton.interactable = isAcceptable;
        }
    }*/

    private void RepairCar(CarData car)
    {

        if (inventorySystem.RepairCar(car))
        {
            UpdateInventoryDisplay();
        }
        else
        {
            ShowErrorMessage("Failed to repair car");
        }
    }

    private void SellCarToCustomer(CarData car)
    {

        if (customerSystem.SellCarToCustomer())
        {
            UpdateInventoryDisplay();
        }
        else
        {
            ShowErrorMessage("Failed to sell car");
        }
    }

    private void ShowCustomerPanel(CustomerData customer)
    {
        customerPanel.SetActive(true);

        if (customerNameText != null)
        {
            customerNameText.text = customer.customerName;
        }

        if (customerOfferText != null)
        {
            customerOfferText.text = "Offer: $" + customer.budget.ToString("N0");
        }

        CarData car = inventorySystem.GetCarById(customer.interestedCarId);

        if (car != null)
        {
            if (carInfoText != null)
            {
                string info = car.carName + "\n";
                info += "Type: " + car.carType.ToString() + "\n";
                info += "Condition: " + car.condition.ToString() + "\n";
                info += "Value: $" + car.CalculateSellingPrice().ToString("N0");
                carInfoText.text = info;
            }

            if (carImage != null)
            {
                Sprite carSprite = car.GetCarSprite();
                if (carSprite != null)
                {
                    carImage.sprite = carSprite;
                }
            }
        }

        if (acceptButton != null)
        {
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(() => {
                customerSystem.SellCarToCustomer();
            });
        }

        if (declineButton != null)
        {
            declineButton.onClick.RemoveAllListeners();
            declineButton.onClick.AddListener(() => {
                customerSystem.DeclineCustomer();
            });
        }
    }

    private void HideCustomerPanel(CustomerData customer, bool purchased)
    {
        customerPanel.SetActive(false);

        if (purchased)
        {
            ShowInfoMessage(customer.customerName + " purchased a car for $" + customer.budget.ToString("N0") + "!");
        }
        else
        {
            ShowInfoMessage(customer.customerName + " left without buying.");
        }

        UpdateInventoryDisplay();
    }

    public void ShowMainMenu()
    {
        HideAllPanels();
        mainMenuPanel.SetActive(true);
    }

    public void StartGameWithLoading()
    {
        if (startGameButton != null)
            startGameButton.interactable = false;

        if (loadingSlider != null)
        {
            loadingSlider.gameObject.SetActive(true);
            loadingSlider.value = 0f;
        }

        if (loadingProgressText != null)
        {
            loadingProgressText.gameObject.SetActive(true);
            loadingProgressText.text = "Loading: 0%";
        }

        isLoading = true;
        StartCoroutine(LoadGameRoutine());
    }

    private IEnumerator LoadGameRoutine()
    {
        float loadingTime = 3f; 
        float elapsedTime = 0f;

        while (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / loadingTime);

            if (loadingSlider != null)
                loadingSlider.value = progress;

            if (loadingProgressText != null)
                loadingProgressText.text = $"Loading: {Mathf.RoundToInt(progress * 100)}%";

            yield return null;
        }

        isLoading = false;

        StartGame();

        if (loadingSlider != null)
            loadingSlider.gameObject.SetActive(false);

        if (loadingProgressText != null)
            loadingProgressText.gameObject.SetActive(false);

        if (startGameButton != null)
            startGameButton.interactable = true;
    }

    public void StartGame()
    {
        HideAllPanels();
        gameplayPanel.SetActive(true);

        UpdateMoneyDisplay(resourceSystem.GetMoney());
        UpdateLevelDisplay(resourceSystem.GetLevel());
        UpdateInventoryDisplay();
    }

    public void ShowSettings()
    {
        HideAllPanels();
        settingsPanel.SetActive(true);
    }

    public void ShowCarShop()
    {
        HideAllPanels();
        carShopPanel.SetActive(true);

        UpdateCarShopDisplay();
        UpdateShopRefreshTimer();
    }

    public void ShowInventory()
    {
        HideAllPanels();
        inventoryPanel.SetActive(true);

        UpdateInventoryDisplay();
    }

    private void HideAllPanels()
    {
        mainMenuPanel.SetActive(false);
        gameplayPanel.SetActive(false);
        settingsPanel.SetActive(false);
        buildingPanel.SetActive(false);
        carShopPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        customerPanel.SetActive(false);
    }

    private void ShowErrorMessage(string message)
    {
        Debug.LogError(message);
    }

    private void ShowInfoMessage(string message)
    {
        Debug.Log(message);
    }

    public void QuitGame()
    {
        GameManager.Instance.SaveGame();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}