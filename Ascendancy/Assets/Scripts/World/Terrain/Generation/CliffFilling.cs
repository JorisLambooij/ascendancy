using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CliffFilling : TerrainOperation
{
    public override void TileOperation(int x, int y)
    {
        //Vector2 tCliff = new Vector2(0, 0);
        Tile neighbor;
        Tile me;
        TileCliff cliff;

        me = originalTilemap[x, y];
        
        //check left
        if (x > 0)
        {
            neighbor = originalTilemap[x - 1, y];

            if (neighbor.face.topRight.y < me.face.topLeft.y || neighbor.face.botRight.y < me.face.botLeft.y)
            {
                //check if tile is already a cliff
                if (!(newTilemap.GetType() == typeof(TileCliff)))
                {
                    newTilemap[x, y] = new TileCliff(originalTilemap[x, y]);
                }
                cliff = (TileCliff)newTilemap[x, y];

                cliff.leftCliff = new Face();
                cliff.leftCliff.topLeft = me.face.topLeft; //top left
                cliff.leftCliff.topRight = me.face.botLeft; //up right
                cliff.leftCliff.botRight = neighbor.face.botRight; //down right
                cliff.leftCliff.botLeft = neighbor.face.topRight; //down left

                //cliff.face.topLeft = cliff.face.topLeft + Vector3.up;
                newTilemap[x, y] = cliff;
            }
        }
        
        //check above
        if (y < newTilemap.GetLength(1) - 1)
        {
            neighbor = originalTilemap[x, y + 1];

            if (neighbor.face.botLeft.y < me.face.topLeft.y || neighbor.face.botRight.y < me.face.topRight.y)
            {
                //check if tile is already a cliff
                if (!(newTilemap[x, y].GetType() == typeof(TileCliff)))
                {
                    newTilemap[x, y] = new TileCliff(originalTilemap[x, y]);
                }
                cliff = (TileCliff)newTilemap[x, y];

                cliff.topCliff = new Face();
                cliff.topCliff.topLeft = me.face.topRight; //top left
                cliff.topCliff.topRight = me.face.topLeft; //up right
                cliff.topCliff.botRight = neighbor.face.botLeft; //down right
                cliff.topCliff.botLeft = neighbor.face.botRight; //down left

                newTilemap[x, y] = cliff;
            }
        }

        //check right
        if (x < newTilemap.GetLength(0) - 1)
        {
            neighbor = originalTilemap[x + 1, y];

            if (neighbor.face.topLeft.y < me.face.topRight.y || neighbor.face.botLeft.y < me.face.botRight.y)
            {
                //check if tile is already a cliff
                if (!(newTilemap[x, y].GetType() == typeof(TileCliff)))
                {
                    newTilemap[x, y] = new TileCliff(originalTilemap[x, y]);
                }
                cliff = (TileCliff)newTilemap[x, y];

                cliff.rightCliff = new Face();
                cliff.rightCliff.topLeft = me.face.botRight; //top left
                cliff.rightCliff.topRight = me.face.topRight; //up right
                cliff.rightCliff.botRight = neighbor.face.topLeft; //down right
                cliff.rightCliff.botLeft = neighbor.face.botLeft; //down left

                newTilemap[x, y] = cliff;
            }
        }

        //check below
        if (y > 0)
        {
            neighbor = originalTilemap[x, y - 1];

            if (neighbor.face.topLeft.y < me.face.botLeft.y || neighbor.face.topRight.y < me.face.botRight.y)
            {
                //check if tile is already a cliff
                if (!(newTilemap[x, y].GetType() == typeof(TileCliff)))
                {
                    newTilemap[x, y] = new TileCliff(originalTilemap[x, y]);
                }
                cliff = (TileCliff)newTilemap[x, y];

                cliff.botCliff = new Face();
                cliff.botCliff.topLeft = me.face.botLeft; //top left
                cliff.botCliff.topRight = me.face.botRight; //up right
                cliff.botCliff.botRight = neighbor.face.topRight; //down right
                cliff.botCliff.botLeft = neighbor.face.topLeft; //down left

                newTilemap[x, y] = cliff;
            }
        }
    }

}
