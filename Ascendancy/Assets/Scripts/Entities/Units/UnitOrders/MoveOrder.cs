using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOrder : UnitOrder
{
    /// <summary>
    /// The destination the unit should go to.
    /// </summary>
    private Vector3 destination;

    /// <summary>
    /// The EntityFeature that allows thisEntity to move.
    /// </summary>
    //private MovementFeature movementFeature;
    
    public MoveOrder(Entity entity, Vector3 destination) : base(entity)
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
        entity.Controller.NavAgent.SetDestination(CurrentDestination);
        entity.Controller.NavAgent.isStopped = false;
    }

    public override bool Fulfilled
    {
        get { return entity.Controller.NavAgent.remainingDistance == 0; }
    }

}

