using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MountainRanges : TerrainFeature
{
    public int continents;
    public float width;
    public float maxHeight;
    public float density;

    public override void AddFeature(ref Tile[,] tilemap)
    {
        List<Vector2Int> continentPoints = RandomPositions(continents, tilemap.GetLength(0), tilemap.GetLength(1));

        for (int x = 0; x < tilemap.GetLength(0); x++)
            for (int y = 0; y < tilemap.GetLength(1); y++)
            {
                KeyValuePair<Vector2Int, float> closestContinent = new KeyValuePair<Vector2Int, float>(Vector2Int.zero, Mathf.Infinity);
                KeyValuePair<Vector2Int, float> secondContinent = new KeyValuePair<Vector2Int, float>(Vector2Int.zero, Mathf.Infinity);

                foreach(Vector2Int p in continentPoints)
                {
                    float distance = Vector2.Distance(p, new Vector2(x, y));
                    if (distance < closestContinent.Value)
                    {
                        secondContinent = closestContinent;
                        closestContinent = new KeyValuePair<Vector2Int, float>(p, distance);
                    }
                    else if (distance < secondContinent.Value)
                    {
                        secondContinent = new KeyValuePair<Vector2Int, float>(p, distance);
                    }
                }

                float delta = secondContinent.Value - closestContinent.Value;
                if (delta < width)
                {
                    tilemap[x, y].terrainType = TerrainType.NONE;
                    float strength = 1 - (delta / width);
                    if (tilemap[x, y].Height >= 0)
                        tilemap[x, y].Height += Mathf.RoundToInt(strength * maxHeight);
                }
            }
    }
}
