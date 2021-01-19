using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economy : MonoBehaviour
{
    public SubscribableDictionary<Resource, float> resourceStorage;
    public SubscribableList<Resource> availableResources;

    private List<Resource_Amount> startResources;
    
    public void Initialize()
    {
        //adding start resources
        startResources = GameSettingsManager.instance.startResources;

        resourceStorage = new SubscribableDictionary<Resource, float>();
        availableResources = new SubscribableList<Resource>();
    }

    public void NewAvailableResource(Resource r)
    {
        Debug.Log("New Resource: " + r.name);
        availableResources.Add(r);
        resourceStorage.Add(r, 0f);

        foreach (Resource_Amount resAm in startResources)
        {
            if (resAm.resource.Equals(r))
                AddResources(resAm);
        }

    }

    /// <summary>
    /// Checks the Resource_Amount r against the player's stored Resources.
    /// </summary>
    /// <param name="r"></param>
    /// <returns>True if the storage contains enough of the corresponding Resource. </returns>
    public bool CheckResource(Resource_Amount r)
    {
        return r.amount <= GetResourceAmount(r.resource);
    }
    /// <summary>
    /// Checks the current amount of stored Resource r.
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public float GetResourceAmount(Resource r)
    {
        Debug.Assert(availableResources.Contains(r), "Unavailable Resource checked: " + r);
        return resourceStorage.GetValue(r);
    }

    public void AddResources(Resource_Amount res_amount)
    {
        Debug.Assert(res_amount.amount > 0, "Amount must be positive: " + res_amount.amount);

        float newAmount = GetResourceAmount(res_amount.resource) + res_amount.amount;
        resourceStorage.SetValue(res_amount.resource, newAmount);
    }

    public void RemoveResources(Resource_Amount res_amount)
    {
        Debug.Assert(res_amount.amount > 0, "Amount must be positive: " + res_amount.amount);
        Debug.Assert(CheckResource(res_amount), "Not enough of Resource " + res_amount.resource.name + " in storage (" + res_amount.amount + ")");

        float newAmount = GetResourceAmount(res_amount.resource) - res_amount.amount;
        resourceStorage.SetValue(res_amount.resource, newAmount);
    }
}
