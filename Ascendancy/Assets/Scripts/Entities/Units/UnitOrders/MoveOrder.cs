using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOrder : UnitOrder
{
    /// <summary>
    /// The destination the unit should go to.
    /// </summary>
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
        unit.Controller.NavAgent.isStopped = false;
    }

    public override bool Fulfilled
    {
        get { return unit.Controller.NavAgent.remainingDistance == 0; }
    }
}

