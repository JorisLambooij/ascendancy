using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QueueObject
{
    public EntityInfo BaseUnit { get; }
    public List<ResourceAmount> PaidCost { get; }

    public QueueObject( EntityInfo baseUnit, List<ResourceAmount> paidCost)
    {
        BaseUnit = baseUnit;
        PaidCost = paidCost;
    }
}
