using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lakes : TerrainFeature
{
    public int numberOfLakes;
    public int maximumSize;

    protected override void AddFeature(Tile[,] originalTilemap, ref Tile[,] newTilemap)
    {
        if (!enabled)
            return;

        List<Vector2Int> positions = RandomPositions(numberOfLakes, originalTilemap.GetLength(0), originalTilemap.GetLength(1));
        foreach(Vector2Int position in positions)
            CreateLake(originalTilemap, ref newTilemap, position);
    }

    private void CreateLake(Tile[,] originalTilemap, ref Tile[,] newTilemap, Vector2Int pos)
    {
        Vector2Int lowestPoint = pos;
        Debug.Log("Starting at: " + lowestPoint + " with height " + originalTilemap[lowestPoint.x, lowestPoint.y].rawHeight);
        for (int i = 0; i <= 30; i++)
        {
            Vector2Int lowerPos = lowestPoint + originalTilemap[lowestPoint.x, lowestPoint.y].gradient;

            Debug.Assert((lowerPos.x < 0 || lowerPos.x >= originalTilemap.GetLength(0) || lowerPos.y < 0 || lowerPos.y >= originalTilemap.GetLength(1)) == false, "Gradient went out of bounds!");
            
            // no more lower points, so break the loop
            if (lowerPos == lowestPoint)
                break;

            lowestPoint = lowerPos;

            if (i == 30)
                Debug.LogError("Lake Gradient Iteration taking too long!");
        }

        Debug.Log("Arrived at " + lowestPoint);
        // carve out the basin
        int lakeHeight = originalTilemap[lowestPoint.x, lowestPoint.y].Height;
        Stack<Vector2Int> exploration = new Stack<Vector2Int>();
        Stack<Vector2Int> trackedTiles = new Stack<Vector2Int>();
        exploration.Push(lowestPoint);
        int check = 0;
        while (exploration.Count > 0 && check <= maximumSize * 2)
        {
            Vector2Int position = exploration.Pop();
            trackedTiles.Push(position);
            //tilemap[position.x, position.y].terrainType = TerrainType.SAND;
            
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    int u = position.x + dx;
                    int v = position.y + dy;

                    if (u < 0 || u >= originalTilemap.GetLength(0) || v < 0 || v >= originalTilemap.GetLength(1))
                        continue;

                    if (dx == 0 && dy == 0)
                        continue;

                    Vector2Int nb = new Vector2Int(u, v);
                    // if the neighboring tile should belong to the lake but doesn't yet, add it to the stack
                    if (originalTilemap[u, v].Height <= lakeHeight && originalTilemap[u, v].terrainType != TerrainType.WATER && !trackedTiles.Contains(nb) && !exploration.Contains(nb))
                        exploration.Push(nb);
                }
            check++;
        }

        //Debug.Assert(check < maximumSize * 2, "Stack traversal took too long");

        // fill the basin if it does not exceed the maximum lake size
        if (trackedTiles.Count < maximumSize)
            foreach (Vector2Int v in trackedTiles)
                newTilemap[v.x, v.y].terrainType = TerrainType.WATER;

        newTilemap[lowestPoint.x, lowestPoint.y].terrainType = TerrainType.SAND;
    }

}
