using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(World))]
public class TileOccupationMap : MonoBehaviour
{
    private World world;
    private TileOccupation[,] occupationMap;

    protected void Start()
    {
        world = transform.GetComponent<World>();
        int worldSize = (int)world.EffectiveWorldSize;
        occupationMap = new TileOccupation[worldSize, worldSize];

        for (int x = 0; x < worldSize; x++)
            for (int y = 0; y < worldSize; y++)
                occupationMap[x, y] = new TileOccupation();
    }

    public OccupationType CheckTile(Vector2Int intPos, TileOccupation.OccupationLayer layer = TileOccupation.OccupationLayer.Building)
    {
        if (!InBounds(intPos))
            return null;

        return occupationMap[intPos.x, intPos.y].occupation[layer];
    }

    public bool IsTileFree(int tileX, int tileY, TileOccupation.OccupationLayer layer = TileOccupation.OccupationLayer.Building)
    {
        if (!InBounds(tileX, tileY))
            return false;

        return occupationMap[tileX, tileY].occupation[layer] == null;
    }
    
    public bool AreTilesFree(Vector3 pos, Vector2Int dimensions, TileOccupation.OccupationLayer layer = TileOccupation.OccupationLayer.Building)
    {
        Vector2Int v = world.IntVector(pos);
        
        int halfX = dimensions.x / 2;
        int halfY = dimensions.y / 2;

        // If a single tile is occupied, return false.
        for (int x = 0; x < dimensions.x; x++)
            for (int y = 0; y < dimensions.y; y++)
            {
                int finalX = v.x + x - halfX, finalY = v.y + y - halfY;
                if (!IsTileFree(finalX, finalY, layer))
                    return false;
            }

        // All tiles free.
        return true;
    }

    /// <summary>
    /// Places an occupying object at the specified position.
    /// </summary>
    /// <param name="pos">The root position of the object.</param>
    /// <param name="occupyingEntity">Can be either an Entity or a Construction Site (for now).</param>
    /// <param name="layer">Which Layer are we operating on?</param>
    public void NewOccupation(Vector3 pos, OccupationType occupyingEntity, TileOccupation.OccupationLayer layer = TileOccupation.OccupationLayer.Building)
    {
        Vector2Int v = world.IntVector(pos);
        Vector2Int dimensions = occupyingEntity.GetEntityInfo().dimensions;

        int halfX = dimensions.x / 2;
        int halfY = dimensions.y / 2;

        // TODO: check that all tiles are within the world bounds (not < 0, etc.)
        Debug.Assert(v.x - halfX >= 0, "X position too small!");
        Debug.Assert(v.y - halfY >= 0, "Y position too small!");
        Debug.Assert(v.x + halfX >= 0, "X position too big!");
        Debug.Assert(v.y + halfY >= 0, "Y position too big!");

        // Mark all tiles as occupied.
        for (int x = 0; x < dimensions.x; x++)
            for (int y = 0; y < dimensions.y; y++)
            {
                Debug.Assert(occupationMap[v.x + x - halfX, v.y + y - halfY].occupation[layer] == null, "Tile " + v.x + ":" + v.y + " already occupied, please check.");

                occupationMap[v.x + x - halfX, v.y + y - halfY].occupation[layer] = occupyingEntity;
            }

        SendUpdates(v);
    }

    /// <summary>
    /// Sends LocalUpdates() to occupying Entities adjecent to the specified position.
    /// </summary>
    /// <param name="pos">The origin of the Update signal.</param>
    /// <param name="layer">Which Layer to send the update to</param>
    protected void SendUpdates(Vector2Int pos, TileOccupation.OccupationLayer layer = TileOccupation.OccupationLayer.Building)
    {
        for(int x = -1; x <= 1; x++)
            for(int y = -1; y <= 1; y++)
            {
                Vector2Int target = pos + new Vector2Int(x, y);
                if (InBounds(target))
                {
                    Entity e = occupationMap[target.x, target.y].occupation[layer] as Entity;
                    if (e == null)
                        continue;

                    e.LocalUpdate();
                }
            }

    }

    public void ClearOccupation(Vector3 pos, Vector2Int dimensions, TileOccupation.OccupationLayer layer = TileOccupation.OccupationLayer.Building)
    {
        Vector2Int v = world.IntVector(pos);

        int halfX = dimensions.x / 2;
        int halfY = dimensions.y / 2;

        // Mark all tiles as occupied.
        for (int x = 0; x < dimensions.x; x++)
            for (int y = 0; y < dimensions.y; y++)
            {
                //Debug.Assert(occupationMap[v.x + x - halfX, v.y + y - halfY].occupation[layer] == null, "Tile " + v.x + ":" + v.y + " already Occupied, please check.");
                occupationMap[v.x + x - halfX, v.y + y - halfY].occupation[layer] = null;
            }
    }

    protected bool InBounds(Vector2Int v)
    {
        return InBounds(v.x, v.y);
    }

    protected bool InBounds(int x, int y)
    {
        return x >= 0 && x < occupationMap.GetLength(0) && y >= 0 && y < occupationMap.GetLength(1);
    }
}
