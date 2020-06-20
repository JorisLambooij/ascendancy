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
public struct DamageComposition
{
    public List<DamageAmount> dmgComp;

    public DamageComposition(List<DamageAmount> damageComposition)
    {
        this.dmgComp = damageComposition;
    }

    public DamageComposition MultiplyDamage(float factor)
    {
        List<DamageAmount> newComposition = new List<DamageAmount>(dmgComp.Count);
        for (int i = 0; i < dmgComp.Count; i++)
        {
            DamageAmount amount = dmgComp[i];
            amount.APAmount *= factor;
            amount.nonAPAmount *= factor;
            newComposition.Add(amount);
        }
        DamageComposition returnValue = new DamageComposition(newComposition);
        return returnValue;
    }
}
