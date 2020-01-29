using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewProductionFeature", menuName = "Entity Features/Resource Production Feature")]
public class ProductionFeature : EntityFeature
{
    public Resource producedResource;
    public float producedAmount;

    public Resource consumedResource;
    public float consumedAmount;
    
    public override void Update10Override()
    {
        Produce(entity.Owner);
    }

    private bool Produce(Player owner)
    {
        if (consumedResource != null && !owner.economy.availableResources.Contains(consumedResource))
        // Consumed Resource has not been unlocked yet
        {
            Debug.Log("Resource " + consumedResource.resourceName + " not unlocked.");
            return false;
        }
        if (producedResource != null && !owner.economy.availableResources.Contains(producedResource))
        // Produced Resource has not been unlocked yet
        {
            // Debug.Log("Resource " + producedResource.resourceName + " not unlocked.");
            return false;
        }

        if (consumedResource != null)
        {
            float inStorage = owner.economy.resourceStorage.GetValue(consumedResource);
            if (inStorage > consumedAmount)
                owner.economy.resourceStorage.SetValue(consumedResource, inStorage - consumedAmount);
            else
                // not enough of the needed Resource, so don't produce anything
                return false;
        }
        
        float producedInStorage = owner.economy.resourceStorage.GetValue(producedResource);
        owner.economy.resourceStorage.SetValue(producedResource, producedInStorage + producedAmount);
        return true;
    }
    
}
