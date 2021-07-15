using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float duration;

    public void Explode(ExplodingProjectileInfo info, Entity launcher = null)
    {
        StartCoroutine(PlayParticles());
        AudioSource sound = GetComponent<AudioSource>();
        if (sound != null)
            sound.Play();

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

    IEnumerator PlayParticles()
    {
        ParticleSystem pSystem = GetComponent<ParticleSystem>();
        pSystem.Play();
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
}
