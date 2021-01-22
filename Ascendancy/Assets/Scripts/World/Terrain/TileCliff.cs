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

    public new Face[] GetFaces()
    {
        List<Face> faceList = new List<Face>();
        faceList.Add(face);

        if (topCliff != null)
            faceList.Add(topCliff);

        if (rightCliff != null)
            faceList.Add(rightCliff);

        if (botCliff != null)
            faceList.Add(botCliff);

        if (leftCliff != null)
            faceList.Add(leftCliff);

        Face[] faces = faceList.ToArray();
        return faces;
    }

}