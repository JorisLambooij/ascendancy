using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MountainRanges : TerrainFeature
{
    public int continents;
    [Min(1)]
    public int subContinents;
    public float width;
    public float maxHeight;

    public HeightMapParameters heightmapParameters;

    public override void AddFeature(ref Tile[,] tilemap)
    {
        if (!enabled)
            return;

        float[,] mountainNoise = heightMapGenerator.GenerateNoiseMap(tilemap.GetLength(0), tilemap.GetLength(1), heightMapGenerator.perlinOffset, heightmapParameters.octaves, heightmapParameters.frequency, heightmapParameters.persistance, heightmapParameters.noiseScale);

        List<Vector2Int> continentPoints = RandomPositions(continents, tilemap.GetLength(0), tilemap.GetLength(1));
        List<Vector2Int> subContinentPoints = RandomPositions(continents * subContinents, tilemap.GetLength(0), tilemap.GetLength(1));
        Dictionary<Vector2Int, Vector2Int> subcontinentAssignments = new Dictionary<Vector2Int, Vector2Int>(continents * subContinents);

        // split continents into subcontinents, then assign the subcontinents to the closest continent-point
        foreach(Vector2Int subcontinent in subContinentPoints)
        {
            KeyValuePair<Vector2Int, float> closestContinent = new KeyValuePair<Vector2Int, float>(Vector2Int.zero, Mathf.Infinity);

            foreach (Vector2Int continent in continentPoints)
            {
                float distance = Vector2.Distance(continent, subcontinent);
                if (distance < closestContinent.Value)
                    closestContinent = new KeyValuePair<Vector2Int, float>(continent, distance);
            }
            subcontinentAssignments.Add(subcontinent, closestContinent.Key);
        }

        Dictionary<Vector2Int, float> mountainRangeTiles = new Dictionary<Vector2Int, float>();

        // find the borders of continents
        for (int x = 0; x < tilemap.GetLength(0); x++)
            for (int y = 0; y < tilemap.GetLength(1); y++)
            {
                KeyValuePair<Vector2Int, float> closestContinent = new KeyValuePair<Vector2Int, float>(Vector2Int.zero, Mathf.Infinity);
                KeyValuePair<Vector2Int, float> secondContinent = new KeyValuePair<Vector2Int, float>(Vector2Int.zero, Mathf.Infinity);

                foreach(Vector2Int p in subContinentPoints)
                {
                    float distance = Vector2.Distance(p, new Vector2(x, y));
                    if (distance < closestContinent.Value)
                    {
                        secondContinent = closestContinent;
                        closestContinent = new KeyValuePair<Vector2Int, float>(p, distance);
                    }
                    else if (distance < secondContinent.Value)
                        secondContinent = new KeyValuePair<Vector2Int, float>(p, distance);
                }

                if (subcontinentAssignments[closestContinent.Key] == subcontinentAssignments[secondContinent.Key])
                    continue;

                float delta = secondContinent.Value - closestContinent.Value;
                if (delta < width)
                    mountainRangeTiles.Add(new Vector2Int(x, y), delta);
            }

        // transform the tiles into mountains
        foreach (KeyValuePair<Vector2Int, float> kvp in mountainRangeTiles)
        {
            int x = kvp.Key.x;
            int y = kvp.Key.y;

            float strength = 1 - (kvp.Value / width);
            float noiseValue = Mathf.Abs(mountainNoise[x, y]);
            //if (tilemap[x, y].rawHeight >= 0)
            tilemap[x, y].rawHeight += strength * noiseValue * maxHeight;
            tilemap[x, y].Height = Mathf.RoundToInt(tilemap[x, y].rawHeight);

            if (tilemap[x, y].Height > maxHeight)
                tilemap[x, y].terrainType = TerrainType.NONE;

        }
    }
}
