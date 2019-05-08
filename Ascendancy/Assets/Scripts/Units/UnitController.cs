using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
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
            if (orders.Count > 0)
            {
                NewOrder(orders.Dequeue());
            }
            else
                return;
        }
        
        if (currentOrder.Fulfilled)
            currentOrder = null;
    }

    public void NewOrder(UnitOrder order)
    {
        currentOrder = order;
        order.Execute();
    }

    public NavMeshAgent NavAgent
    {
        get { return navMeshAgent; }
    }
}
