using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QueueObject
{
    public EntityInfo BaseUnit { get; }
    public List<Resource_Amount> PaidCost { get; }

    public QueueObject( EntityInfo baseUnit, List<Resource_Amount> paidCost)
    {
        BaseUnit = baseUnit;
        PaidCost = paidCost;
    }
}
