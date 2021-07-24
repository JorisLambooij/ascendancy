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
        //check bottom-left
        if (x > 0 && y > 0)
        {
            if (cliff.botCliff != null && cliff.leftCliff != null)
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
                    topLeft  = originalTilemap[x - 1, y].face.topRight, //top left
                    topRight = originalTilemap[x - 1, y].face.topRight, //up right
                    botRight = originalTilemap[x, y - 1].face.topRight, //down right
                    botLeft  = originalTilemap[x, y - 1].face.topLeft //down left
                };
            }
        }

        //check top-left
        if (x > 0 && y < maxY)
        {
            if (cliff.topCliff != null && cliff.leftCliff != null)
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
                    topLeft  = originalTilemap[x - 1, y].face.topRight, //top left
                    topRight = originalTilemap[x, y + 1].face.botRight, //up right
                    botRight = originalTilemap[x, y + 1].face.botRight, //down right
                    botLeft  = originalTilemap[x - 1, y].face.botRight //down left
                };
            }
        }

        //check top-right
        if (x < maxX && y < maxY)
        {
            if (cliff.topCliff != null && cliff.rightCliff != null)
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
                    topLeft  = originalTilemap[x + 1, y].face.topLeft, //top left
                    topRight = originalTilemap[x + 1, y].face.botLeft, //up right
                    botRight = originalTilemap[x, y + 1].face.botLeft, //down right
                    botLeft  = originalTilemap[x, y + 1].face.botLeft //down left
                };
            }
        }

        //check bottom-right
        if (x < maxX && y > 0)
        {
            if (cliff.botCliff != null && cliff.rightCliff != null)
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
                    topLeft = originalTilemap[x, y - 1].face.topLeft, //top left
                    topRight = originalTilemap[x + 1, y].face.topLeft, //up right
                    botRight = originalTilemap[x + 1, y].face.botLeft, //down right
                    botLeft = originalTilemap[x, y - 1].face.topLeft //down left
                };
            }
        }
    }
}
