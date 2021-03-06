﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Economy : NetworkBehaviour
{
    public readonly SyncDictionary<string, float> resourceSyncDictionary = new SyncDictionary<string, float>();
    public SubscribableDictionary<Resource, float> resourceStorage;
    public SubscribableList<Resource> availableResources;

    private List<ResourceAmount> startResources;

    public override void OnStartServer()
    {
        base.OnStartServer();
        resourceSyncDictionary.Add("Wood", 169);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        resourceSyncDictionary.Callback += OnResourceChange;
    }

    public void Initialize()
    {
        //adding start resources
        startResources = GameSettingsManager.instance.startResources;

        resourceStorage = new SubscribableDictionary<Resource, float>();
        availableResources = new SubscribableList<Resource>();
    }
    void OnResourceChange(SyncDictionary<string, float>.Operation op, string resource, float amount)
    {
        Debug.Log(op + " - " + resource + " " + amount);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            resourceSyncDictionary["Wood"] = 20;
    }

    public void NewAvailableResource(Resource resource)
    {
        //Debug.Log("New Resource: " + resource.name);
        availableResources.Add(resource);
        resourceStorage.Add(resource, 0f);

        foreach (ResourceAmount resAm in startResources)
        {
            if (resAm.resource.Equals(resource))
                AddResourceAmount(resAm);
        }

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
        Debug.Assert(availableResources.Contains(resource), "Unavailable Resource checked: " + resource.name);
        return resourceStorage.GetValue(resource);
    }

    #region Resource Addition/Subtraction
    public void SetResourceAmount(Resource resource, float amount)
    {
        Debug.Assert(availableResources.Contains(resource), "Unavailable Resource checked: " + resource.name);
        Debug.Assert(amount > 0, "Amount must be positive: " + amount);

        resourceStorage.SetValue(resource, amount);
    }
    public void SetResourceAmount(ResourceAmount res_amount)
    {
        SetResourceAmount(res_amount.resource, res_amount.amount);
    }

    public void AddResourceAmount(Resource resource, float amount)
    {
        Debug.Assert(amount > 0, "Amount must be positive: " + amount);

        float newAmount = GetResourceAmount(resource) + amount;
        resourceStorage.SetValue(resource, newAmount);
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
        resourceStorage.SetValue(res_amount.resource, newAmount);
    }
    #endregion
}
