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

    public override bool Fulfilled
    {
        get
        {
            float distance = Vector3.Distance(unit.transform.position, destination);
            return distance < 0.5f;
        }
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

    public override bool Fulfilled
    {
        get
        {
            float targetHealth = target.CurrentHealth;
            return targetHealth <= 0;
        }
    }
}