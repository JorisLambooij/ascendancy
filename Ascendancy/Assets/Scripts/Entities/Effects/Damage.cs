using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DamageType { Physical, Energy }

[System.Serializable]
public struct DamageAmount
{
    public DamageType type;

    public float amount;

    public bool armorPiercing;
}

[System.Serializable]
public struct AttackStrength
{
    public List<DamageAmount> damageComposition;
}
