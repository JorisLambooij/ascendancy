using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeFeature", menuName = "Entity Features/Melee Feature")]
public class MeleeFeature : EntityFeature
{
    /// <summary>
    /// Melee Attack increases chance to land a melee hit.
    /// </summary>
    public int meleeAttack;

    /// <summary>
    /// Melee Defense decreases the chance for an enemy to land a melee hit.
    /// </summary>
    public int meleeDefense;

    /// <summary>
    /// The Strength of a melee attack.
    /// </summary>
    public int meleeStrength;

    /// <summary>
    /// The range of a melee attack.
    /// </summary>
    public int meleeRange;

    /// <summary>
    /// How long it takes to perform one melee attack.
    /// </summary>
    public float attackSpeed;

    /// <summary>
    /// List of special abilities.
    /// </summary>
    //public List<>
    
}
