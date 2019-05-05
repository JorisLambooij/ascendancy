using UnityEngine;
using System.Collections;

public class Tile
{

    public Vector3 upperLeft;
    public Vector3 upperRight;
    public Vector3 lowerRight;
    public Vector3 lowerLeft;

    public float height;    //the idealized height of this tile
    public bool isSlope = false;

    public float tileSize = 3f; //tile size in meters

    //cached things
    private float centerX;
    private float centerZ;

    //basic constructor
    public Tile(float centerX, float centerZ, float height, float size)
    {

        this.height = height;
        this.centerX = centerX;
        this.centerZ = centerZ;
        //make sure we're set to the right size
        tileSize = size;
        float halfSize = size / 2f;

        //setup the vectors!
        upperLeft = new Vector3((centerX * tileSize), height, (centerZ * tileSize) + tileSize);
        upperRight = new Vector3((centerX * tileSize) + tileSize, height, (centerZ * tileSize) + tileSize);
        lowerRight = new Vector3((centerX * tileSize) + tileSize, height, (centerZ * tileSize));
        lowerLeft = new Vector3((centerX * tileSize), height, (centerZ * tileSize));

    }

    public void recalculate()
    {
        //reset our vectors
        upperLeft = new Vector3((centerX * tileSize), height, (centerZ * tileSize) + tileSize);
        upperRight = new Vector3((centerX * tileSize) + tileSize, height, (centerZ * tileSize) + tileSize);
        lowerRight = new Vector3((centerX * tileSize) + tileSize, height, (centerZ * tileSize));
        lowerLeft = new Vector3((centerX * tileSize), height, (centerZ * tileSize));
    }

    public void ReSetStats()
    {
        height = Mathf.Max(upperRight.y, upperLeft.y, lowerLeft.y, lowerRight.y);

        if ((upperLeft.y + upperRight.y + lowerLeft.y + lowerRight.y) / 4f != height)
        {
            isSlope = true;
        }
    }
}