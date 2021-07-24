using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hills : TerrainFeature
{
    public float frequency;
    public float size;
    public bool depressions;

    public override void AddFeature(ref Tile[,] tilemap)
    {
        if (!enabled)
            return;

        int width = tilemap.GetLength(0);
        int height = tilemap.GetLength(1);
        //float[,] noisemap = heightMapGenerator.GenerateNoiseMap(width, height, heightMapGenerator.perlinOffset, 3, frequency, 0.7f, 10f / frequency);
        float heightThreshold = Mathf.Lerp(20, -3, size / 10) / 20;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Tile t = tilemap[x, y];
                int h = Mathf.RoundToInt(t.rawHeight * World.Instance.heightResolution);
                t.Height = depressions ? h : Mathf.Max(h, 0);// h > heightThreshold ? h : 0;
                t.terrainType = t.Height > 1 ? TerrainType.ROCK : t.Height > 0 ? TerrainType.DIRT : TerrainType.GRASS;
            }

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                Vector2Int lowestNeighbor = new Vector2Int(x, y);
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int u = x + dx;
                        int v = y + dy;

                        if (u < 0 || u >= width || v < 0 || v >= height)
                            continue;

                        if (tilemap[u, v].rawHeight < tilemap[lowestNeighbor.x, lowestNeighbor.y].rawHeight)
                            lowestNeighbor = new Vector2Int(u, v);
                    }
                tilemap[x, y].gradient = lowestNeighbor - new Vector2Int(x, y);
            }
    }
                
    protected override Tile ChangeTile(Tile t)
    {
        Tile newT = new Tile(t.worldX, t.worldZ, t.Height);
        
        newT.terrainType = t.Height > 1 ? TerrainType.ROCK : TerrainType.GRASS;
        return newT;
    }
}
