using UnityEngine;

[CreateAssetMenu(fileName = "CarTypeData", menuName = "Game/Car Type Data")]
public class CarTypeData : ScriptableObject
{
    public string typeName;
    public CarData.CarType carType;
    public float basePurchasePrice;
    public float repairCost;
    public GameObject carPrefab; 
    public Sprite carSprite;      
}