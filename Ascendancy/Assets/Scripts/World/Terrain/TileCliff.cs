using UnityEngine;
using System.Collections.Generic;

public class TileCliff : Tile
{
    #region Internal data
    public Face topCliff;
    public Face rightCliff;
    public Face botCliff;
    public Face leftCliff;

    public int diagonal;
    #endregion

    //basic constructor
    public TileCliff(Tile baseTile) : base(baseTile.worldX, baseTile.worldZ, baseTile.Height)
    {
        topCliff = null;
        rightCliff = null;
        botCliff = null;
        leftCliff = null;

        this.Height = baseTile.Height;    //the idealized height of this tile

        this.face.topLeft = baseTile.face.topLeft;
        this.face.topRight = baseTile.face.topRight;
        this.face.botLeft = baseTile.face.botLeft;
        this.face.botRight = baseTile.face.botRight;

        this.worldX = baseTile.worldX;
        this.worldZ = baseTile.worldZ;
        this.diagonal = 0;
        this.terrainType = baseTile.terrainType;
        //this.terrainType = TerrainType.NONE;
    }

    public override bool FlatLand()
    {
        return false;
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