using System.Collections.Generic;

[System.Serializable]
public class InventoryData
{
    public List<CarData> cars = new List<CarData>();
    public int maxCars = 3; 

    public bool AddCar(CarData car)
    {
        if (cars.Count < maxCars)
        {
            cars.Add(car);
            return true;
        }
        return false;
    }

    public bool RemoveCar(CarData car)
    {
        return cars.Remove(car);
    }

    public bool RemoveCar(int index)
    {
        if (index >= 0 && index < cars.Count)
        {
            cars.RemoveAt(index);
            return true;
        }
        return false;
    }

    public int GetFreeCarsSpace()
    {
        return maxCars - cars.Count;
    }

    public void IncreaseMaxCars(int amount)
    {
        maxCars += amount;
    }
}