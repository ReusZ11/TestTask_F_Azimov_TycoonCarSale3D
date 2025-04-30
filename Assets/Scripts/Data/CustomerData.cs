using System.Collections.Generic;

[System.Serializable]
public class CustomerData
{
    public string customerName;
    public CustomerType customerType;
    public float budgetMultiplier;
    public float budget; 
    public List<CarData.CarType> preferredCarTypes = new List<CarData.CarType>(); 
    public CarData.CarCondition minAcceptableCondition; 
    public string interestedCarId; 


    [System.Serializable]
    public enum CustomerType
    {
        Regular,  
        Business, 
        Family,  
        Sports,   
        Wealthy  
    }
}