using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPortalFeature", menuName = "Entity Features/Special/Portal Feature", order = 0)]
public class PortalFeature : EntityFeature
{
    public Vector3 portalOffset;
    public Building partnerPortal;


    public void TeleportUnit(Unit u)
    {
        u.transform.position = partnerPortal.transform.position;
    }
}
