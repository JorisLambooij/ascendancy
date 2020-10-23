using UnityEngine;
using System.Collections.Generic;


public enum TerrainType { None, Grass, Rock, Dirt, Sand, Water };

public class Tile
{
    #region Internal data
    public Face face;

    public TerrainType terrainType = 0;

    public float height;    //the idealized height of this tile

    public float worldX, worldZ;
    #endregion

    #region Terrain Data

    protected bool flatLand;

    #endregion
    public bool FlatLand { get => flatLand; set => flatLand = value; }

    //basic constructor
    public Tile()
    {
        face = new Face();
    }

}