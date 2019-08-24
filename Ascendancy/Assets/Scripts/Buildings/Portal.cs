using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Building
{
    public Portal partnerPortal;
    public Transform unitExit;

    protected override void Start()
    {
        base.Start();

        // if a partner portal is set up, link the two on both ends
        if (partnerPortal != null)
            LinkPortal(partnerPortal);
    }

    protected override void Update()
    {
        base.Update();
    }

    public void TeleportUnit(Unit u)
    {
        MoveOrder order = new MoveOrder(u, partnerPortal.unitExit.position);
        u.IssueOrder(order, false);
        u.transform.position = partnerPortal.unitExit.position;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Unit"))
        {
            Unit u = collider.transform.GetComponentInParent<Unit>();
            TeleportUnit(u);
        }
    }

    public void LinkPortal(Portal otherPortal)
    {
        Debug.Log("Link");

        partnerPortal = otherPortal;
        otherPortal.partnerPortal = this;

        GetComponentInChildren<PortalView>().LinkPortalView();
        otherPortal.GetComponentInChildren<PortalView>().LinkPortalView();
    }
}
