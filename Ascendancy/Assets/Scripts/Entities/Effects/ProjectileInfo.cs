using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileInfo
{
    [Tooltip("The Initial speed at which this Projectile is launched at."), Min(1)]
    public float horizontalVelocity;

    [Tooltip("The height of the arc this Projectile is travelling at."), Min(0)]
    public float arcHeight;
    
    [Tooltip("How strong should the homing effect be"), Min(0)]
    public float targetSeekingCoefficient;
    
    [Tooltip("The amount of time that this Projectile will exist"), Min(0)]
    public float lifeTime;
    
    [Tooltip("The damage composition of this Projectile")]
    public AttackStrength attackStrength;

    [Tooltip("Can this Projectile be shot down")]
    public bool interceptable;

    [Tooltip("How much this Projectile can pierce through Entities"), Min(1)]
    public float piercingPower;

    [Tooltip("Does this Projectile explode on death")]
    public bool explosive;
}
