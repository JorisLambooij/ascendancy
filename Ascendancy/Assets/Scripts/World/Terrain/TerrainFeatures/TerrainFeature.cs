using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainFeature
{
    public bool enabled;

    public static HeightMapGenerator heightMapGenerator;

    public virtual void AddFeature(ref Tile[,] tilemap)
    {
        if (!enabled)
            return;

        for (int x = 0; x < tilemap.GetLength(0); x++)
            for (int y = 0; y < tilemap.GetLength(1); y++)
            {
                tilemap[x, y] = ChangeTile(tilemap[x, y]);
            }
    }

    protected virtual Tile ChangeTile(Tile t)
    {
        return t;
    }

    protected List<Vector2Int> RandomPositions(int number, int size)
    {
        return RandomPositions(number, size, size);
    }

    protected List<Vector2Int> RandomPositions(int number, int width, int height)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int i = 0; i < number; i++)
        {
            Vector2Int position = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            positions.Add(position);
        }
        return positions;
    }

    protected List<Vector2> RandomDirections(int number)
    {
        List<Vector2> directions = new List<Vector2>();
        for (int i = 0; i < number; i++)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            directions.Add(direction);
        }
        return directions;
    }
}
