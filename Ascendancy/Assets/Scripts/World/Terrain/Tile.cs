using UnityEngine;
using System.Collections.Generic;


public enum TerrainType { NONE, NONE_WALL, GRASS, GRASS_WALL, ROCK, ROCK_WALL, DIRT, DIRT_WALL, SAND, SAND_WALL, WATER, WATER_WALL };

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

    public Face[] GetFaces()
    {
        Face[] faces = new Face[1];
        faces[0] = face;
        return faces;
    }
}