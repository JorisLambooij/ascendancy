using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeFeature", menuName = "Entity Features/Melee Feature")]
public class MeleeFeature : EntityFeature
{
    [Tooltip("Melee Attack increases chance to land a melee hit.")]
    public int meleeAttack;
    
    [Tooltip("Melee Defense decreases the chance for an enemy to land a melee hit.")]
    public int meleeDefense;
    
    [Tooltip("The Strength of a melee attack.")]
    public DamageComposition meleeDamage;
    
    [Tooltip("The range of a melee attack.")]
    public int meleeRange;
    
    [Tooltip("How long it takes to perform one melee attack.")]
    public float meleeAttackSpeed;

    /// <summary>
    /// List of special abilities.
    /// </summary>
    //public List<>

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
                Entity target = hit.collider.GetComponentInParent<Entity>();
                MeleeAttackOrder attackOrder = new MeleeAttackOrder(entity, target);
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
