using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTypeEqualization : TerrainOperation
{
    public override void TileOperation(int x, int y)
    {
        Tile me = originalTilemap[x, y];

        if (me.terrainType == TerrainType.NONE)
            return;

        Dictionary<TerrainType, int> neighboringTypes = new Dictionary<TerrainType, int>();

        int terrainTypeCount = System.Enum.GetNames(typeof(TerrainType)).Length;
        for (int t = 0; t < terrainTypeCount; t++)
            neighboringTypes.Add((TerrainType)t, 0);

        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                int u = x + dx;
                int v = y + dy;

                if (u < 0 || u >= originalTilemap.GetLength(0))
                    continue;
                if (v < 0 || v >= originalTilemap.GetLength(1))
                    continue;
                if (dx == 0 && dy == 0)
                    continue;

                Tile nbTile = originalTilemap[u, v];

                if (nbTile.Height == me.Height)
                    neighboringTypes[nbTile.terrainType]++;
            }

        List<KeyValuePair<TerrainType, int>> myList = neighboringTypes.ToList();
        myList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

        KeyValuePair<TerrainType, int> highestKVP = myList[myList.Count - 1];
        if (highestKVP.Value > neighboringTypes[me.terrainType])
            newTilemap[x, y].terrainType = highestKVP.Key;
    }
}
