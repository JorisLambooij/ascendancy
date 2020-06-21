using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAuraFeature", menuName = "Entity Features/Aura Feature")]
public class AuraFeature : EntityFeature
{
    public List<AuraEffect> effects;
    public float radius;

    public override void Initialize(Entity entity)
    {
        base.Initialize(entity);
        foreach(AuraEffect effect in effects)
        {
            effect.auraFeature = this;
            entity.StartCoroutine(effect.UpdateCycle());
        }
    }
}
