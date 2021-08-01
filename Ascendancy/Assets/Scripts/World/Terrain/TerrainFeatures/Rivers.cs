using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rivers : TerrainFeature
{
    public int numberOfRivers;
    public int padding;
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

        List<Vector2Int> startPositions = RandomPositions(Mathf.Max(numberOfRivers - 1, 4), tilemap.GetLength(0), tilemap.GetLength(1), padding);
        //List<Vector2Int> endPositions = RandomPositions(numberOfRivers, tilemap.GetLength(0), tilemap.GetLength(1));

        for (int i = 0; i < numberOfRivers; i++)
        {
            int randomI = Random.Range(0, startPositions.Count);
            int randomJ = Random.Range(0, startPositions.Count - 1);
            if (randomI == randomJ)
                randomJ++;
            CreateRiver(ref tilemap, startPositions[randomI], startPositions[randomJ] + Vector2Int.one);
            //Vector2Int p = startPositions[i];
            //Debug.Log(p);
            //tilemap[p.x, p.y].terrainType = TerrainType.NONE;
        }
    }

    private void CreateRiver(ref Tile[,] tilemap, Vector2Int startPosition, Vector2Int endPosition)
    {
        float desiredLength = Random.Range(minimumLength, maximumLength);
        float desiredWidth = Random.Range(riverWidth / 1.5f, riverWidth);
        float startingHeight = tilemap[startPosition.x, startPosition.y].Height;

        Vector2 direction = endPosition - startPosition;
        direction.Normalize();

        while (tilemap[startPosition.x, startPosition.y].rawHeight > 0)
        {
            startPosition += tilemap[startPosition.x, startPosition.y].gradient;
            
            if (startPosition.x < 0 || startPosition.x >= tilemap.GetLength(0))
                return;

            if (startPosition.y < 0 || startPosition.y >= tilemap.GetLength(1))
                return;
        }
        Vector2 position = startPosition;
        Vector2 initialDirection = direction;
        for (float l = 0; l < desiredLength; l += 0.2f)
        {
            float width = Mathf.Lerp(desiredWidth / 2, desiredWidth, l / desiredLength);
            Vector2 gradient = ConvertTilesToRiver(ref tilemap, position, width, startingHeight);
            
            Vector2 gradientDirection = Vector3.Slerp(initialDirection, gradient, meanderingCoefficient);
            direction = Vector3.Slerp(direction, gradientDirection, correctingCoefficient);
            position += direction;
            l += direction.magnitude;
        }

        tilemap[startPosition.x, startPosition.y].terrainType = TerrainType.NONE;
    }

    private Vector2 ConvertTilesToRiver(ref Tile[,] tilemap, Vector2 center, float radius, float startingHeight)
    {
        Vector2 gradient = Vector2.zero;
        float gradientNormalization = 0;
        int radiusInt = Mathf.CeilToInt(radius) + 1;
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
                //t.terrainType = TerrainType.SAND;
                //t.Height = inner ? -1 : Mathf.Max(-1, t.Height - 1);
                //}
                t.Height = -1;
                //t.Height = Mathf.RoundToInt(Mathf.Lerp(-1, t.rawHeight, 1 / Mathf.Sqrt(dSquare)));
                //t.terrainType = inner ? TerrainType.SAND : t.terrainType;
                t.terrainType = TerrainType.SAND;
            }
        return gradient / gradientNormalization;
    }
}
