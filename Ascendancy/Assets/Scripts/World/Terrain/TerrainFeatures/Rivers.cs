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


    protected override void AddFeature(Tile[,] originalTilemap, ref Tile[,] newTilemap)
    {
        List<Vector2Int> startPositions = RandomPositions(Mathf.Max(numberOfRivers - 1, 4), originalTilemap.GetLength(0), originalTilemap.GetLength(1), padding);
        //List<Vector2Int> endPositions = RandomPositions(numberOfRivers, tilemap.GetLength(0), tilemap.GetLength(1));

        for (int i = 0; i < numberOfRivers; i++)
        {
            int randomI = Random.Range(0, startPositions.Count);
            int randomJ = Random.Range(0, startPositions.Count - 1);
            if (randomI == randomJ)
                randomJ++;
            CreateRiver(originalTilemap, ref newTilemap, startPositions[randomI], startPositions[randomJ] + Vector2Int.one);
            //Vector2Int p = startPositions[i];
            //Debug.Log(p);
            //tilemap[p.x, p.y].terrainType = TerrainType.NONE;
        }
    }

    private void CreateRiver(Tile[,] originalTilemap, ref Tile[,] newTilemap, Vector2Int startPosition, Vector2Int endPosition)
    {
        float desiredLength = Random.Range(minimumLength, maximumLength);
        float desiredWidth = Random.Range(riverWidth / 1.5f, riverWidth);
        float startingHeight = originalTilemap[startPosition.x, startPosition.y].Height;

        Vector2 direction = endPosition - startPosition;
        direction.Normalize();

        while (originalTilemap[startPosition.x, startPosition.y].rawHeight > 0)
        {
            startPosition += originalTilemap[startPosition.x, startPosition.y].gradient;
            
            if (startPosition.x < 0 || startPosition.x >= originalTilemap.GetLength(0))
                return;

            if (startPosition.y < 0 || startPosition.y >= originalTilemap.GetLength(1))
                return;
        }
        Vector2 position = startPosition;
        Vector2 initialDirection = direction;
        for (float l = 0; l < desiredLength; l += 0.2f)
        {
            float width = Mathf.Lerp(desiredWidth / 2, desiredWidth, l / desiredLength);
            Vector2 gradient = ConvertTilesToRiver(originalTilemap, ref newTilemap, position, width, startingHeight);
            
            Vector2 gradientDirection = Vector3.Slerp(initialDirection, gradient, meanderingCoefficient);
            direction = Vector3.Slerp(direction, gradientDirection, correctingCoefficient);
            position += direction;
            l += direction.magnitude;
        }

        newTilemap[startPosition.x, startPosition.y].terrainType = TerrainType.NONE;
    }

    private Vector2 ConvertTilesToRiver(Tile[,] originalTilemap, ref Tile[,] newTilemap, Vector2 center, float radius, float startingHeight)
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

                if (u < 0 || u >= originalTilemap.GetLength(0) || v < 0 || v >= originalTilemap.GetLength(1))
                    continue;

                float normalizationFactor = 1;// f / dSquare;
                Vector2Int tileGradient = originalTilemap[u, v].gradient;
                gradient += new Vector2(tileGradient.x, tileGradient.y) * normalizationFactor;
                gradientNormalization += normalizationFactor;

                newTilemap[u, v].Height = -1;
                newTilemap[u, v].terrainType = TerrainType.SAND;
            }
        return gradient / gradientNormalization;
    }
}
