using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rivers : TerrainFeature
{
    public int numberOfRivers;
    public float minimumLength;
    public float maximumLength;
    public float riverWidth;

    //public int riverMidwayPoints;
    public float meanderingCoefficient;
    public float correctingCoefficient;


    public override void AddFeature(ref Tile[,] tilemap)
    {
        if (!enabled)
            return;

        List<Vector2Int> startPositions = RandomPositions(numberOfRivers, tilemap.GetLength(0), tilemap.GetLength(1));
        List<Vector2Int> endPositions = RandomPositions(numberOfRivers, tilemap.GetLength(0), tilemap.GetLength(1));
        List<Vector2> riverDirections = RandomDirections(numberOfRivers);

        for (int i = 0; i < numberOfRivers; i++)
        {
            CreateRiver(ref tilemap, startPositions[i], riverDirections[i]);
        }
    }

    private void CreateRiver(ref Tile[,] tilemap, Vector2Int startPosition, Vector2 direction)
    {
        float desiredLength = Random.Range(minimumLength, maximumLength);
        Vector2 target = startPosition + desiredLength * direction;
        float desiredWidth = Random.Range(riverWidth / 2, riverWidth);
        float startingHeight = tilemap[startPosition.x, startPosition.y].Height;

        Debug.Log("Creating River at " + startPosition);

        Vector2 position = startPosition;
        for (float l = 0; l < desiredLength; l += 0.2f)
        {
            Vector2 gradient = ConvertTilesToRiver(ref tilemap, position, desiredWidth, startingHeight);

            direction = Vector2.Lerp(direction, gradient, meanderingCoefficient);
            direction = Vector2.Lerp(direction, (target - position).normalized, correctingCoefficient);
            position += direction;
            l += direction.magnitude;
        }

        tilemap[startPosition.x, startPosition.y].terrainType = TerrainType.NONE;
    }

    private Vector2 ConvertTilesToRiver(ref Tile[,] tilemap, Vector2 center, float radius, float startingHeight)
    {
        Vector2 gradient = Vector2.zero;
        float gradientNormalization = 0;
        int radiusInt = Mathf.CeilToInt(radius);
        float innerRadius = radius * 0.7f;
        for (int dx = -radiusInt; dx <= radiusInt; dx++)
            for (int dy = -radiusInt; dy <= radiusInt; dy++)
            {
                float dSquare = dx * dx + dy * dy;

                //outside the circle
                if (dSquare > radius * radius)
                    continue;

                int u = Mathf.RoundToInt(center.x + dx);
                int v = Mathf.RoundToInt(center.y + dy);

                if (u < 0 || u >= tilemap.GetLength(0) || v < 0 || v >= tilemap.GetLength(1))
                    continue;

                Tile t = tilemap[u, v];

                float normalizationFactor = 1;// f / dSquare;
                Vector2Int tileGradient = t.gradient;
                gradient += new Vector2(tileGradient.x, tileGradient.y) * normalizationFactor;
                gradientNormalization += normalizationFactor;

                bool inner = dSquare < innerRadius * innerRadius;
                //if (t.Height <= startingHeight + 1)
                //{
                    t.terrainType = TerrainType.WATER;
                    t.Height = inner ? -1 : Mathf.Max(-1, t.Height - 1);
                //}
            }
        return gradient / gradientNormalization;
    }
}
