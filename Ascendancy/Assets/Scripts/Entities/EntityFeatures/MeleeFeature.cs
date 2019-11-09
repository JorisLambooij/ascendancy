using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeFeature", menuName = "Entity Features/Melee Feature")]
public class MeleeFeature : EntityFeature
{
    /// <summary>
    /// Melee Attack increases chance to land a melee hit.
    /// </summary>
    public int meleeAttack;

    /// <summary>
    /// Melee Defense decreases the chance for an enemy to land a melee hit.
    /// </summary>
    public int meleeDefense;

    /// <summary>
    /// The Strength of a melee attack.
    /// </summary>
    public int meleeStrength;

    /// <summary>
    /// The range of a melee attack.
    /// </summary>
    public int meleeRange;

    /// <summary>
    /// How long it takes to perform one melee attack.
    /// </summary>
    public float attackSpeed;

    /// <summary>
    /// List of special abilities.
    /// </summary>
    //public List<>

    public override bool ClickOrder(Entity entity, RaycastHit hit, bool enqueue = false)
    {
        if (hit.collider == null)
        {
            Debug.LogError("No hit.collider!");
            return false;
        }

        switch (hit.collider.tag)
        {
            case ("Unit"):
                Unit targetU = hit.collider.GetComponentInParent<Unit>();
                MeleeAttackOrder attackOrderU = new MeleeAttackOrder(entity, targetU);
                entity.IssueOrder(attackOrderU, enqueue);
                return true;
            case ("Building"):
                Building targetB = hit.collider.GetComponentInParent<Building>();
                MeleeAttackOrder attackOrderB = new MeleeAttackOrder(entity, targetB);
                entity.IssueOrder(attackOrderB, enqueue);
                return true;

            default:
                //Unknown tag
                Debug.Log("Unknown tag hit with ray cast: tag '" + entity.tag + "' in " + hit.collider.ToString());
                entity.Controller.orders.Clear();
                return false;
        }
    }
}
