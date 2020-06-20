using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "NewProjectileInfo", menuName = "Projectiles/Regular")]
public class ProjectileInfo : ScriptableObject
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
    public DamageComposition rangedDamage;

    [Tooltip("Can this Projectile be shot down")]
    public bool interceptable;

    [Tooltip("How much this Projectile can pierce through Entities"), Min(1)]
    public float piercingPower;

    [Tooltip("The Model that this Projectile uses.")]
    public Mesh projectileModel;


#if UNITY_EDITOR
    public virtual void DoAdditionalLayout() { }
#endif
}
