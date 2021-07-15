using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ResourceAmount
{
    public Resource resource;
    public float amount;

    public ResourceAmount(Resource resource, float amount)
    {
        this.resource = resource;
        this.amount = amount;
    }
}
