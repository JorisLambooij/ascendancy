using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitOrder
{
    protected Unit unit;

    public UnitOrder(Unit unit)
    {
        this.unit = unit;
    }

    public abstract Vector3 CurrentDestination
    {
        get;
    }

    public abstract void Execute();

    public abstract bool Fulfilled
    {
        get;
    }
}

public class MoveOrder : UnitOrder
{
    private Vector3 destination;

    public MoveOrder(Unit unit, Vector3 destination) : base(unit)
    {
        this.destination = destination;
    }
    
    public override Vector3 CurrentDestination
    {
        get { return destination; }
    }

    public void SetDestination(Vector3 dest)
    {
        destination = dest;
    }

    public override void Execute()
    {
        unit.Controller.NavAgent.SetDestination(CurrentDestination);
    }

    public override bool Fulfilled
    {
        get { return unit.Controller.NavAgent.remainingDistance == 0;  }
    }


}

public class RotateOrder : UnitOrder
{
    private Vector3 orientation;

    public RotateOrder(Unit unit, Vector3 orientation) : base(unit)
    {
        this.orientation = orientation;
    }

    public override Vector3 CurrentDestination
    {
        get { return unit.transform.position; }
    }

    public override bool Fulfilled
    {
        get { return Vector3.Angle(unit.transform.forward, orientation) < 6; }
    }

    public override void Execute()
    {
        unit.transform.GetComponent<UnitRotator>().LookAt(orientation);
    }

    public Vector3 TargetOrientation
    {
        get { return orientation; }
    }
}

public class AttackOrder : UnitOrder
{
    private Unit target;

    public AttackOrder(Unit unit, Unit target) : base (unit)
    {
        this.target = target;
    }

    public override Vector3 CurrentDestination
    {
        get { return target.transform.position; }
    }

    public void SetTarget(Unit u)
    {
        target = u;
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public override bool Fulfilled
    {
        get
        {
            float targetHealth = target.CurrentHealth;
            return targetHealth <= 0;
        }
    }
}