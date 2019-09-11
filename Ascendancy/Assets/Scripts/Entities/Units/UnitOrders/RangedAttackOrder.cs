using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackOrder : UnitOrder
{
    public RangedAttackOrder(Unit unit, Entity target, bool guardMode = false) : base(unit)
    {

    }

    public override Vector3 CurrentDestination => throw new System.NotImplementedException();

    public override bool Fulfilled => throw new System.NotImplementedException();
}
