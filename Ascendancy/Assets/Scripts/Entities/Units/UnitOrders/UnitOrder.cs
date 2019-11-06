using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the abstract superclass for all types of Unit Orders.
/// </summary>
public abstract class UnitOrder
{
    /// <summary>
    /// The Entity this Order is associated with.
    /// </summary>
    protected Entity entity;

    /// <summary>
    /// Some Orders should be repeated in certain periods. This is the timer variable that enables that.
    /// </summary>
    protected float cooldown;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="entity">The Unit this Order is issued to.</param>
    public UnitOrder(Entity entity)
    {
        this.entity = entity;
    }

    /// <summary>
    /// The destination, derived from the other properties.
    /// </summary>
    public abstract Vector3 CurrentDestination
    {
        get;
    }

    /// <summary>
    /// Initialize this Order.
    /// </summary>
    public virtual void Execute()
    {

    }

    /// <summary>
    /// Some Orders will need continuous Updating (following enemies, building something etc.)
    /// </summary>
    public virtual void Update()
    {

    }

    /// <summary>
    /// Whether or not the Order has been fulfilled.
    /// </summary>
    public abstract bool Fulfilled
    {
        get;
    }
}
