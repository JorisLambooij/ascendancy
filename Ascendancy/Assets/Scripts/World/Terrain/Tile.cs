using UnityEngine;
using System.Collections.Generic;

public class Tile
{
    #region Internal data
    public Vector3 topLeft;
    public Vector3 topRight;
    public Vector3 botLeft;
    public Vector3 botRight;

    public float height;    //the idealized height of this tile
    public bool isSlope = false;

    public float tileSize = 3f; //tile size in meters

    //cached things
    private float centerX;
    private float centerZ;

    public float worldX, worldZ;
    #endregion

    #region Terrain Data

    private bool flatLand;
    private TerrainType type;

    #endregion
    public bool FlatLand { get => flatLand; set => flatLand = value; }

    //basic constructor
    public Tile()
    {
       
    }

}