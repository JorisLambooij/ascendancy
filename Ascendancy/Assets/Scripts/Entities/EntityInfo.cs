using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityInfo", menuName = "Entity")]
public class EntityInfo : ScriptableObject
{
    [Header("Core Info")]
    /// <summary>
    /// The name of this entity type.
    /// </summary>
    new public string name;

    /// <summary>
    /// Short description.
    /// </summary>
    public string Description;

    /// <summary>
    /// Maximum Health of this entity.
    /// </summary>
    public int MaxHealth;

    /// <summary>
    /// Armor will reduce incoming damage.
    /// </summary>
    public int Armor;

    [Header("Visual Data")]
    /// <summary>
    /// The Prefab used to instantiate this entity.
    /// </summary>
    public GameObject Prefab;

    /// <summary>
    /// entity Thumbnail.
    /// </summary>
    public Sprite Thumbnail;

    /// <summary>
    /// The Sprite used for the minimap.
    /// NULL means default marker.
    /// </summary>
    public Sprite MinimapMarker;

    [Header("Technical")]
    /// <summary>
    /// entity view distance in tiles.
    /// </summary>
    public float ViewDistance;

    /// <summary>
    /// How many options the context menu has for this entity.
    /// </summary>
    public int ContextMenuOptions;

    /// <summary>
    /// Only entities of the highest selectionPriority will be selected when dragging the mouse.
    /// </summary>
    public int selectionPriority;

    /// <summary>
    /// Base cost of the entity.
    /// </summary>
    public List<Resource_Amount> ResourceAmount;

    /// <summary>
    /// Time needed to build the entity in seconds.
    /// </summary>
    public float BuildTime;

    /// <summary>
    /// List of all EntityFeatures.
    /// </summary>
    public List<EntityFeature> EntityFeatures;
}
