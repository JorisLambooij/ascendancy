using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Economy : NetworkBehaviour
{
    public SubscribableDictionary<Resource, float> resourceStorage;
    public SubscribableList<Resource> availableResources;
    
    public void Initialize()
    {
        resourceStorage = new SubscribableDictionary<Resource, float>();
        availableResources = new SubscribableList<Resource>();

        foreach (Resource resource in availableResources.AsList)
            resourceStorage.Add(resource, 0);
    }

    public void NewAvailableResource(Resource r)
    {
        //Debug.Log("New Resource: " + r.resourceName);
        availableResources.Add(r);
        resourceStorage.Add(r, 0);
    }
}
