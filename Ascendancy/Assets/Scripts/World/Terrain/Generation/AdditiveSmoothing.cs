using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditiveSmoothing : TerrainOperation
{
    public override void TileOperation(int x, int y)
    {

        Tile neighbor;
        bool tl = false;
        bool tr = false;
        bool bl = false;
        bool br = false;

        bool tl2 = false;
        bool tr2 = false;
        bool bl2 = false;
        bool br2 = false;

        Tile me = originalTilemap[x, y];

        #region direct
        //check left
        if (x > 0)
        {
            neighbor = originalTilemap[x - 1, y];

            if (neighbor.face.topRight.y == me.face.topLeft.y + 1 && neighbor.face.botRight.y == me.face.botLeft.y + 1)
            {
                tl = true;
                bl = true;
            }
        }

        //check above
        if (y < originalTilemap.GetLength(1) - 1)
        {
            neighbor = originalTilemap[x, y + 1];

            if (neighbor.face.botLeft.y == me.face.topLeft.y + 1 && neighbor.face.botRight.y == me.face.topRight.y + 1)
            {
                tl = true;
                tr = true;
            }
        }

        //check right
        if (x < originalTilemap.GetLength(0) - 1)
        {
            neighbor = originalTilemap[x + 1, y];

            if (neighbor.face.topLeft.y == me.face.topRight.y + 1 && neighbor.face.botLeft.y == me.face.botRight.y + 1)
            {
                tr = true;
                br = true;
            }
        }

        //check below
        if (y > 0)
        {
            neighbor = originalTilemap[x, y - 1];

            if (neighbor.face.topLeft.y == me.face.botLeft.y + 1 && neighbor.face.topRight.y == me.face.botRight.y + 1)
            {
                bl = true;
                br = true;
            }
        }
        #endregion

        #region diagonal
        //check topLeft
        if (x > 0 && y < originalTilemap.GetLength(1) - 1)
        {
            neighbor = originalTilemap[x - 1, y + 1];

            if (neighbor.face.botRight.y > me.face.topLeft.y)
                tl = true;

            if (neighbor.face.botRight.y - 1 > me.face.topLeft.y)
                tl2 = true;
        }

        //check topRight
        if (y < originalTilemap.GetLength(1) - 1 && x < originalTilemap.GetLength(0) - 1)
        {
            neighbor = originalTilemap[x + 1, y + 1];

            if (neighbor.face.botLeft.y > me.face.topRight.y)
                tr = true;

            if (neighbor.face.botLeft.y - 1 > me.face.topRight.y)
                tr2 = true;
        }

        //check botRight
        if (y > 0 && x < originalTilemap.GetLength(0) - 1)
        {
            neighbor = originalTilemap[x + 1, y - 1];

            if (neighbor.face.topLeft.y > me.face.botRight.y)
                br = true;

            if (neighbor.face.topLeft.y - 1 > me.face.botRight.y)
                br2 = true;
        }

        //check botLeft
        if (y > 0 && x > 0)
        {
            neighbor = originalTilemap[x - 1, y - 1];

            if (neighbor.face.topRight.y > me.face.botLeft.y)
                bl = true;

            if (neighbor.face.topRight.y - 1 > me.face.botLeft.y)
                bl2 = true;
        }


        #endregion


        if (tl)
            newTilemap[x, y].face.topLeft.y += 1;
        
        if (tr)
            newTilemap[x, y].face.topRight.y += 1;
        
        if (br)
            newTilemap[x, y].face.botRight.y += 1;
        
        if (bl)
            newTilemap[x, y].face.botLeft.y += 1;
        

        int tileType = (originalTilemap[x, y].GetTileType());

        if (tl2)
        {
            if (tileType == 1101 || tileType == 2212)
                if (me.face.topRight.y + 1 == originalTilemap[x - 1, y + 1].face.botLeft.y || originalTilemap[x - 1, y].face.topRight.y > me.face.topLeft.y || originalTilemap[x, y + 1].face.botLeft.y > me.face.topLeft.y)
                    newTilemap[x, y].face.topLeft.y += 1;
                
        }
        if (tr2)
        {
            if (tileType == 1110 || tileType == 2221)
                //TODO Ist immer noch sehr blocky,
                //viel besser, wenn das if drumherum weg ist,
                //dann aber weird z.b. in chunk 0/0 beim berg,                
                //komische spitzen.
                //Am besten das if fixen, ich komme nicht drauf :(
                //if (originalTilemap[x + 1, y].face.topLeft.y > me.face.topRight.y || originalTilemap[x, y + 1].face.botRight.y > me.face.topRight.y)
                if (me.face.topRight.y + 1 == originalTilemap[x + 1, y + 1].face.botLeft.y || originalTilemap[x + 1, y].face.topLeft.y > me.face.topRight.y || originalTilemap[x, y + 1].face.botRight.y > me.face.topRight.y)
                    newTilemap[x, y].face.topRight.y += 1;
                
        }
        if (br2)
        {
            if (tileType == 0111 || tileType == 1222)
                if (me.face.topRight.y + 1 == originalTilemap[x + 1, y - 1].face.botLeft.y || originalTilemap[x + 1, y].face.botLeft.y > me.face.botRight.y || originalTilemap[x, y - 1].face.topRight.y > me.face.botRight.y)
                    newTilemap[x, y].face.botRight.y += 1;
                
        }
        if (bl2)
        {
            if (tileType == 1011 || tileType == 2122)
                if (me.face.topRight.y + 1 == originalTilemap[x - 1, y - 1].face.botLeft.y || originalTilemap[x - 1, y].face.botRight.y > me.face.botLeft.y || originalTilemap[x, y - 1].face.topLeft.y > me.face.botLeft.y)
                    newTilemap[x, y].face.botLeft.y += 1;
                
        }

        //now lets find illegal tiles and make em legal

        //update tileType
        tileType = (originalTilemap[x, y].GetTileType());

        //case: double slope
        switch (tileType)
        {
            //case: double slope1
            case 1010:
            case 2121:
                RaisePointAt(x, y, 100);
                RaisePointAt(x, y, 1);
                //if (System.Convert.ToBoolean((x + y) % 2))
                //    RaisePointAt(x, y, 100);
                //else
                //    RaisePointAt(x, y, 1);
                break;

            //case: double slope1
            case 1212:
            case 101:
                RaisePointAt(x, y, 1000);
                RaisePointAt(x, y, 10);

                //if (System.Convert.ToBoolean((x + y) % 2))
                //    RaisePointAt(x, y, 1000);
                //else
                //    RaisePointAt(x, y, 10);
                break;

            default:
                break;
        }


    }

    public void RaisePointAt(int x, int y, int corner)
    {
        switch (corner)
        {
            case 1000:
                newTilemap[x, y].face.topLeft.y += 1;
                break;
            case 100:
                newTilemap[x, y].face.topRight.y += 1;
                break;
            case 10:
                newTilemap[x, y].face.botRight.y += 1;
                break;
            case 1:
                newTilemap[x, y].face.botLeft.y += 1;
                break;
            default:
                Debug.LogError("Call of RaisePointAt() at " + x + "," + y + " with wrong integer " + corner);
                break;
        }

    }
}
