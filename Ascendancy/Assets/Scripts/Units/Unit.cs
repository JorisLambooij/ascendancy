using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitInfo unitInfo;
    public int owner = 1;

    private float currentHealth;
    private UnitController controller;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = unitInfo.maxHealth;
        controller = GetComponent<UnitController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// When a click is registered, each unit will individually determine what to do with the order.
    /// </summary>
    /// <param name="hit">The RayCastHit from the Camera.</param>
    /// <param name="enqueue">Whether or not the order should be placed in the queue (true), or replace the current order queue entirely (false).</param>
    public void ClickOrder(RaycastHit hit, bool enqueue)
    {
        switch (hit.collider.tag)
        {
            case ("Unit"):
                Unit target = hit.collider.GetComponentInParent<Unit>();
                AttackOrder attackOrder = new AttackOrder(this, target);
                IssueOrder(attackOrder, enqueue);
                break;
            case ("Ground"):
                MoveOrder moveOrder = new MoveOrder(this, hit.point);
                IssueOrder(moveOrder, enqueue);
                break;
            default:
                //Unknown tag
                Debug.Log("Unknown tag hit with ray cast");
                controller.orders.Clear();
                break;
        }
    }

    private void IssueOrder(UnitOrder order, bool enqueue)
    {
        if (enqueue)
            controller.orders.Enqueue(order);
        else
        {
            controller.orders.Clear();
            controller.NewOrder(order);
        }
    }

    public float CurrentHealth
    {
        get { return currentHealth; }
    }

    /*
    public int Owner
    {
        get { return owner; }
        set { owner = value; }
    }
    */
}
