using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object to hold basic Unit stats.
/// </summary>
[CreateAssetMenu(fileName ="New Unit", menuName = "Unit SO", order = 0)]
public class UnitInfo : ScriptableObject
{
    /// <summary>
    /// The name of this Unit type.
    /// </summary>
    public string unitName;

    /// <summary>
    /// Short description.
    /// </summary>
    public string description;

    /// <summary>
    /// Maximum Health of this Unit.
    /// </summary>
    public int maxHealth;

    /// <summary>
    /// Armor will reduce incoming damage.
    /// </summary>
    public int armor;

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
    /// How fast this Unit is.
    /// </summary>
    public float
        speed,
        acceleration;

    /// <summary>
    /// The Prefab used to instantiate this Unit.
    /// </summary>
    public GameObject prefab;

    /// <summary>
    /// Unit Thumbnail.
    /// </summary>
    public Sprite thumbnail;

    /// <summary>
    /// How many options the context menu has for this unit.
    /// </summary>
    public int contextMenuOptions;

    /// <summary>
    /// The Sprite used for the minimap.
    /// NULL means default marker.
    /// </summary>
    public Sprite minimapMarker;
}
