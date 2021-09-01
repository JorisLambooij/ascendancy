using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOrder : UnitOrder
{
    protected const float MAX_REMAINING_DISTANCE = 0.2f;

    /// <summary>
    /// The destination the unit should go to.
    /// </summary>
    protected Vector3 destination;

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
        if (entity.Controller.NavAgent == null)
        {
            Debug.LogError("MoveOrder issued to Entity without a NavMeshAgent");
            return;
        }

        entity.Controller.NavAgent.SetDestination(CurrentDestination);
        entity.Controller.NavAgent.isStopped = false;
    }

    public override bool Fulfilled
    {
        get 
        {
            if (entity.entityInfo.construction_Method == ConstructionMethod.Building || entity.entityInfo.construction_Method == ConstructionMethod.SpecialBuilding)
            {
                Debug.Log("Cancelling move order for building '" + entity.entityInfo.name + "'");
                return true;
            }

            if (entity.Controller.NavAgent.pathPending)
                return false;
            return entity.Controller.NavAgent.remainingDistance <= MAX_REMAINING_DISTANCE;
        }
    }

}

