﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Economy : NetworkBehaviour
{
    public readonly SyncDictionary<string, float> resourceSyncDictionary = new SyncDictionary<string, float>();
    public readonly SyncList<string> availableResources = new SyncList<string>();

    //public SubscribableDictionary<Resource, float> resourceStorage;
    //public SubscribableList<Resource> availableResources;

    //private List<ResourceAmount> startResources;

    public void Initialize()
    {
        //adding start resources
        //startResources = GameSettingsManager.instance.startResources;

        //resourceStorage = new SubscribableDictionary<Resource, float>();
        //availableResources = new SubscribableList<Resource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            resourceSyncDictionary["Wood"] = 20;
    }


    public void NewAvailableResource(Resource resource)
    {
        if (isServer)
            NewAvailableResource(resource.name);
        else
            CmdNewAvailableResource(resource.name);
    }

    [Command]
    public void CmdNewAvailableResource(string resource)
    {
        NewAvailableResource(resource);
    }

    private void NewAvailableResource(string resource)
    {
        if (!availableResources.Contains(resource))
            availableResources.Add(resource);

        if (!resourceSyncDictionary.ContainsKey(resource))
            resourceSyncDictionary.Add(resource, 0);

        foreach (ResourceAmount resAm in GameSettingsManager.instance.startResources)
            if (resAm.resource.name.Equals(resource))
                AddResourceAmount(resAm);
    }

    /// <summary>
    /// Checks the Resource_Amount r against the player's stored Resources.
    /// </summary>
    /// <returns>True if the storage contains enough of the corresponding Resource. </returns>
    public bool IsRecourceAmountAvailable(ResourceAmount res_amount)
    {
        return res_amount.amount <= GetResourceAmount(res_amount.resource);
    }
    /// <summary>
    /// Returns the current amount of stored Resource r.
    /// </summary>
    public float GetResourceAmount(Resource resource)
    {
        Debug.Assert(availableResources.Contains(resource.name), "Unavailable Resource checked: " + resource.name);
        return resourceSyncDictionary[resource.name];
        //return resourceStorage.GetValue(resource);
    }

    #region Resource Addition/Subtraction
    public void SetResourceAmount(Resource resource, float amount)
    {
        Debug.Assert(availableResources.Contains(resource.name), "Unavailable Resource checked: " + resource.name);
        Debug.Assert(amount >= 0, "Amount must be positive: " + amount);

        //resourceSyncDictionary[resource.name] = amount;
        //resourceStorage.SetValue(resource, amount);

        if (isServer)
            resourceSyncDictionary[resource.name] = amount;
        else
            CmdSetResourceAmount(resource.name, amount);
    }

    public bool IsResourceAvailable(Resource resource)
    {
        return availableResources.Contains(resource.name);
    }

    [Command]
    public void CmdSetResourceAmount(string resource, float amount)
    {
        resourceSyncDictionary[resource] = amount;
    }

    public void SetResourceAmount(ResourceAmount res_amount)
    {
        SetResourceAmount(res_amount.resource, res_amount.amount);
    }

    public void AddResourceAmount(Resource resource, float amount)
    {
        Debug.Assert(amount > 0, "Amount must be positive: " + amount);

        float newAmount = GetResourceAmount(resource) + amount;
        SetResourceAmount(resource, newAmount);
    }
    public void AddResourceAmount(ResourceAmount res_amount)
    {
        AddResourceAmount(res_amount.resource, res_amount.amount);
    }

    public void RemoveResourceAmount(ResourceAmount res_amount)
    {
        Debug.Assert(res_amount.amount > 0, "Amount must be positive: " + res_amount.amount);
        Debug.Assert(IsRecourceAmountAvailable(res_amount), "Not enough of Resource " + res_amount.resource.name + " in storage (" + res_amount.amount + ")");

        float newAmount = GetResourceAmount(res_amount.resource) - res_amount.amount;
        SetResourceAmount(res_amount.resource, newAmount);
    }
    #endregion
}
