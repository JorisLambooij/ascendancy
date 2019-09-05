using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object to hold basic Unit stats.
/// </summary>
[CreateAssetMenu(fileName = "New Unit", menuName = "Unit SO", order = 0)]
public class UnitInfo : ScriptableObject, IEntityInfo
{

    /// <summary>
    /// Short description.
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

    /// <summary>
    /// Unit view distance in half tiles.
    /// </summary>
    [Range(0, 100)]
    public int viewDistance = 10;

    /// <summary>
    /// Base cost of the unit.
    /// </summary>
    public List<Resource_Amount> resource_amount;

    /// <summary>
    /// Time needed to build the unit in seconds.
    /// </summary>
    public float build_time;


    //implementation if IEntityInfo
    public string Name
    {
        get { return unitName; }
        set { }
    }

    public string Description
    {
        get { return description; }
        set { }
    }

    public int MaxHealth
    {
        get { return maxHealth; }
        set { }
    }

    public GameObject Prefab
    {
        get { return prefab; }
        set { }
    }

    public Sprite Thumbnail
    {
        get { return thumbnail; }
        set { }
    }

    public int ContextMenuOptions
    {
        get { return contextMenuOptions; }
        set { }
    }

    public Sprite MinimapMarker
    {
        get { return minimapMarker; }
        set { }
    }

    public int ViewDistance
    {
        get { return viewDistance; }
        set { }
    }

    public List<Resource_Amount> Resource_amount
    {
        get { return resource_amount; }
        set { }
    }

    public float Build_time
    {
        get { return build_time; }
        set { }
    }
}
