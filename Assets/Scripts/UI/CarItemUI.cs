using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI carNameText;
    [SerializeField] private TextMeshProUGUI carTypeText;
    [SerializeField] private TextMeshProUGUI carConditionText;
    [SerializeField] private TextMeshProUGUI carPriceText;
    [SerializeField] private Image carImage;
    [SerializeField] private TextMeshProUGUI statusMessageText;


    public Button buyButton;
    public Button repairButton;
    //public Button sellButton;

    private CarData carData;

    public event Action<CarData> OnCarSelected;

    public void SetCarData(CarData car)
    {
        carData = car;

        if (carNameText != null)
        {
            carNameText.text = car.carName;
        }

        if (carTypeText != null)
        {
            carTypeText.text = car.carType.ToString();
        }

        if (carConditionText != null)
        {
            carConditionText.text = "Condition: " + car.condition.ToString();

            switch (car.condition)
            {
                case CarData.CarCondition.Junk:
                    carConditionText.color = Color.red;
                    break;
                case CarData.CarCondition.Poor:
                    carConditionText.color = new Color(1.0f, 0.5f, 0f); // Оранжевый
                    break;
                case CarData.CarCondition.Average:
                    carConditionText.color = Color.yellow;
                    break;
                case CarData.CarCondition.Good:
                    carConditionText.color = Color.green;
                    break;
                case CarData.CarCondition.Excellent:
                    carConditionText.color = new Color(0f, 1.0f, 1.0f); // Голубой
                    break;
            }
        }

        if (carPriceText != null)
        {
            if (buyButton != null && buyButton.gameObject.activeSelf)
            {
                carPriceText.text = "Buy: $" + car.basePurchasePrice.ToString("N0");
            }
            else
            {
                carPriceText.text = "Value: $" + car.CalculateSellingPrice().ToString("N0");
            }
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


    public void SetStatusMessage(string message)
    {
        if (statusMessageText != null)
        {
            statusMessageText.text = message;
            statusMessageText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        }
    }

    public void OnClick()
    {
        OnCarSelected?.Invoke(carData);
    }
}