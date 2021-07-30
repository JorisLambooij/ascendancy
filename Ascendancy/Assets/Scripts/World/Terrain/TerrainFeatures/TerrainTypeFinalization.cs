using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainTypeFinalization : TerrainFeature
{
    public int snowHeight;
    public float snowThreshold;

    public override void AddFeature(ref Tile[,] tilemap)
    {
        if (!enabled)
            return;

        for (int x = 0; x < tilemap.GetLength(0); x++)
            for (int y = 0; y < tilemap.GetLength(1); y++)
            {
                Tile t = tilemap[x, y];

                if (t.Height > snowHeight)
                    t.terrainType = TerrainType.SNOW;
                else if (t.Height == snowHeight)
                    t.terrainType = Random.Range(0, 1f) < snowThreshold ? TerrainType.SNOW : TerrainType.ROCK;

            }
    }
}
