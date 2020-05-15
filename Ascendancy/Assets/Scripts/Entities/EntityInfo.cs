using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is this a unit that needs to be recruited, or a building that can be built directly?
/// </summary>
public enum ConstructionMethod { Unit, Building };

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

    /// <summary>
    /// The size (measured in Tiles) of this Entity.
    /// </summary>
    public Vector2Int dimensions;

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
    public ConstructionMethod Construction_Method;

    /// <summary>
    /// What category of Entity is this? (E.g. production, military, cavalry etc.)
    /// </summary>
    public string Category;

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
        GameObject prefab;
        Transform targetParent;
        if (Construction_Method == ConstructionMethod.Building)
        {
            targetParent = owner.BuildingsGO.transform;
            prefab = Resources.Load("Prefabs/Entities/Building Prefab") as GameObject;
        }
        else
        {
            targetParent = owner.UnitsGO.transform;
            prefab = Resources.Load("Prefabs/Entities/Unit Prefab") as GameObject;
        }

        GameObject go = Instantiate(prefab, targetParent);
        go.transform.position = position;

        go.GetComponentInChildren<MeshFilter>().mesh = Mesh;
        go.GetComponent<Entity>().entityInfo = this;

        go.name = name;

        return go;
    }
}
