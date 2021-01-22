using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class TerrainOperation
{
    protected Tile[,] originalTilemap;
    protected Tile[,] newTilemap;
    /// <summary>
    /// Takes a mesh and performs this class's operation on each Tile
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="dimensions"></param>
    /// <returns></returns>
    public Tile[,] Run(Tile[,] tilemap, int batchSize)
    {
        int width  = tilemap.GetLength(0);
        int height = tilemap.GetLength(1);

        originalTilemap = tilemap;
        newTilemap = new Tile[width, height];

        int totalTiles = width * height;
        int batchAmount = Mathf.CeilToInt(totalTiles / batchSize);

        Parallel.For(0, batchAmount, batchNo =>
            {
                int start = batchNo * batchSize;
                int end = Mathf.Min(start + batchSize, totalTiles);
                OperationBatch(start, end, width);
            } );
        
        return newTilemap;
    }

    private void OperationBatch(int start, int end, int width)
    {
        for (int i = start; i < end; i++)
        {
            int x = i % width;
            int y = i / width;
            newTilemap[x, y] = originalTilemap[x, y];
            TileOperation(x, y);
        }
    }

    /// <summary>
    /// The operation that is carried out per tile. Edge cases must still be handled manually.
    /// </summary>
    /// <param name="x">The x coordinate of the tile.</param>
    /// <param name="y">The y coordinate of the tile.</param>
    public virtual void TileOperation(int x, int y)
    {
        newTilemap[x, y] = originalTilemap[x, y];
    }

}
