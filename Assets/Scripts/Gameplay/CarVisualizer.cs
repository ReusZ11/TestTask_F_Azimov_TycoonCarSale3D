using UnityEngine;

public class CarVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject junkVisual; // Визуал для состояния Junk
    [SerializeField] private GameObject poorVisual; // Визуал для состояния Poor
    [SerializeField] private GameObject averageVisual; // Визуал для состояния Average
    [SerializeField] private GameObject goodVisual; // Визуал для состояния Good
    [SerializeField] private GameObject excellentVisual; // Визуал для состояния Excellent

    public void UpdateVisuals(CarData car)
    {
        // Скрываем все визуалы
        if (junkVisual != null) junkVisual.SetActive(false);
        if (poorVisual != null) poorVisual.SetActive(false);
        if (averageVisual != null) averageVisual.SetActive(false);
        if (goodVisual != null) goodVisual.SetActive(false);
        if (excellentVisual != null) excellentVisual.SetActive(false);

        // Активируем нужный визуал в зависимости от состояния
        switch (car.condition)
        {
            case CarData.CarCondition.Junk:
                if (junkVisual != null) junkVisual.SetActive(true);
                break;
            case CarData.CarCondition.Poor:
                if (poorVisual != null) poorVisual.SetActive(true);
                break;
            case CarData.CarCondition.Average:
                if (averageVisual != null) averageVisual.SetActive(true);
                break;
            case CarData.CarCondition.Good:
                if (goodVisual != null) goodVisual.SetActive(true);
                break;
            case CarData.CarCondition.Excellent:
                if (excellentVisual != null) excellentVisual.SetActive(true);
                break;
        }
    }
}