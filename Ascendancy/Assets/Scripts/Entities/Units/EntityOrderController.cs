﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Unit AI
/// </summary>
public class EntityOrderController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    public Queue<UnitOrder> orders;
    public UnitOrder currentOrder;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        orders = new Queue<UnitOrder>();
        
        //navMeshAgent.SetDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentOrder == null)
        {
            if (orders == null)
            {
                Debug.LogError("orders is NULL");
            }

            if (orders.Count > 0)
                NewOrder(orders.Dequeue());
            else
                return;
        }

        if (currentOrder.Fulfilled)
            currentOrder = null;
        else
            currentOrder.Update();
    }

    public void NewOrder(UnitOrder order)
    {
        if (currentOrder != null)
            currentOrder.Cancel();
        currentOrder = order;
        order.Execute();
    }

    public NavMeshAgent NavAgent
    {
        get { return navMeshAgent; }
    }

    public void EnterMelee(Entity enemy)
    {
        if (currentOrder == null)
        {
            Entity thisUnit = GetComponent<Entity>();
            MeleeAttackOrder defendOrder = new MeleeAttackOrder(thisUnit, enemy, true);

            NewOrder(defendOrder);
        }
    }
}
