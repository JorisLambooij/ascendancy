//there is only ever one of this script, and it holds ALL the world data. Actuall meshes are drawn by chunks, of which there could be several

using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour_Singleton
{
    public enum DisplayMode { Height, Color };
    public DisplayMode displayMode = DisplayMode.Color;

    public GameObject chunkPrefab;
    public LocalNavMeshBuilder navMeshBuilder;

    //tweakables
    public int worldSize = 64;  //tiles per side of the world
    public int numberOfChunks = 2; // number of chunk per side of the world

    public float tileSize = 5f; //meters per side of each tiles
    public float heightScale = 1f;   //meters of elevation each new level gives us
    public float heightResolution = 1f; // amount of different levels of elevation

    //public float seaLevel = 0;

    public Transform ChunkCollector;

    public GameObject fow_plane;
    public Texture2D tex;

    private float[,] heightmap;
    private Tile[,] map;    //set of all the tiles that make up the world
    private Chunk[,] chunks; //set of all the chunks we're going to use to draw the world
                             //private GameObject chunkGO; //the instantiated Chunk

    protected void Awake()
    {
        base.Start();

        CreateWorld();

        //resize fow_plane
        if (fow_plane == null)
            Debug.LogError("FoW_Plane not found, no fog for you! Go fog yourself!");
        else
        {
            //TODO Calculate stuff instead of estimating '25.5'
            fow_plane.transform.localScale = new Vector3(25.5f, 1, 25.5f);
            fow_plane.transform.position = new Vector3(worldSize, fow_plane.transform.position.y, worldSize);
        }
    }

    public void CreateWorld()
    {
        HeightMapGenerator heightMapGenerator = GetComponent<HeightMapGenerator>();
        heightMapGenerator.GenerateHeightMap(worldSize, worldSize);
        heightmap = heightMapGenerator.AmplifyCliffs();

        //initiate things
        map = new Tile[worldSize, worldSize];

        chunks = new Chunk[numberOfChunks, numberOfChunks];
        for (int x = 0; x < worldSize; x++)
            for (int z = 0; z < worldSize; z++)
                map[x, z] = new Tile(x, z, 0f, tileSize);

        Debug.Log("Building Terrain");
        //generate the terrain!
        GenerateTerrain();

        //tell all the chunks to draw their share of the mesh
        for (int i = 0; i < chunks.GetLength(0); i++)
            for (int j = 0; j < chunks.GetLength(1); j++)
            {
                chunks[i, j] = GenerateChunk(i, j);
                chunks[i, j].DrawTiles(map);

                //chunks[i, j].GetComponent<MeshRenderer>().material.SetTexture("_TerrainTexture", heightMapGenerator.HeightTexture(i, j, worldSize / numberOfChunks, displayMode));
            }


        chunks[0, 0].GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_TerrainTexture", heightMapGenerator.WorldTexture(heightMapGenerator.noise, displayMode));

        navMeshBuilder.UpdateNavMesh(false);

    }

    public void DestroyWorld()
    {
        if (chunks != null)
            for (int i = 0; i < chunks.GetLength(0); i++)
                for (int j = 0; j < chunks.GetLength(1); j++)
                    if (chunks[i, j] != null)
                    DestroyImmediate(chunks[i, j].gameObject);
    }

    // Generate a chunk, fill it with necessary data and return the Chunk object
    Chunk GenerateChunk(int x, int z)
    {
        GameObject chunkGO = Instantiate(chunkPrefab, ChunkCollector);
        chunkGO.transform.position = new Vector3(x, 0, z) * (worldSize / numberOfChunks) * tileSize;
        Chunk chunk = chunkGO.GetComponent<Chunk>();
        chunk.Initialize();
        chunk.tileSize = tileSize;
        chunk.chunkSize = worldSize / numberOfChunks;
        chunk.chunkIndex = new Vector2Int(x, z);
        return chunk;
    }

    void GenerateTerrain()
    {
        //make some height
        for (int x = 0; x < worldSize; x++)
        {
            for (int z = 0; z < worldSize; z++)
            {
                Tile active = map[x, z];

                if (active.isSlope)
                {
                    break;
                }
                active.upperLeft = AdjustVector(active.upperLeft);
                active.upperRight = AdjustVector(active.upperRight);
                active.lowerRight = AdjustVector(active.lowerRight);
                active.lowerLeft = AdjustVector(active.lowerLeft);
                active.ReSetStats();
            }
        }
    }

    Vector3 AdjustVector(Vector3 input)
    {
        float newHeight = input.y;
        newHeight = (int)Height(input.x, input.z) * heightScale / heightResolution;
        return new Vector3(input.x, newHeight, input.z);
    }

    float Height(float x, float z)
    {
        int texX = (int)(x / worldSize * heightmap.GetLength(0));
        int texY = (int)(z / worldSize * heightmap.GetLength(1));

        texX = Mathf.RoundToInt(texX / tileSize);
        texY = Mathf.RoundToInt(texY / tileSize);

        texX = Mathf.Clamp(texX, 0, heightmap.GetLength(0) - 1);
        texY = Mathf.Clamp(texY, 0, heightmap.GetLength(1) - 1);

        return heightmap[texX, texY] * heightResolution;
    }
    
    public Collider GetCollider()
    {
        return chunks[0, 0].GetComponent<MeshCollider>();
    }

    public float GetHeight(Vector3 pos)
    {
        Vector2Int v = IntVector(pos);
        return map[v.x, v.y].height;
    }

    public bool IsFlat(Vector3 pos)
    {
        Vector2Int v = IntVector(pos);
        return map[v.x, v.y].flatLand;
    }

    public Tile GetTile(Vector3 pos)
    {
        Vector2Int v = IntVector(pos);
        return map[v.x, v.y];
    }

    private Vector2Int IntVector(Vector3 v)
    {
        float x = v.x / tileSize;
        float y = v.z / tileSize;

        int x_int = Mathf.FloorToInt(x);
        int y_int = Mathf.FloorToInt(y);

        return new Vector2Int(x_int, y_int);
    }

    public void ToggleGrid(bool on)
    {
        float gridfloat;

        if (on)
        {
            gridfloat = 1;
        }
        else
        {
            gridfloat = 0;
        }

        chunks[0, 0].GetComponent<Renderer>().material.SetFloat("_grid", gridfloat);
    }
}