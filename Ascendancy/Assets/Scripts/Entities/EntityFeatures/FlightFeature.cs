using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewFlightFeature", menuName = "Entity Features/Flight Feature", order = 0)]
public class FlightFeature : EntityFeature
{
    public float flyingSpeed;
    public float angularFlyingSpeed;

    public override void UpdateOverride()
    {
        base.UpdateOverride();
    }

    public override bool ClickOrder(RaycastHit hit, bool enqueue = false, bool ctrl = false)
    {
        // ctrl means special order, so no movement
        if (ctrl)
            return false;

        if (hit.collider == null)
        {
            Debug.LogError("No hit.collider!");
            return false;
        }

        MoveOrder moveOrder;
        switch (hit.collider.tag)
        {
            case ("Unit"):
            case ("Building"):
                moveOrder = new FlightOrder(entity, hit.collider.transform.position);
                entity.IssueOrder(moveOrder, enqueue);
                return true;
            case ("Ground"):
                moveOrder = new FlightOrder(entity, hit.point);
                entity.IssueOrder(moveOrder, enqueue);
                return true;

            default:
                //Unknown tag
                Debug.Log("Unknown tag hit with ray cast: tag '" + entity.tag + "' in " + hit.collider.ToString());
                entity.Controller.orders.Clear();
                return false;
        }
    }
}
