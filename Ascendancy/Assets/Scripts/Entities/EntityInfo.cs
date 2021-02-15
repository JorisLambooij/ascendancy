using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is this a unit that needs to be recruited, or a building that can be built directly?
/// </summary>
public enum ConstructionMethod { Unit, Building };

[System.Serializable]
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
    public string description;

    /// <summary>
    /// Maximum Health of this entity.
    /// </summary>
    public int maxHealth;

    /// <summary>
    /// Armor will reduce incoming damage.
    /// </summary>
    public int armor;

    /// <summary>
    /// The size (measured in Tiles) of this Entity.
    /// </summary>
    public Vector2Int dimensions;

    [Header("Visual Data")]
    /// <summary>
    /// The Prefab used to instantiate this entity.
    /// </summary>
    public GameObject prefab;

    /// <summary>
    /// entity Thumbnail.
    /// </summary>
    public Sprite thumbnail;

    /// <summary>
    /// The Sprite used for the minimap.
    /// NULL means default marker.
    /// </summary>
    public Sprite minimapMarker;

    [Header("Technical")]
    /// <summary>
    /// How good this entity is at stopping other Entities.
    /// </summary>
    public float mass;

    /// <summary>
    /// Entity view distance in tiles.
    /// </summary>
    public float viewDistance;

    /// <summary>
    /// How many options the context menu has for this entity.
    /// </summary>
    public int contextMenuOptions;

    /// <summary>
    /// What type of Entity if this?
    /// </summary>
    public ConstructionMethod construction_Method;

    /// <summary>
    /// What category of Entity is this? (E.g. production, military, cavalry etc.)
    /// </summary>
    public EntityCategoryInfo category;

    /// <summary>
    /// Only entities of the highest selectionPriority will be selected when dragging the mouse.
    /// </summary>
    public int selectionPriority = 1;

    [Header("Building Cost")]
    /// <summary>
    /// Time needed to build the entity in seconds.
    /// </summary>
    public float buildTime;

    /// <summary>
    /// Base cost of the entity.
    /// </summary>
    [SerializeField]
    public List<Resource_Amount> resourceAmount;

    [Header("Features")]
    /// <summary>
    /// List of all EntityFeatures.
    /// </summary>
    public List<EntityFeature> entityFeatures;

    public virtual GameObject CreateInstance(Player owner, Vector3 position)
    {
        GameObject entityPrefab;
        Transform targetParent;
        if (construction_Method == ConstructionMethod.Building)
        {
            targetParent = owner.BuildingsGO.transform;
            entityPrefab = Resources.Load("Prefabs/Entities/Building Prefab") as GameObject;
        }
        else
        {
            targetParent = owner.UnitsGO.transform;
            entityPrefab = Resources.Load("Prefabs/Entities/Unit Prefab") as GameObject;
        }

        GameObject go = Instantiate(entityPrefab, targetParent);
        go.transform.position = position;

        if (prefab == null)
        {
            Debug.LogError("No Prefab selected for EntityInfo " + name);
            return null;
        }

        GameObject e_model = Instantiate(prefab, go.transform);
        foreach (MeshRenderer mr in e_model.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (Material mat in mr.materials)
                if (mat.name.ToLower().Contains("playercolor"))
                    mat.SetColor("_BaseColor", owner.playerColor);
        }
        //go.GetComponentInChildren<MeshFilter>().mesh = Mesh;
        go.GetComponent<Entity>().entityInfo = this;

        go.name = name;

        return go;
    }
}
