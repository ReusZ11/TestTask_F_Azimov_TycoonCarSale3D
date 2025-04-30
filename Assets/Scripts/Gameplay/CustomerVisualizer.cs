using System;
using UnityEngine;

public class CustomerVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject regularCustomerVisual;
    [SerializeField] private GameObject businessCustomerVisual;
    [SerializeField] private GameObject familyCustomerVisual;
    [SerializeField] private GameObject sportsCustomerVisual;
    [SerializeField] private GameObject wealthyCustomerVisual;

    [SerializeField] private CustomerMovement movement;

    private CustomerData customerData;

    private void Awake()
    {
        if (movement == null)
            movement = GetComponent<CustomerMovement>();
    }

    public void SetCustomerType(CustomerData.CustomerType customerType)
    {
        if (regularCustomerVisual != null) regularCustomerVisual.SetActive(false);
        if (businessCustomerVisual != null) businessCustomerVisual.SetActive(false);
        if (familyCustomerVisual != null) familyCustomerVisual.SetActive(false);
        if (sportsCustomerVisual != null) sportsCustomerVisual.SetActive(false);
        if (wealthyCustomerVisual != null) wealthyCustomerVisual.SetActive(false);

        switch (customerType)
        {
            case CustomerData.CustomerType.Regular:
                if (regularCustomerVisual != null) regularCustomerVisual.SetActive(true);
                break;
            case CustomerData.CustomerType.Business:
                if (businessCustomerVisual != null) businessCustomerVisual.SetActive(true);
                break;
            case CustomerData.CustomerType.Family:
                if (familyCustomerVisual != null) familyCustomerVisual.SetActive(true);
                break;
            case CustomerData.CustomerType.Sports:
                if (sportsCustomerVisual != null) sportsCustomerVisual.SetActive(true);
                break;
            case CustomerData.CustomerType.Wealthy:
                if (wealthyCustomerVisual != null) wealthyCustomerVisual.SetActive(true);
                break;
        }
    }

    public void SetCustomerData(CustomerData data)
    {
        customerData = data;
        SetCustomerType(data.customerType);
    }

    public void MoveToDestination(Transform destination)
    {
        if (movement != null)
        {
            movement.MoveTo(destination);
        }
    }

    public void RegisterDestinationReachedCallback(Action callback)
    {
        if (movement != null)
        {
            movement.OnDestinationReached += callback;
        }
    }
}