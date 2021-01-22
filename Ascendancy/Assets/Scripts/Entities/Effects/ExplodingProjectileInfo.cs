using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "NewProjectileInfo", menuName = "Projectiles/Explosive")]
public class ExplodingProjectileInfo : ProjectileInfo
{
    [Tooltip("The amount of damage that is dealt at the center point.")]
    public DamageComposition explosionDamage;

    [Tooltip("The explosion radius of this Projectile. Leave at 0 for no explosion."), Min(0)]
    public float explosionRadius;

    [Tooltip("Controls how much the damage is reduced by the distance to the center. 0 means no falloff"), Min(0)]
    public float damageFalloff;

    [Tooltip("The explosion effect that should play when this Projectile is destroyed.")]
    public GameObject explosionEffect;


#if UNITY_EDITOR
    public override void DoAdditionalLayout()
    {
        //explosionDamage = EditorGUILayout.PropertyField(explosionDamage);
        //explosionDamage = EditorGUILayout.FloatField("Explosion Damage", explosionDamage);
        //explosionRadius = EditorGUILayout.FloatField("Explosion Radius", explosionRadius);
    }
#endif
}
