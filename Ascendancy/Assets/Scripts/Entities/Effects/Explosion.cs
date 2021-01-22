using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void Explode(ExplodingProjectileInfo info, Entity launcher = null)
    {
        float duration = GetComponent<ParticleSystem>().main.duration;
        duration -= 0.05f;
        if (duration <= 0)
        {
            Debug.LogError("Explosion Duration too short!");
            duration = 0.05f;
        }
        Destroy(this.gameObject, duration);

        // Get all colliders in explosion radius
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, info.explosionRadius);
        foreach (Collider coll in collidersInRange)
        {
            // only process if the collider has an Entity attached
            Entity e = coll.GetComponentInParent<Entity>();
            if (e != null && e != launcher)
            {
                float distance = Vector3.Distance(e.transform.position, transform.position);
                float dmgFalloffFactor = Mathf.Pow(1 - distance / info.explosionRadius, info.damageFalloff);
                DamageComposition modifiedDamage = info.explosionDamage.MultiplyDamage(dmgFalloffFactor);
                e.TakeDamage(modifiedDamage);
            }
        }

    }
}
