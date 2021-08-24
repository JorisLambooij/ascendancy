using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hills : TerrainFeature
{
    public float frequency;
    public float size;
    public bool depressions;

    protected override void AddFeature(Tile[,] originalTilemap, ref Tile[,] newTilemap)
    {
        int width = originalTilemap.GetLength(0);
        int height = originalTilemap.GetLength(1);
        //float[,] noisemap = heightMapGenerator.GenerateNoiseMap(width, height, heightMapGenerator.perlinOffset, 3, frequency, 0.7f, 10f / frequency);
        float heightThreshold = Mathf.Lerp(20, -3, size / 10) / 20;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Tile t = originalTilemap[x, y];
                int h = Mathf.RoundToInt(t.rawHeight * World.Instance.heightResolution);
                newTilemap[x, y].Height = depressions ? h : Mathf.Max(h, 0);// h > heightThreshold ? h : 0;
                newTilemap[x, y].terrainType = newTilemap[x, y].Height > 1 ? TerrainType.ROCK : newTilemap[x, y].Height > 0 ? TerrainType.DIRT : TerrainType.GRASS;
            }

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Vector2Int lowestNeighbor = new Vector2Int(x, y);
                Vector2Int highestNeighbor = new Vector2Int(x, y);
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int u = x + dx;
                        int v = y + dy;

                        if (u < 0 || u >= width || v < 0 || v >= height)
                            continue;

                        if (originalTilemap[u, v].rawHeight < originalTilemap[lowestNeighbor.x, lowestNeighbor.y].rawHeight)
                            lowestNeighbor = new Vector2Int(u, v);

                        else if (originalTilemap[u, v].rawHeight > originalTilemap[highestNeighbor.x, highestNeighbor.y].rawHeight)
                            highestNeighbor = new Vector2Int(u, v);
                    }
                newTilemap[x, y].gradient     = lowestNeighbor  - new Vector2Int(x, y);
                newTilemap[x, y].antiGradient = highestNeighbor - new Vector2Int(x, y);
            }
    }
}
