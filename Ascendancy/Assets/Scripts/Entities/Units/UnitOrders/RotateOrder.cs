using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOrder : UnitOrder
{
    private Vector3 orientation;

    public RotateOrder(Entity entity, Vector3 orientation) : base(entity)
    {
        this.orientation = orientation;
    }

    public override Vector3 CurrentDestination
    {
        get { return entity.transform.position; }
    }

    public override bool Fulfilled
    {
        get { return Vector3.Angle(entity.transform.forward, orientation) < 6; }
    }
    
    public override void Update()
    {
        //entity.transform.GetComponent<UnitRotator>().RotateTowards(orientation);
    }

    public Vector3 TargetOrientation
    {
        get { return orientation; }
    }
}
