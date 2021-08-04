using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainTypeFinalization : TerrainFeature
{
    public int snowHeight;
    public float snowThreshold;

    protected override void AddFeature(Tile[,] originalTilemap, ref Tile[,] newTilemap)
    {
        for (int x = 0; x < originalTilemap.GetLength(0); x++)
            for (int y = 0; y < originalTilemap.GetLength(1); y++)
            {
                Tile t = originalTilemap[x, y];

                if (t.Height > snowHeight)
                    newTilemap[x, y].terrainType = TerrainType.SNOW;
                else if (t.Height == snowHeight)
                    newTilemap[x, y].terrainType = Random.Range(0, 1f) < snowThreshold ? TerrainType.SNOW : TerrainType.ROCK;

            }
    }
}
