using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuilding", menuName = "Building SO", order = 2)]
public class BuildingInfo : ScriptableObject, IEntityInfo
{
    /// <summary>
    /// Short description.
    /// </summary>
    public string buildingName;

    /// <summary>
    /// Short description.
    /// </summary>
    public string description;

    //public float goldCost;

    public int maxHealth;

    public List<BuildingFeature> features;

    /// <summary>
    /// Base cost of the unit.
    /// </summary>
    public List<Resource_Amount> resource_amount;

    /// <summary>
    /// Time needed to build the unit in seconds.
    /// </summary>
    public float build_time;

    /// <summary>
    /// The Prefab used to instantiate this Unit.
    /// </summary>
    public GameObject prefab;

    /// <summary>
    /// Unit Thumbnail.
    /// </summary>
    public Sprite thumbnail;

    /// <summary>
    /// How many options the context menu has for this building.
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

    //implementation if IEntityInfo
    public string Name
    {
        get { return buildingName; }
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
