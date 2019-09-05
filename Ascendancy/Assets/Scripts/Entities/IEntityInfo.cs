using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityInfo
{
    /// <summary>
    /// The name of this Unit type.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Short description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Maximum Health of this Unit.
    /// </summary>
    int MaxHealth { get; }

    /// <summary>
    /// The Prefab used to instantiate this Unit.
    /// </summary>
    GameObject Prefab { get; }

    /// <summary>
    /// Unit Thumbnail.
    /// </summary>
    Sprite Thumbnail { get; }

    /// <summary>
    /// How many options the context menu has for this unit.
    /// </summary>
    int ContextMenuOptions { get; }

    /// <summary>
    /// The Sprite used for the minimap.
    /// NULL means default marker.
    /// </summary>
    Sprite MinimapMarker { get; }

    /// <summary>
    /// Unit view distance in half tiles.
    /// </summary>
    int ViewDistance { get; }

    /// <summary>
    /// Base cost of the unit.
    /// </summary>
    List<Resource_Amount> Resource_amount { get; }

    /// <summary>
    /// Time needed to build the unit in seconds.
    /// </summary>
    float Build_time { get; }

}
