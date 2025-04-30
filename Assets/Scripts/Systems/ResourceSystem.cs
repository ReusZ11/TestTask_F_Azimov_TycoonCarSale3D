using UnityEngine;

public class ResourceSystem : BaseSystem
{
    private PlayerData playerData;
    private float timeSinceLastPassiveIncome = 0f;

    [SerializeField] private float passiveIncomeInterval = 1f; 

    public override void Initialize()
    {
        base.Initialize();

        if (playerData == null)
        {
            playerData = new PlayerData();
        }
    }

    private void Update()
    {
        if (playerData.passiveIncomePerSecond > 0)
        {
            timeSinceLastPassiveIncome += Time.deltaTime;

            if (timeSinceLastPassiveIncome >= passiveIncomeInterval)
            {
                float income = playerData.passiveIncomePerSecond * timeSinceLastPassiveIncome;
                AddMoney(income);
                timeSinceLastPassiveIncome = 0f;

                EventManager.Instance.TriggerMoneyChanged(playerData.money);
            }
        }
    }

    public void AddMoney(float amount)
    {
        playerData.AddMoney(amount);

        EventManager.Instance.TriggerMoneyChanged(playerData.money);
    }

    public bool SpendMoney(float amount)
    {
        bool success = playerData.SpendMoney(amount);

        if (success)
        {
            EventManager.Instance.TriggerMoneyChanged(playerData.money);
        }

        return success;
    }

    public void AddExperience(float amount)
    {
        bool leveledUp = playerData.AddExperience(amount);

        if (leveledUp)
        {
            EventManager.Instance.TriggerLevelUp(playerData.level);
        }
    }

    public void IncreasePassiveIncome(float amount)
    {
        playerData.passiveIncomePerSecond += amount;
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public void SetPlayerData(PlayerData data)
    {
        playerData = data;

        EventManager.Instance.TriggerMoneyChanged(playerData.money);
        EventManager.Instance.TriggerLevelUp(playerData.level);
    }

    public bool CanAfford(float amount)
    {
        return playerData.CanAfford(amount);
    }

    public float GetMoney() { return playerData.money; }
    public int GetLevel() { return playerData.level; }
    public float GetExperience() { return playerData.experience; }
    public float GetExperienceToNextLevel() { return playerData.experienceToNextLevel; }
    public float GetPassiveIncome() { return playerData.passiveIncomePerSecond; }
}