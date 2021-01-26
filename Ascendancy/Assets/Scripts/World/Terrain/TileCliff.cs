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

    //basic constructor
    public TileCliff(Tile baseTile) : base(baseTile.worldX, baseTile.worldZ)
    {
        topCliff = null;
        rightCliff = null;
        botCliff = null;
        leftCliff = null;
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