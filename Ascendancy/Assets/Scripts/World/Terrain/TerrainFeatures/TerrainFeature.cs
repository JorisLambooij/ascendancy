using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainFeature
{
    public bool enabled;
    public float frequency;
    public float size;

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
}
