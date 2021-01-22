using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AuraEffect : ScriptableObject
{
    public float updateFrequency;
    public bool affectsSelf;

    [HideInInspector]
    public AuraFeature auraFeature;

    public IEnumerator UpdateCycle()
    {
        while(true)
        {
            Collider[] collidersInRange = Physics.OverlapSphere(auraFeature.entity.transform.position, auraFeature.radius);

            foreach (Collider coll in collidersInRange)
            {
                // only process if the collider has an Entity attached
                Entity e = coll.GetComponentInParent<Entity>();
                // 
                if (e != null && (e != CastingEntity || affectsSelf))
                {
                    OnEffect(e);
                }
            }
            yield return new WaitForSeconds(updateFrequency);
        }
    }

    public abstract void OnEffect(Entity target);

    protected Entity CastingEntity
    {
        get { return auraFeature.entity; }
    }
}
