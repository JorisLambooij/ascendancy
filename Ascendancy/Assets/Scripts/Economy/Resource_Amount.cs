using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Resource_Amount
{
    public Resource resource;
    public float amount;

    public Resource_Amount(Resource resource, float amount)
    {
        this.resource = resource;
        this.amount = amount;
    }
}
