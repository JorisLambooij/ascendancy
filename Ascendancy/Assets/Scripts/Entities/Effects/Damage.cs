using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DamageType { Physical, Energy }

[System.Serializable]
public struct DamageAmount
{
    public DamageType type;

    public float APAmount;
    public float nonAPAmount;
}

[System.Serializable]
public struct AttackStrength
{
    public List<DamageAmount> damageComposition;

    public AttackStrength(List<DamageAmount> damageComposition)
    {
        this.damageComposition = damageComposition;
    }

    public AttackStrength MultiplyDamage(float factor)
    {
        List<DamageAmount> newComposition = new List<DamageAmount>(damageComposition.Count);
        for (int i = 0; i < damageComposition.Count; i++)
        {
            DamageAmount amount = damageComposition[i];
            amount.APAmount *= factor;
            amount.nonAPAmount *= factor;
            newComposition.Add(amount);
        }
        AttackStrength returnValue = new AttackStrength(newComposition);
        return returnValue;
    }
}
