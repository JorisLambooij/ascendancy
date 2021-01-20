using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The master class for Units.
/// </summary>
public class Unit : Entity
{
    //private UnitController controller;

    //private Sprite minimapMarker;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        currentHealth = entityInfo.maxHealth;

        foreach (EntityFeature feature in entityInfo.entityFeatures)
            feature.Initialize(this);

        //unit_features?
    }

    // Update is called once per frame
    protected override void Update()
    {
        foreach (EntityFeature feature in entityInfo.entityFeatures)
            feature.UpdateOverride();
    }

    /// <summary>
    /// When a click is registered, each unit will individually determine what to do with the order.
    /// </summary>
    /// <param name="hit">The RayCastHit from the Camera.</param>
    /// <param name="enqueue">Whether or not the order should be placed in the queue (true), or replace the current order queue entirely (false).</param>
    public override void ClickOrder(RaycastHit hit, bool enqueue)
    {
        if (hit.collider == null)
        {
            Debug.LogError("No hit.collider!");
            return;
        }

        switch (hit.collider.tag)
        {
            case ("Unit"):
            case ("Building"):
                Entity target = hit.collider.GetComponentInParent<Entity>();
                MeleeAttackOrder attackOrder = new MeleeAttackOrder(this, target);
                IssueOrder(attackOrder, enqueue);
                break;
            case ("Ground"):
                MoveOrder moveOrder = new MoveOrder(this, hit.point);
                IssueOrder(moveOrder, enqueue);
                break;

            default:
                //Unknown tag
                Debug.Log("Unknown tag hit with ray cast: tag '" + tag + "' in "+hit.collider.ToString());
                controller.orders.Clear();
                break;
        }
    }

    /// <summary>
    /// Relay an order to this Unit.
    /// </summary>
    /// <param name="order">The order that is being issued.</param>
    /// <param name="enqueue">Whether the order should be queued or replace the current order queue. </param>
    public void IssueOrder(UnitOrder order, bool enqueue)
    {
        if (enqueue)
            controller.orders.Enqueue(order);
        else
        {
            controller.orders.Clear();
            controller.NewOrder(order);
        }
    }

    public EntityOrderController Controller
    {
        get { return controller; }
    }
}
