using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainFeature
{
    public bool enabled;

    public static HeightMapGenerator heightMapGenerator;

    public void Apply(ref Tile[,] originalTilemap)
    {
        if (!enabled)
            return;

        Tile[,] newTilemap = new Tile[originalTilemap.GetLength(0), originalTilemap.GetLength(1)];
        for (int x = 0; x < originalTilemap.GetLength(0); x++)
            for (int y = 0; y < originalTilemap.GetLength(1); y++)
                newTilemap[x, y] = new Tile(originalTilemap[x, y]);
        
        AddFeature(originalTilemap, ref newTilemap);

        originalTilemap = newTilemap;
    }

    protected virtual void AddFeature(Tile[,] originalTilemap, ref Tile[,] newTilemap)
    {
        for (int x = 0; x < originalTilemap.GetLength(0); x++)
            for (int y = 0; y < originalTilemap.GetLength(1); y++)
            {
                newTilemap[x, y] = ChangeTile(originalTilemap[x, y]);
            }
    }

    protected virtual Tile ChangeTile(Tile t)
    {
        return t;
    }

    public static List<Vector2Int> RandomPositions(int number, int size)
    {
        return RandomPositions(number, size, size);
    }

    public static List<Vector2Int> RandomPositions(int number, int width, int height, int padding = 0)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        int sqrt = Mathf.Max(1, Mathf.FloorToInt(Mathf.Sqrt(number)));
        int amountOfSectors = sqrt * sqrt;
        int quadrantWidth = width / sqrt;
        int quadrantHeight = height / sqrt;
        for (int i = 0; i < number; i++)
        {
            int sector = i % amountOfSectors;
            int sectorX = sector % sqrt;
            int sectorY = sector / sqrt;

            int minX = sectorX * quadrantWidth;
            int maxX = (sectorX + 1) * quadrantWidth;
            int minY = sectorY * quadrantHeight;
            int maxY = (sectorY + 1) * quadrantHeight;

            Vector2Int position = new Vector2Int(Random.Range(minX, maxX), Random.Range(minY, maxY));
            position = new Vector2Int(Mathf.Clamp(position.x, padding, width - padding), Mathf.Clamp(position.y, padding, height - padding));
            positions.Add(position);
        }
        return positions;
    }


    protected List<Vector2> RandomDirections(int number)
    {
        List<Vector2> directions = new List<Vector2>();
        for (int i = 0; i < number; i++)
        {
            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            directions.Add(direction);
        }
        return directions;
    }
}
