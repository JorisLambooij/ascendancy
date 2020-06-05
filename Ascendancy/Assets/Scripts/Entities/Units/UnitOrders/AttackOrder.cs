using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackOrder : UnitOrder
{
    /// <summary>
    /// The target of the attack Order.
    /// </summary>
    protected Entity target;

    /// <summary>
    /// The Movement-EntityFeature that enables this Entity to move. If not found, Entity still will be able to defend its melee range.
    /// </summary>
    protected MovementFeature moveFeature;

    /// <summary>
    /// Guard Mode means the unit will not follow the target when it gets out range.
    /// </summary>
    protected bool guardMode;

    public AttackOrder(Entity entity, Entity target, bool guardMode = false) : base(entity)
    {
        Debug.Log("Attack Order");
        this.target = target;
        this.guardMode = guardMode;
        this.moveFeature = entity.FindFeature<MovementFeature>();
    }

    public override Vector3 CurrentDestination
    {
        get { return target.transform.position; }
    }
    
    public override bool Fulfilled
    {
        get
        {
            float targetHealth = target.Health;
            return targetHealth <= 0 || (guardMode && !IsInRange);
        }
    }
    
    public void SetTarget(Entity e)
    {
        target = e;
    }

    protected abstract bool IsInRange { get; }
}
