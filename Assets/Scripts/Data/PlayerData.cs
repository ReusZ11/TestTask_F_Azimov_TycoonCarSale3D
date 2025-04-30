[System.Serializable]
public class PlayerData
{
    public string playerName = "Player";
    public float money = 10000f; 
    public int level = 1; 
    public float experience = 0; 
    public float experienceToNextLevel = 100; 
    public int totalCarsSold = 0; 
    public float totalProfit = 0; 
    public float passiveIncomePerSecond = 0; 

    public bool AddExperience(float amount)
    {
        experience += amount;
        if (experience >= experienceToNextLevel)
        {
            LevelUp();
            return true;
        }
        return false;
    }

    private void LevelUp()
    {
        level++;
        experience -= experienceToNextLevel;
        experienceToNextLevel *= 1.5f; 
    }

    public void AddMoney(float amount)
    {
        money += amount;
        if (amount > 0)
        {
            totalProfit += amount;
        }
    }

    public bool CanAfford(float amount)
    {
        return money >= amount;
    }

    public bool SpendMoney(float amount)
    {
        if (CanAfford(amount))
        {
            money -= amount;
            return true;
        }
        return false;
    }
}