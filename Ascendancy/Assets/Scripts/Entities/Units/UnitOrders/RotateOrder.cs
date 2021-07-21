using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        get
        {
            if (entity.entityInfo.construction_Method == ConstructionMethod.Building)
            {
                Debug.Log("Cancelling rotate order for building '" + entity.entityInfo.name + "'");
                return true;
            }

            return Vector3.Angle(entity.transform.forward, orientation) < 1;
        }
    }
    
    public override void Update()
    {
        float angle = Vector3.Angle(orientation, entity.transform.forward);

        Quaternion lookRotation = Quaternion.LookRotation(orientation);
        NavMeshAgent agent = entity.GetComponent<NavMeshAgent>();
        float lerpFactor = Time.deltaTime * agent.angularSpeed / angle;
        entity.transform.rotation = Quaternion.Slerp(entity.transform.rotation, lookRotation, lerpFactor);

        //entity.transform.GetComponent<UnitRotator>().RotateTowards(orientation);
    }

    public Vector3 TargetOrientation
    {
        get { return orientation; }
    }
}
