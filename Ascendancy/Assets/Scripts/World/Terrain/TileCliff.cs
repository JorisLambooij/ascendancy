using UnityEngine;
using System.Collections.Generic;

public class TileCliff : Tile
{
    #region Internal data
    public Face topCliff;
    public Face rightCliff;
    public Face botCliff;
    public Face leftCliff;
    #endregion

    #region Terrain Data

    #endregion

    //basic constructor
    public TileCliff(Tile baseTile) : base()
    {
        topCliff = null;
        rightCliff = null;
        botCliff = null;
        leftCliff = null;


        this.face.topLeft = baseTile.face.topLeft;
        this.face.topRight = baseTile.face.topRight;
        this.face.botLeft = baseTile.face.botLeft;
        this.face.botRight = baseTile.face.botRight;

        this.height = baseTile.height;    //the idealized height of this tile

        this.worldX = baseTile.worldX;
        this.worldZ = baseTile.worldZ;

        this.flatLand = baseTile.FlatLand;
        this.terrainType = baseTile.terrainType;
    }

}