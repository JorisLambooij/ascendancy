using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum TerrainType { NONE, NONE_WALL, GRASS, GRASS_WALL, ROCK, ROCK_WALL, DIRT, DIRT_WALL, SAND, SAND_WALL, WATER, WATER_WALL };

public class Tile
{
    #region Internal data
    public Face face;
    public bool flippedTriangles { get; private set; } = false;

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

    /// <summary>
    /// returns a specific ID based on edge level of tile, starting from top left
    /// </summary>
    /// <returns>specific integer</returns>
    public int GetTileType()
    {
        int returnMe = 0;

        int avgHeight = 0;        

        foreach (Vector3 v in face.GetVectors())
        {
            avgHeight += (int)v.y;
        }

        avgHeight = System.Convert.ToInt32(avgHeight / 4f);

        //topLeft
        int compareVertex = -1;

        if ((int)face.topLeft.y == avgHeight)
            compareVertex = 1;
        else if ((int)face.topLeft.y < avgHeight)
            compareVertex = 0; 
        else if ((int)face.topLeft.y > avgHeight)
            compareVertex = 2;
        else        
            Debug.LogError("wtf");

        returnMe += compareVertex * 1000;

        //topRight
        compareVertex = -1;

        if ((int)face.topRight.y == avgHeight)
            compareVertex = 1;
        else if ((int)face.topRight.y < avgHeight)
            compareVertex = 0;
        else if ((int)face.topRight.y > avgHeight)
            compareVertex = 2;
        else
            Debug.LogError("wtf");

        returnMe += compareVertex * 100;

        //botRight
        compareVertex = -1;

        if ((int)face.botRight.y == avgHeight)
            compareVertex = 1;
        else if ((int)face.botRight.y < avgHeight)
            compareVertex = 0;
        else if ((int)face.botRight.y > avgHeight)
            compareVertex = 2;
        else
            Debug.LogError("wtf");

        returnMe += compareVertex * 10;

        //botLeft
        compareVertex = -1;

        if ((int)face.botLeft.y == avgHeight)
            compareVertex = 1;
        else if ((int)face.botLeft.y < avgHeight)
            compareVertex = 0;
        else if ((int)face.botLeft.y > avgHeight)
            compareVertex = 2;
        else
            Debug.LogError("wtf");

        returnMe += compareVertex * 1;

        if (returnMe == 1000)
        {
            Debug.Log("returnme = " + returnMe + " with " + face.GetVectors()[0].y + "/" + face.GetVectors()[1].y + "/" + face.GetVectors()[2].y + "/" + face.GetVectors()[3].y + " and avgHeight = " + avgHeight);
        }

        return returnMe;
    }

    public void ToggleTriangleFlip()
    {
        if (!flippedTriangles)
        {
            flippedTriangles = true;
        }
        else
        {
            flippedTriangles = false;
        }
    }
}