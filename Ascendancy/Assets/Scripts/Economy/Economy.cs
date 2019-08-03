using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economy : MonoBehaviour
{
    public Dictionary<Resource, float> resourceStorage;

    public List<Resource> availableResources;
    
    public void Initialize()
    {
        resourceStorage = new Dictionary<Resource, float>();

        foreach (Resource resource in availableResources)
            resourceStorage.Add(resource, 0);
    }
}
