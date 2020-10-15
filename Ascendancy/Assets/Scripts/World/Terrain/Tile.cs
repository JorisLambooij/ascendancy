using UnityEngine;
using System.Collections.Generic;

public class Tile
{
    #region Internal data
    public Face face;

    public float height;    //the idealized height of this tile

    public float worldX, worldZ;
    #endregion

    #region Terrain Data

    protected bool flatLand;
    public TerrainType tType;

    #endregion
    public bool FlatLand { get => flatLand; set => flatLand = value; }

    //basic constructor
    public Tile()
    {
        face = new Face();
    }

}