using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityInfo
{
    /// <summary>
    /// The name of this entity type.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Short description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Maximum Health of this entity.
    /// </summary>
    int MaxHealth { get; }

    /// <summary>
    /// The Prefab used to instantiate this entity.
    /// </summary>
    GameObject Prefab { get; }

    /// <summary>
    /// entity Thumbnail.
    /// </summary>
    Sprite Thumbnail { get; }

    /// <summary>
    /// How many options the context menu has for this entity.
    /// </summary>
    int ContextMenuOptions { get; }

    /// <summary>
    /// The Sprite used for the minimap.
    /// NULL means default marker.
    /// </summary>
    Sprite MinimapMarker { get; }

    /// <summary>
    /// entity view distance in half tiles.
    /// </summary>
    int ViewDistance { get; }

    /// <summary>
    /// Base cost of the entity.
    /// </summary>
    List<Resource_Amount> Resource_amount { get; }

    /// <summary>
    /// Time needed to build the entity in seconds.
    /// </summary>
    float Build_time { get; }

    List<EntityFeature> EntityFeatures { get; }

}
