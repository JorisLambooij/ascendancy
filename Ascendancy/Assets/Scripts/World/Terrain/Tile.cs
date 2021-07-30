using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum TerrainType { NONE, GRASS, ROCK, DIRT, SAND, SNOW, WATER};

public class Tile
{
    #region Internal data
    public Face face;
    public Face face2;
    public bool flippedTriangles { get; private set; } = false;
    public int Height { 
        get => height;
        set
        {
            height = value;
            SetFace();
        }
    }

    public TerrainType terrainType = 0;

    private int height;    //the idealized height of this tile

    public float rawHeight;
    public Vector2Int gradient;

    public float worldX, worldZ;
    #endregion
    public Tile(float worldX, float worldZ, int height, float rawHeight)
    {
        this.worldX = worldX;
        this.worldZ = worldZ;
        this.Height = height;
        this.rawHeight = rawHeight;
        SetFace();
    }

    //basic constructor
    public Tile(float worldX, float worldZ,  int height)
    {
        this.worldX = worldX;
        this.worldZ = worldZ;
        this.Height = height;
        SetFace();
    }

    protected void SetFace()
    {
        face = new Face
        {
            topLeft = new Vector3(worldX - 0.5f, Height, worldZ + 0.5f), //top left
            topRight = new Vector3(worldX + 0.5f, Height, worldZ + 0.5f), //up right
            botRight = new Vector3(worldX + 0.5f, Height, worldZ - 0.5f), //down right
            botLeft = new Vector3(worldX - 0.5f, Height, worldZ - 0.5f) //down left
        };
    }


    public virtual bool FlatLand()
    {
        return face.botLeft.y == face.botRight.y && face.botRight.y == face.topRight.y && face.topRight.y == face.topLeft.y;
    }

    public Face[] GetFaces()
    {
        Face[] faces = new Face[1] { face };
        return faces;
    }

    public void SetHeightWithoutAffectingFace(int newHeight)
    {
        this.height = newHeight;
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