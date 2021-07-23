using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lakes : TerrainFeature
{
    public int numberOfLakes;
    public int maximumSize;

    public override void AddFeature(ref Tile[,] tilemap)
    {
        if (!enabled)
            return;

        List<Vector2Int> positions = RandomPositions(numberOfLakes, tilemap.GetLength(0), tilemap.GetLength(1));
        foreach(Vector2Int position in positions)
            CreateLake(ref tilemap, position);
    }

    private void CreateLake(ref Tile[,] tilemap, Vector2Int pos)
    {
        Vector2Int lowestPoint = pos;
        Debug.Log("Starting at: " + lowestPoint + " with height " + tilemap[lowestPoint.x, lowestPoint.y].rawHeight);
        for (int i = 0; i <= 30; i++)
        {
            Vector2Int lowerPos = lowestPoint + tilemap[lowestPoint.x, lowestPoint.y].gradient;

            Debug.Assert((lowerPos.x < 0 || lowerPos.x >= tilemap.GetLength(0) || lowerPos.y < 0 || lowerPos.y >= tilemap.GetLength(1)) == false, "Gradient went out of bounds!");
            
            // no more lower points, so break the loop
            if (lowerPos == lowestPoint)
                break;

            lowestPoint = lowerPos;

            if (i == 30)
                Debug.LogError("Lake Gradient Iteration taking too long!");
        }

        Debug.Log("Arrived at " + lowestPoint);
        // carve out the basin
        int lakeHeight = tilemap[lowestPoint.x, lowestPoint.y].Height;
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

                    if (u < 0 || u >= tilemap.GetLength(0) || v < 0 || v >= tilemap.GetLength(1))
                        continue;

                    if (dx == 0 && dy == 0)
                        continue;

                    Vector2Int nb = new Vector2Int(u, v);
                    // if the neighboring tile should belong to the lake but doesn't yet, add it to the stack
                    if (tilemap[u, v].Height <= lakeHeight && tilemap[u, v].terrainType != TerrainType.WATER && !trackedTiles.Contains(nb) && !exploration.Contains(nb))
                        exploration.Push(nb);
                }
            check++;
        }

        //Debug.Assert(check < maximumSize * 2, "Stack traversal took too long");

        // fill the basin if it does not exceed the maximum lake size
        if (trackedTiles.Count < maximumSize)
            foreach (Vector2Int v in trackedTiles)
                tilemap[v.x, v.y].terrainType = TerrainType.WATER;

        tilemap[lowestPoint.x, lowestPoint.y].terrainType = TerrainType.SAND;
    }


    protected override Tile ChangeTile(Tile t)
    {
        int h = t.Height < 0 ? 0 : t.Height;
        Tile newT = new Tile(t.worldX, t.worldZ, h);
        newT.terrainType = t.Height < 0 ? TerrainType.WATER : t.terrainType;

        return newT;
    }
}
