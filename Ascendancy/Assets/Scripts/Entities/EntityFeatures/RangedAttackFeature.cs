using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRangedFeature", menuName = "Entity Features/Ranged Attack Feature")]
public class RangedAttackFeature : EntityFeature
{
    [Tooltip("Minimum distance between this Entity and its target"), Min(0)]
    public int minRange;
   
    [Tooltip("Maximum distance between this Entity and its target")]
    public int maxRange;
    
    [Tooltip("Time inbetween attacks")]
    public float rangedAttackCooldown;

    [Tooltip("Amount of burst shots per volley"), Min(1)]
    public int volley;

    [Tooltip("Time in takes the Entity to shoot one volley"), Min(0)]
    public float volleyDuration;

    [Tooltip("Inaccuracy of the ranged attack.")]
    public float inaccuracy;

    [Tooltip("Properties of the projectiles")]
    public ProjectileInfo projectileInfo;

    public override bool ClickOrder(RaycastHit hit, bool enqueue = false)
    {
        if (hit.collider == null)
        {
            Debug.LogError("No hit.collider!");
            return false;
        }

        switch (hit.collider.tag)
        {
            case ("Unit"):
            case ("Building"):
            case ("Entity"):
                Entity target = hit.collider.GetComponentInParent<Entity>();
                RangedAttackOrder attackOrder = new RangedAttackOrder(entity, target);
                entity.IssueOrder(attackOrder, enqueue);
                return true;
            case ("Ground"):
                MoveOrder moveOrder = new MoveOrder(entity, hit.point);
                entity.IssueOrder(moveOrder, enqueue);
                return true;
            default:
                //Unknown tag
                Debug.Log("Unknown tag hit with ray cast: tag '" + hit.collider.tag + "' in " + hit.collider.ToString());

                Debug.Log(entity.Controller);
                entity.Controller.orders.Clear();
                return false;
        }
    }
    
}
