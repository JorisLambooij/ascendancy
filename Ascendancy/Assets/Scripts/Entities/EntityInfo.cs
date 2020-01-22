using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Unit, Building };
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
    public Mesh Mesh;

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
    /// What type of Entity if this?
    /// </summary>
    public EntityType EntityType;

    /// <summary>
    /// Only entities of the highest selectionPriority will be selected when dragging the mouse.
    /// </summary>
    public int selectionPriority;

    [Header("Building Cost")]
    /// <summary>
    /// Time needed to build the entity in seconds.
    /// </summary>
    public float BuildTime;

    /// <summary>
    /// Base cost of the entity.
    /// </summary>
    public List<Resource_Amount> ResourceAmount;

    [Header("Features")]
    /// <summary>
    /// List of all EntityFeatures.
    /// </summary>
    public List<EntityFeature> EntityFeatures;

    public virtual GameObject CreateInstance(Player owner, Vector3 position)
    {
        GameObject prefab = Resources.Load("Prefabs/Entities/Entity Prefab") as GameObject;
        GameObject go = Instantiate(prefab, owner.transform);
        go.transform.position = position;

        go.GetComponentInChildren<MeshFilter>().mesh = Mesh;
        go.GetComponent<Entity>().entityInfo = this;

        go.name = name;

        return go;
    }
}
