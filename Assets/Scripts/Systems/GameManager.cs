using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private BuildSystem buildSystem;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private ResourceSystem resourceSystem;
    [SerializeField] private CarDealershipSystem carDealershipSystem; 
    [SerializeField] private CustomerSystem customerSystem;
    [SerializeField] private SaveSystem saveSystem;

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

    private void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.targetFrameRate = 60;
#endif
        InitializeSystems();
    }

    private void InitializeSystems()
    {
        buildSystem.Initialize();
        inventorySystem.Initialize();
        resourceSystem.Initialize();
        carDealershipSystem.Initialize(); 
        customerSystem.Initialize(); 


        saveSystem.Initialize();
        LoadGame();
    }

    public void SaveGame()
    {
        buildSystem.SaveState();
        inventorySystem.SaveState();
        resourceSystem.SaveState();

        saveSystem.SaveGame();
    }

    public void LoadGame()
    {
        if (saveSystem.HasSaveData())
        {
            saveSystem.LoadGame();

            buildSystem.LoadState();
            inventorySystem.LoadState();
            resourceSystem.LoadState();
        }
        else
        {
            StartNewGame();
        }
    }

    private void StartNewGame()
    {
    }
}