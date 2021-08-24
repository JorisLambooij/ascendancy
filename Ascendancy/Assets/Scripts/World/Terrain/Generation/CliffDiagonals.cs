using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffDiagonals : TerrainOperation
{
    public override void TileOperation(int x, int y)
    {
        TileCliff cliff = originalTilemap[x, y] as TileCliff;
        if (cliff == null)
            return;

        int maxX = originalTilemap.GetLength(0) - 1;
        int maxY = originalTilemap.GetLength(1) - 1;

        if (cliff.GetTileType() != 1111)
            return;

        Tile l = GetTileAt(originalTilemap, x - 1, y);
        Tile t = GetTileAt(originalTilemap, x, y + 1);
        Tile r = GetTileAt(originalTilemap, x + 1, y);
        Tile b = GetTileAt(originalTilemap, x, y - 1);

        Tile bl = GetTileAt(originalTilemap, x - 1, y - 1);
        Tile tl = GetTileAt(originalTilemap, x - 1, y + 1);
        Tile tr = GetTileAt(originalTilemap, x + 1, y + 1);
        Tile br = GetTileAt(originalTilemap, x + 1, y - 1);

        //check bottom-left
        if (x > 0 && y > 0)
        {
            if (cliff.botCliff != null && cliff.leftCliff != null && cliff.rightCliff == null && cliff.topCliff == null
                && l.face.botRight.y == bl.face.topRight.y && bl.face.topRight.y == b.face.topLeft.y)
            {
                // make the leftCliff a diagonal wall, delete bottom wall
                cliff.leftCliff.botRight = cliff.botCliff.botRight;
                cliff.leftCliff.topRight = cliff.botCliff.topRight;
                cliff.botCliff = null;
                cliff.diagonal = 1;

                // fix the flat part
                cliff.face.botLeft = cliff.face.botRight;
                cliff.face2 = new Face
                {
                    topLeft  = l.face.topRight, //top left
                    topRight = l.face.topRight, //up right
                    botRight = b.face.topRight, //down right
                    botLeft  = b.face.topLeft //down left
                };
            }
        }

        //check top-left
        if (x > 0 && y < maxY)
        {
            if (cliff.topCliff != null && cliff.leftCliff != null && cliff.rightCliff == null && cliff.botCliff == null
                && l.face.topRight.y == tl.face.botRight.y && tl.face.botRight.y == t.face.botLeft.y)
            {
                // make the leftCliff a diagonal wall, delete top wall
                cliff.leftCliff.topLeft = cliff.topCliff.topLeft;
                cliff.leftCliff.botLeft = cliff.topCliff.botLeft;
                cliff.topCliff = null;
                cliff.diagonal = 2;

                // fix the flat part
                cliff.face.topLeft = cliff.face.botLeft;
                cliff.face2 = new Face
                {
                    topLeft  = l.face.topRight, //top left
                    topRight = t.face.botRight, //up right
                    botRight = t.face.botRight, //down right
                    botLeft  = l.face.botRight //down left
                };
            }
        }

        //check top-right
        if (x < maxX && y < maxY)
        {
            if (cliff.topCliff != null && cliff.rightCliff != null && cliff.leftCliff == null && cliff.botCliff == null
                && r.face.topLeft.y == tr.face.botLeft.y && tr.face.botLeft.y == t.face.botRight.y)
            {
                // make the rightCliff a diagonal wall, delete top wall
                cliff.rightCliff.topRight = cliff.topCliff.topRight;
                cliff.rightCliff.botRight = cliff.topCliff.botRight;
                cliff.topCliff = null;
                cliff.diagonal = 3;

                // fix the flat part
                cliff.face.topRight = cliff.face.topLeft;
                cliff.face2 = new Face
                {
                    topLeft  = r.face.topLeft, //top left
                    topRight = r.face.botLeft, //up right
                    botRight = t.face.botLeft, //down right
                    botLeft  = t.face.botLeft //down left
                };
            }
        }

        //check bottom-right
        if (x < maxX && y > 0)
        {
            if (cliff.botCliff != null && cliff.rightCliff != null && cliff.leftCliff == null && cliff.topCliff == null
                && r.face.botLeft.y == br.face.topLeft.y && br.face.topLeft.y == b.face.topRight.y)
            {
                // make the rightCliff a diagonal wall, delete bottom wall
                cliff.rightCliff.topLeft = cliff.botCliff.topLeft;
                cliff.rightCliff.botLeft = cliff.botCliff.botLeft;
                cliff.botCliff = null;
                cliff.diagonal = 4;

                // fix the flat part
                cliff.face.botRight = cliff.face.topRight;
                cliff.face2 = new Face
                {
                    topLeft = b.face.topLeft, //top left
                    topRight = r.face.topLeft, //up right
                    botRight = r.face.botLeft, //down right
                    botLeft = b.face.topLeft //down left
                };
            }
        }
    }

    //private Vector2Int HeightsAt(int x, int y, int corner, Tile[,] tilemap)
    //{
    //    int h1, h2;
    //    switch(corner)
    //    {
    //        // bottom left
    //        case 1:
    //            tilemap[x - 1, y]
    //    }
    //}
}
