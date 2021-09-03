using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSegmentFeature", menuName = "Entity Features/Segmentation Feature")]
public class SegmentationFeature : EntityFeature
{
    Vector2Int worldCoordinates;

    public GameObject cardinalSegmentPrefab;
    public GameObject diagonalSegmentPrefab;
    public List<EntityInfo> allowedConnections;

    protected Transform segmentsParent;
    protected Dictionary<string, Entity> neighbors;
    protected Dictionary<string, GameObject> segments;

    protected Dictionary<string, Vector2Int> compassCardinal = new Dictionary<string, Vector2Int>()
    {
        { "N", Vector2Int.up },
        { "E", Vector2Int.right },
        { "S", Vector2Int.down },
        { "W", Vector2Int.left }
    };
    protected Dictionary<string, Vector2Int> compassOrdinal = new Dictionary<string, Vector2Int>()
    {
        { "NE", new Vector2Int(1, 1) },
        { "SE", new Vector2Int(1, -1) },
        { "SW", new Vector2Int(-1, -1) },
        { "NW", new Vector2Int(-1, 1) }
    };
    protected Dictionary<string, string> compassOpposite = new Dictionary<string, string>()
    {
        { "N", "S" },
        { "NE", "SW" },
        { "E", "W" },
        { "SE", "NW" },
        { "S", "N" },
        { "SW", "NE" },
        { "W", "E" },
        { "NW", "SE" },
    };

    public override void Initialize(Entity entity)
    {
        base.Initialize(entity);

        worldCoordinates = World.Instance.IntVector(entity.transform.position);

        entity.OnDestroyEvent.AddListener(OnEntityDestroyed);
        segmentsParent = new GameObject("Segments Parent").transform;
        segmentsParent.parent = entity.transform;
        segmentsParent.transform.localPosition = Vector3.zero;

        neighbors = new Dictionary<string, Entity>();
        segments = new Dictionary<string, GameObject>();
        
        
        foreach (KeyValuePair<string, Vector2Int> kvp in compassCardinal)
        {
            neighbors.Add(kvp.Key, null);
            segments.Add(kvp.Key, InitializeSegment(kvp.Value));
        }

        
        foreach (KeyValuePair<string, Vector2Int> kvp in compassOrdinal)
        {
            neighbors.Add(kvp.Key, null);
            segments.Add(kvp.Key, InitializeSegment(kvp.Value));
        }
        UpdateSegments();
    }

    void OnEntityDestroyed()
    {
        foreach (KeyValuePair<string, Entity> kvp in neighbors)
        {
            // teill neighbor that this Entity has been destroyed
            bool nbPresent = kvp.Value != null;

            if (!nbPresent)
                continue;

            SegmentationFeature nbSegment = kvp.Value.FindFeature<SegmentationFeature>();
            Debug.Assert(nbSegment != null, "No SegmentationFeature on adjacent Entity. This Error should be impossible.");

            nbSegment.SetNeighborSegment(compassOpposite[kvp.Key], false);
        }
    }

    public override void LocalUpdate()
    {
        base.LocalUpdate();
        UpdateSegments();
        Debug.Log("local update received");
    }

    public void SetNeighborSegment(string direction, bool activate)
    {
        segments[direction].SetActive(activate);
    }

    protected void UpdateSegments(int limit = 0)
    {
        // check horizontally and vertically adjacent spot for other instances of this (segmented) building type
        foreach (KeyValuePair<string, Vector2Int> kvp in compassCardinal)
            neighbors[kvp.Key] = EntityAt(kvp.Value);

        foreach(KeyValuePair<string, Vector2Int> kvp in compassOrdinal)
        {
            neighbors[kvp.Key] = EntityAt(kvp.Value);
        }

        foreach (KeyValuePair<string, Entity> kvp in neighbors)
        {
            // see if corresponding neighbor's active state corresponds to this segment's
            bool nbPresent = kvp.Value != null && (kvp.Value.entityInfo == entity.entityInfo || allowedConnections.Contains(kvp.Value.entityInfo));

            // if the neighbor is diagonal, check adjacent straight directions first
            if (compassOrdinal.ContainsKey(kvp.Key))
            {
                string d1 = kvp.Key[0].ToString();
                string d2 = kvp.Key[1].ToString();

                if (neighbors[d1] != null || neighbors[d2] != null)
                    nbPresent = false;
            }

            // Enable/disable the segment, depending on the neighbor
            segments[kvp.Key].SetActive(nbPresent);

            if (kvp.Value == null)
                continue;

            // Try to update the neighbor, if it exists
            SegmentationFeature nbSegment = kvp.Value.FindFeature<SegmentationFeature>();

            if (nbSegment == null)
                continue;

            if (limit < 0)
                nbSegment.UpdateSegments(limit+1);
        }

        IsStraightWall();
    }

    /// <summary>
    /// Disables the core segment if this wall is part of a straight line.
    /// </summary>
    protected void IsStraightWall()
    {
        string firstDirection = "-";
        string secondDirection = "-";
        foreach (KeyValuePair<string, GameObject> kvp in segments)
        {
            // a segment is active
            if (kvp.Value.activeInHierarchy)
            {
                if (firstDirection == "-")
                    // The segment is the first one found that is active.
                    firstDirection = kvp.Key;
                else if (secondDirection == "-")
                    // The segment is the second one found that is active.
                    secondDirection = kvp.Key;
                else
                {
                    // The segment is the third one found that is active.
                    entity.modelParent.gameObject.SetActive(true);
                    return;
                }
            }
        }
        
        if (firstDirection == "-" || secondDirection == "-")
        {
            // Less than two segment have been found.
            entity.modelParent.gameObject.SetActive(true);
            return;
        }

        // Exactly two segments have been found. Now check if the two are directly opposite.
        entity.modelParent.gameObject.SetActive(compassOpposite[firstDirection] != secondDirection);
    }

    protected GameObject InitializeSegment(Vector2Int direction)
    {
        float angle = Vector2.SignedAngle(direction, Vector2.left);


        GameObject segment = InstantiateSegmentGO(direction);
        Debug.Assert(segment != null, "No segment instantiated. Faulty direction: " + direction);
        
        segment.transform.position = entity.transform.position;
        segment.transform.rotation = Quaternion.Euler(-90, 0, angle);
        segment.SetActive(false);

        return segment;
    }

    protected GameObject InstantiateSegmentGO(Vector2Int direction)
    {
        // cardinal direction
        if (compassCardinal.ContainsValue(direction))
            return Instantiate(cardinalSegmentPrefab, segmentsParent);

        // ordinal (diagonal) direction
        if (compassOrdinal.ContainsValue(direction))
            return Instantiate(diagonalSegmentPrefab, segmentsParent);

        return null;
    }

    protected Entity EntityAt(Vector2Int pos)
    {
        OccupationType occupation = GameManager.Instance.occupationMap.CheckTile(worldCoordinates + pos);
        if ((occupation is Entity) == false)
            return null;

        return (occupation as Entity);
    }

    protected bool IsOfSameType(Entity other)
    {
        if (other == null)
            return false;

        return other.entityInfo == entity.entityInfo;
    }
}
