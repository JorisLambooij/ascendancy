using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPortalFeature", menuName = "Building Features/Portal Feature", order = 4)]
public class PortalFeature : BuildingFeature
{
    public Vector3 portalOffset;
    public Building partnerPortal;

    public override void Initialize(Building building)
    {
        
    }

    public override void UpdateOverride(Building building)
    {
        
    }

    public void TeleportUnit(Unit u)
    {
        u.transform.position = partnerPortal.transform.position;
    }
}
