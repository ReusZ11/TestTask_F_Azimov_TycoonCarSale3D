using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarDatabase", menuName = "Game/Car Database")]
public class CarDatabase : ScriptableObject
{
    public List<CarTypeData> carTypes = new List<CarTypeData>();

    public CarTypeData GetCarTypeData(CarData.CarType carType)
    {
        return carTypes.Find(ct => ct.carType == carType);
    }
}