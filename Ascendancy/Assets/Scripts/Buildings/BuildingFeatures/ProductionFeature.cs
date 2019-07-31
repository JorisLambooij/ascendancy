using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionFeature : BuildingFeature
{
    public Resource producedResource;
    public float producedAmount;

    public Resource consumedResource;
    public float consumedAmount;

    private bool Produce(Player owner)
    {
        if (consumedResource != null)
        {
            if (owner.economy.resourceStorage[consumedResource] > consumedAmount)
                owner.economy.resourceStorage[consumedResource] -= consumedAmount;
            else
                // not enough of the needed Resource, so don't produce anything
                return false;
        }

        owner.economy.resourceStorage[producedResource] += producedAmount;
        return true;
    }
}
