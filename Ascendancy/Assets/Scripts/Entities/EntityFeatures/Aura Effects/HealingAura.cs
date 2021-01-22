using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHealAura", menuName = "Auras/Healing")]
public class HealingAura : AuraEffect
{
    public float healthRestored;

    public override void OnEffect(Entity target)
    {
        Debug.Log("Restoring Health to: " + target);
        target.TakeHealing(healthRestored);
    }
}
