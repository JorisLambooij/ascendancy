using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTriangleSmoothing : TerrainOperation
{
    //flips the triangles of diagonal slopes in two directions
    public override void TileOperation(int x, int y)
    {
        Tile me;

        me = newTilemap[x, y];
        int tileType = me.GetTileType();

        //topLeft - flip
        if (tileType == 111 || tileType == 2111)
        {
            if (!(me).flippedTriangles)
                (me).ToggleTriangleFlip();
        }

        //topRight - unflip
        if (tileType == 1011 || tileType == 1211)
        {
            if ((me).flippedTriangles)
                (me).ToggleTriangleFlip();
        }

        //botRight - flip
        if (tileType == 1101 || tileType == 1121)
        {
            if (!(me).flippedTriangles)
                (me).ToggleTriangleFlip();
        }

        //botLeft - unflip
        if (tileType == 1110 || tileType == 1112)
        {
            if ((me).flippedTriangles)
                (me).ToggleTriangleFlip();
        }

    }

}
