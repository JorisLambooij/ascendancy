//there is only ever one of this script, and it holds ALL the world data. Actuall meshes are drawn by chunks, of which there could be several

using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour_Singleton
{
    public enum DisplayMode { Height, Color, Gradient, Monochrome };
    public DisplayMode displayMode = DisplayMode.Color;

    #region Tweakables
    [Header("Tweakables")]
    /// <summary>
    /// Tiles per side of the world.
    /// </summary>
    public int worldSize = 64;

    public float EffectiveWorldSize
    {
        get { return worldSize * tileSize; }
    }

    /// <summary>
    /// Number of chunk per side of the world.
    /// </summary>
    [HideInInspector]
    public int numberOfChunks = 2;

    public float noiseScale = 2;
    public float tileSize = 5f; //meters per side of each tiles
    public float heightScale = 1f;   //meters of elevation each new level gives us
    public float heightResolution = 1f; // amount of different levels of elevation
    #endregion

    public GameObject chunkPrefab;
    public LocalNavMeshBuilder navMeshBuilder;

    //public float seaLevel = 0;

    [Header("Misc References")]
    public Transform ChunkCollector;
    public GameObject fow_plane;
    public Texture2D tex;
    public Transform waterPlane;

    /// <summary>
    /// Contains the information about the height of the tiles.
    /// </summary>
    private float[,] heightmap;

    /// <summary>
    /// Set of all the tiles that make up the world
    /// </summary>
    private Tile[,] map;

    /// <summary>
    /// Set of all the chunks used to draw the world.
    /// </summary>
    private Chunk[,] chunks;
    
    public void Awake()
    {
        base.Start();

        CreateWorld();

        //resize fow_plane
        if (fow_plane == null) { }
        //Debug.LogError("FoW_Plane not found, no fog for you! Go fog yourself!");
        else
        {
            //TODO Calculate stuff instead of estimating '25.5'
            fow_plane.transform.localScale = new Vector3(25.5f, 1, 25.5f);
            fow_plane.transform.position = new Vector3(worldSize, fow_plane.transform.position.y, worldSize);
        }
    }

    public void CreateWorld()
    {
        //TODO: Fix it so that chunks can be larger than 64
        numberOfChunks = Mathf.CeilToInt(worldSize / 64);
        Chunk.chunkSize = 64;

        HeightMapGenerator heightMapGenerator = GetComponent<HeightMapGenerator>();
        heightmap = heightMapGenerator.GenerateHeightMap(worldSize, worldSize, noiseScale);
        //heightmap = heightMapGenerator.AmplifyCliffs();

        //initiate things
        map = new Tile[worldSize, worldSize];

        chunks = new Chunk[numberOfChunks, numberOfChunks];

        //generate the terrain!
        Debug.Log("Building Terrain");
        GenerateTerrain();

        //tell all the chunks to draw their share of the mesh
        for (int x = 0; x < chunks.GetLength(0); x++)
            for (int z = 0; z < chunks.GetLength(1); z++)
            {
                chunks[x, z] = GenerateChunk(x, z);
            }

        GenerateTexture(heightMapGenerator);

        waterPlane.transform.position = new Vector3(worldSize * tileSize / 2, -4, worldSize * tileSize / 2);
        float size = worldSize / 9.86f;
        waterPlane.transform.localScale = new Vector3(size * tileSize, 1, size * tileSize);

        try
        {
            navMeshBuilder.UpdateNavMesh(false);
        }
        catch (System.Exception e)
        {
            if (Application.IsPlaying(this))
                throw e;
        }
        Debug.Log("World Generated: ");
    }

    Chunk GenerateChunk(int startX, int startZ)
    {
        int chunkSize = Chunk.chunkSize;
        GameObject chunkGO = Instantiate(chunkPrefab, ChunkCollector);
        //chunkGO.transform.position = new Vector3(startX, 0, startZ) * 64 * tileSize;
        Chunk chunk = chunkGO.GetComponent<Chunk>();
        chunk.chunkIndex = new Vector2Int(startX, startZ);

        Tile[,] chunkTilemap = new Tile[chunkSize, chunkSize];

        for (int x = 0; x < chunkSize; x++)
            for (int z = 0; z < chunkSize; z++)
            {
                chunkTilemap[x, z] = map[chunkSize * startX + x, chunkSize * startZ + z];
            }

        chunk.Initialize(chunkTilemap);
        return chunk;
    }

    public void AdditiveSmoothing()
    {
        Tile Neighbor;
        bool tl = false;
        bool tr = false;
        bool bl = false;
        bool br = false;
        Tile me;

        for (int wd = 0; wd < map.GetLength(0); wd++)
        {
            for (int hg = 0; hg < map.GetLength(1); hg++)
            {
                me = map[wd, hg];
                tl = false;
                tr = false;
                bl = false;
                br = false;

                #region direct
                //check left
                if (wd > 0)
                {
                    Neighbor = map[wd - 1, hg];

                    if (Neighbor.topRight.y > me.topLeft.y && Neighbor.botRight.y > me.botLeft.y)
                    {
                        tl = true;
                        bl = true;
                    }
                }

                //check above
                if (hg < map.GetLength(1) - 1)
                {
                    Neighbor = map[wd, hg + 1];

                    if (Neighbor.botLeft.y > me.topLeft.y && Neighbor.botRight.y > me.topRight.y)
                    {
                        tl = true;
                        tr = true;
                    }
                }

                //check right
                if (wd < map.GetLength(0) - 1)
                {
                    Neighbor = map[wd + 1, hg];

                    if (Neighbor.topLeft.y > me.topRight.y && Neighbor.botLeft.y > me.botRight.y)
                    {
                        tr = true;
                        br = true;
                    }
                }

                //check below
                if (hg > 0)
                {
                    Neighbor = map[wd, hg - 1];

                    if (Neighbor.topLeft.y > me.botLeft.y && Neighbor.topRight.y > me.botRight.y)
                    {
                        bl = true;
                        br = true;
                    }
                }
                #endregion

                #region diagonal
                //check topLeft
                if (wd > 0 && hg < map.GetLength(1) - 1)
                {
                    Neighbor = map[wd - 1, hg + 1];

                    if (Neighbor.botRight.y > me.topLeft.y)
                    {
                        tl = true;
                    }
                }

                //check topRight
                if (hg < map.GetLength(1) - 1 && wd < map.GetLength(0) - 1)
                {
                    Neighbor = map[wd + 1, hg + 1];

                    if (Neighbor.botLeft.y > me.topRight.y)
                    {
                        tr = true;
                    }
                }

                //check botRight
                if (hg > 0 && wd < map.GetLength(0) - 1)
                {
                    Neighbor = map[wd + 1, hg - 1];

                    if (Neighbor.topLeft.y > me.botRight.y)
                    {
                        br = true;
                    }
                }

                //check botLeft
                if (hg > 0 && wd > 0)
                {
                    Neighbor = map[wd - 1, hg - 1];

                    if (Neighbor.topRight.y > me.botLeft.y)
                    {
                        bl = true;
                    }
                }


                #endregion


                if (tl)
                {
                    map[wd, hg].topLeft.y += 1;
                }
                if (tr)
                {
                    map[wd, hg].topRight.y += 1;
                }
                if (br)
                {
                    map[wd, hg].botRight.y += 1;
                }
                if (bl)
                {
                    map[wd, hg].botLeft.y += 1;
                }

            }
        }
    }

    public void GenerateTexture(HeightMapGenerator heightMapGenerator)
    {
        chunks[0, 0].GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_TerrainTexture", heightMapGenerator.WorldTexture(heightmap, displayMode));
    }

    public void DestroyWorld()
    {
        if (chunks != null)
            for (int i = 0; i < chunks.GetLength(0); i++)
                for (int j = 0; j < chunks.GetLength(1); j++)
                    if (chunks[i, j] != null)
                        DestroyImmediate(chunks[i, j].gameObject);
    }


    void GenerateTerrain()
    {
        for (int dx = 0; dx < worldSize; dx++)
            for (int dy = 0; dy < worldSize; dy++)
            {
                int u = Mathf.Min(heightmap.GetLength(0) - 1, dx);
                int v = Mathf.Min(heightmap.GetLength(1) - 1, dy);
                //get y and insert to int heightmap for later
                int y = Mathf.RoundToInt(heightmap[u, v] * 10);

                //Debug.Assert(wd < heightmap.GetLength(0) && hg < heightmap.GetLength(1), "Error. WD/HG: " + wd + " " + hg);
                heightmap[dx, dy] = y;


                map[dx, dy] = new Tile
                {
                    topLeft = new Vector3(dx - 0.5f, y, dy + 0.5f), //top left
                    topRight = new Vector3(dx + 0.5f, y, dy + 0.5f), //up right
                    botRight = new Vector3(dx + 0.5f, y, dy - 0.5f), //down right
                    botLeft = new Vector3(dx - 0.5f, y, dy - 0.5f) //down left
                };
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
        return map[v.x, v.y].FlatLand;
    }

    public bool IsAreaFlat(Vector3 pos, Vector2Int dimensions)
    {
        Vector2Int v = IntVector(pos);

        int halfX = dimensions.x / 2;
        int halfY = dimensions.y / 2;

        // If a single tile is not flat, return false.
        for (int x = 0; x < dimensions.x; x++)
            for (int y = 0; y < dimensions.y; y++)
            {
                int finalX = v.x + x - halfX, finalY = v.y + y - halfY;

                if (!map[finalX, finalY].FlatLand)
                    return false;
            }
        return true;
    }

    public Tile GetTile(Vector3 pos)
    {
        Vector2Int v = IntVector(pos);
        return map[v.x, v.y];
    }

    public Vector2Int IntVector(Vector3 v)
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