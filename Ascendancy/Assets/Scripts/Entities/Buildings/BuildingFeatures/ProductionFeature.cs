using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewProductionFeature", menuName = "Building Features/Resource Production Feature")]
public class ProductionFeature : BuildingFeature
{
    public Resource producedResource;
    public float producedAmount;

    public Resource consumedResource;
    public float consumedAmount;

    private float countdown;
    private const float PROD_TIME = 3;

    public override void Initialize(Building building)
    {
        
    }

    public override void UpdateOverride(Building building)
    {
        if (countdown < 0)
        {
            countdown = PROD_TIME;
            Produce(building.Owner);
        }
        countdown -= Time.deltaTime;
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
            Debug.Log("Resource " + producedResource.resourceName + " not unlocked.");
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

    private IEnumerator Countdown(Building building)
    {
        float duration = 1f;
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        Produce(building.Owner);
        Countdown(building);
    }
}
