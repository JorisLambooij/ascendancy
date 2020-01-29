using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightOrder : MoveOrder
{
    public FlightOrder(Entity entity, Vector3 destination) : base(entity, destination)
    {
        this.destination = destination;
    }

    public override void Execute()
    {
        Rigidbody rb = entity.transform.GetComponent<Rigidbody>();
        Transform t = entity.transform;

        float speed = entity.FindFeature<FlightFeature>().flyingSpeed;
        float angularSpeed = entity.FindFeature<FlightFeature>().angularFlyingSpeed;

        Vector3 targetVector = destination - t.position;
        float lookAngle = Vector3.Angle(t.forward, targetVector);

        if (lookAngle < 30)
        {
            // Make it so that we don't overshoot our target
            float distance = targetVector.magnitude;
            float movementDistance = Mathf.Min(distance, speed);

            Vector3 movement = Time.deltaTime * movementDistance * targetVector.normalized;
            rb.MovePosition(t.position + movement);
        }
    }
}
