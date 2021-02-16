using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class can be used to rotate a Unit to face a certain direction.
/// </summary>
public class UnitRotator : MonoBehaviour
{
    public float allowedAngleForMovement;

    public float maxAngularSpeed;
    
    public void RotateTowards(Vector3 direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * maxAngularSpeed);

        GetComponent<Entity>().Controller.NavAgent.isStopped = Vector3.Angle(transform.forward, direction) < allowedAngleForMovement;
    }
}
