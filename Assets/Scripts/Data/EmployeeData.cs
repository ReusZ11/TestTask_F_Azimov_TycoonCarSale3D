[System.Serializable]
public class EmployeeData
{
    public string id;
    public string employeeName;
    public int skillLevel = 1;
    public float salary; 
    public float efficiency = 1.0f; 
    public bool isEmployed = false; 

    public float GetIncomeBonus()
    {
        return skillLevel * 0.05f; 
    }

    public float GetRepairSpeedBonus()
    {
        return skillLevel * 0.1f;
    }
}