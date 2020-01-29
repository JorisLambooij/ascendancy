using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewMovementFeature", menuName = "Entity Features/Movement Feature", order = 0)]
public class MovementFeature : EntityFeature
{
    public float speed;

    public float turnSpeed;

    public override void Initialize(Entity entity)
    {
        base.Initialize(entity);
    }

    public override void UpdateOverride()
    {
        base.UpdateOverride();
    }

    public override bool ClickOrder(RaycastHit hit, bool enqueue = false)
    {
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
                moveOrder = new MoveOrder(entity, hit.collider.transform.position);
                entity.IssueOrder(moveOrder, enqueue);
                return true;
            case ("Ground"):
                moveOrder = new MoveOrder(entity, hit.point);
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
