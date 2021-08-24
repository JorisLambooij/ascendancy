using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Islandification : TerrainFeature
{
    public int padding;
    public int beachWidth;
    public float beachStrength;
    public HeightMapParameters heightmapParameters;

    protected override void AddFeature(Tile[,] originalTilemap, ref Tile[,] newTilemap)
    {
        int beachPadding = beachWidth + padding;
        int w = originalTilemap.GetLength(0);
        int h = originalTilemap.GetLength(1);
        float[,] noisemap = heightMapGenerator.GenerateNoiseMap(originalTilemap.GetLength(0), originalTilemap.GetLength(1), heightMapGenerator.perlinOffset + Vector2Int.one, heightmapParameters.octaves, heightmapParameters.frequency, heightmapParameters.persistance, heightmapParameters.noiseScale);

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
            {
                if (x < padding || x >= w - padding ||
                    y < padding || y >= h - padding)
                {
                    newTilemap[x, y].Height = -1;
                    newTilemap[x, y].terrainType = TerrainType.SAND;
                    continue;
                }

                if (x < beachPadding + 2 || x >= w - beachPadding - 2 ||
                    y < beachPadding + 2 || y >= h - beachPadding - 2 )
                {
                    float xDelta = 1f - Mathf.Max(beachPadding - x, x - w + beachPadding) / (float)beachPadding;
                    float yDelta = 1f - Mathf.Max(beachPadding - y, y - h + beachPadding) / (float)beachPadding;
                    float delta = Mathf.Min(xDelta, yDelta);// * Mathf.Min(xDelta, yDelta);
                    //newTilemap[x, y].Height = Mathf.RoundToInt(Mathf.Lerp(noisemap[x, y] - beachStrength, originalTilemap[x, y].Height, delta));
                    newTilemap[x, y].Height = Mathf.RoundToInt(Mathf.Lerp(Mathf.Min(0, noisemap[x, y] - beachStrength), originalTilemap[x, y].Height, delta));

                    // everything below sea level is automatically converted to sand
                    if (newTilemap[x, y].Height < 0)
                        newTilemap[x, y].terrainType = TerrainType.SAND;

                    // if the terrain height was changed, change the type to default (grass)
                    if (newTilemap[x, y].Height == 0 && originalTilemap[x, y].Height > 0)
                        newTilemap[x, y].terrainType = TerrainType.GRASS;
                    
                    // add some beaches according to the noisemap, to bring some variation to coastline
                    if (newTilemap[x, y].Height == 0 && noisemap[x, y] >= delta)
                        newTilemap[x, y].terrainType = TerrainType.SAND;
                }
            }
    }
}
