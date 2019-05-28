using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economy : MonoBehaviour
{
    public Dictionary<Resource, float> resourceStorage;

    public List<Resource> availableResources;

    // Start is called before the first frame update
    void Awake()
    {
        resourceStorage = new Dictionary<Resource, float>();

        foreach (Resource resource in availableResources)
            resourceStorage.Add(resource, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
