using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRangedFeature", menuName = "Entity Features/Ranged Attack Feature")]
public class RangedAttackFeature : EntityFeature
{
    public int minRange;

    public int maxRange;
    
    public AttackStrength rangedStrength;

    public float rangedAttackSpeed;

    public int volley;

    public float accuracy;

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
            default:
                //Unknown tag
                Debug.Log("Unknown tag hit with ray cast: tag '" + entity.tag + "' in " + hit.collider.ToString());

                Debug.Log(entity.Controller);
                entity.Controller.orders.Clear();
                return false;
        }
    }
}
