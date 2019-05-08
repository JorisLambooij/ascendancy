using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRotator : MonoBehaviour
{
    private Vector3 lookDirection;
    private bool active;

    public float maxAngularSpeed;
    
    void Update()
    {
        if (active)
        {
            RotateTowards(lookDirection);

            if (Vector3.Angle(lookDirection, transform.forward) < 6)
                active = false;
        }
    }

    public void LookAt(Vector3 lookDirection)
    {
        this.lookDirection = lookDirection;
        active = true;
    }

    private void RotateTowards(Vector3 direction)
    {
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * maxAngularSpeed);
    }
}
