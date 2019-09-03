using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The master class for Units.
/// </summary>
public class Unit : Entity
{
    /// <summary>
    /// Holds all the stats for this Unit.
    /// </summary>
    public UnitInfo unitInfo;
    private UnitController controller;

    private Sprite minimapMarker;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        currentHealth = unitInfo.maxHealth;
        controller = GetComponent<UnitController>();
        minimapMarker = unitInfo.minimapMarker;

        GameObject markerObject = Resources.Load("Sprites/MinimapMarker") as GameObject;

        //if a sprite was provided, we use it while keeping the position and settings
        if (minimapMarker != null)
        {
            markerObject.GetComponent<SpriteRenderer>().sprite = minimapMarker;
            Debug.Log(markerObject.GetComponent <SpriteRenderer > ().sprite.name);
        }

        Instantiate(markerObject, this.transform);
    }

    // Update is called once per frame
    protected override void Update()
    {

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
                Unit targetU = hit.collider.GetComponentInParent<Unit>();
                MeleeAttackOrder attackOrderU = new MeleeAttackOrder(this, targetU);
                IssueOrder(attackOrderU, enqueue);
                break;
            case ("Ground"):
                MoveOrder moveOrder = new MoveOrder(this, hit.point);
                IssueOrder(moveOrder, enqueue);
                break;
            case ("Building"):
                Building targetB = hit.collider.GetComponentInParent<Building>();
                MeleeAttackOrder attackOrderB = new MeleeAttackOrder(this, targetB);
                IssueOrder(attackOrderB, enqueue);
                break;

            default:
                //Unknown tag
                Debug.Log("Unknown tag hit with ray cast: tag '" + hit.collider.tag + "' in "+hit.collider.ToString());
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

    public UnitController Controller
    {
        get { return controller; }
    }
}
