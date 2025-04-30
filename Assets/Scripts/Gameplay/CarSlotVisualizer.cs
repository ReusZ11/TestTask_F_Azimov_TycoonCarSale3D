using TMPro;
using UnityEngine;

public class CarSlotVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject emptySlotVisual; 
    [SerializeField] private GameObject constructedSlotVisual; 
    [SerializeField] private GameObject buildPlaceholderVisual; 
    //[SerializeField] private TextMeshProUGUI levelText; 

    public void UpdateVisuals(BuildingData buildingData)
    {
        if (buildingData.isConstructed)
        {
            if (emptySlotVisual != null) emptySlotVisual.SetActive(false);
            if (constructedSlotVisual != null) constructedSlotVisual.SetActive(true);
            if (buildPlaceholderVisual != null) buildPlaceholderVisual.SetActive(false);

/*           
            if (levelText != null)
            {
                levelText.text = "Level " + buildingData.level;
                levelText.gameObject.SetActive(true);
            }*/
        }
        else
        {
            if (emptySlotVisual != null) emptySlotVisual.SetActive(true);
            if (constructedSlotVisual != null) constructedSlotVisual.SetActive(false);
            if (buildPlaceholderVisual != null) buildPlaceholderVisual.SetActive(false);

/*            if (levelText != null)
            {
                levelText.text = "";
                levelText.gameObject.SetActive(false);
            }*/
        }
    }

    public void ShowBuildPlaceholder()
    {
        if (emptySlotVisual != null) emptySlotVisual.SetActive(false);
        if (constructedSlotVisual != null) constructedSlotVisual.SetActive(false);
        if (buildPlaceholderVisual != null) buildPlaceholderVisual.SetActive(true);

    }
}