//there is only ever one of this script, and it holds ALL the world data. Actuall meshes are drawn by chunks, of which there could be several

using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;


public class World : MonoBehaviour_Singleton
{
    public enum DisplayMode { Height, Color, Gradient, Monochrome };    //Was macht das?
    public DisplayMode displayMode = DisplayMode.Color;                 //Können die beiden weg? :D

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
    public float tileSize = 1f; //meters per side of each tiles
    public float heightScale = 1f;   //meters of elevation each new level gives us
    public float heightResolution = 1f; // amount of different levels of elevation
    #endregion

    public GameObject chunkPrefab;
    public LocalNavMeshBuilder navMeshBuilder;

    [Header("Misc References")]
    public Transform ChunkCollector;
    //public bool useHeightmapAsTexture = false;
    public Texture2D tex;
    public Transform waterPlane;
    [Tooltip("ERROR, GRASS, ROCK, DIRT, SAND, WATER")]
    public Color32[] terrainColors = new Color32[6];

    [Header("DevTools")]
    public bool tintFlippedTiles = false;


    /// <summary>
    /// Contains the information about the height of the tiles.
    /// </summary>
    private float[,] heightmap;

    /// <summary>
    /// Set of all the tiles that make up the world
    /// </summary>
    private Tile[,] map;
    private Color32[,] colormap;

    /// <summary>
    /// Set of all the chunks used to draw the world.
    /// </summary>
    private Chunk[,] chunks;

    public void Awake()
    {
        base.Start();

        CreateWorld();

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
        colormap = new Color32[worldSize, worldSize];
        //terrainTexture = new Texture2D(worldSize * textureTilesize, worldSize * textureTilesize);

        chunks = new Chunk[numberOfChunks, numberOfChunks];

        //generate the terrain!
        Debug.Log("Building Terrain");
        GenerateTerrain();

        //1st map iteration with methods
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                AdditiveSmoothing(x, y);
                FlipTriangleSmoothing(x, y);
            }
        }

        //2nd map iteration with methods
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                GenerateTerrainTypes(x, y);

                FillCliffs(x, y);
                //generate texture
                //terrainTexture.SetPixels(x * textureTilesize, y * textureTilesize, textureTilesize, textureTilesize, tileArray[(int)map[x, y].terrainType].GetPixels());
                //vertex colors
                //UpdateVertexColors(x, y);
                colormap[x, y] = GetColorForType(map[x, y].terrainType);
            }
        }

        //apply texture
        //terrainTexture.Apply();

        //tell all the chunks to draw their share of the mesh
        for (int x = 0; x < chunks.GetLength(0); x++)
            for (int z = 0; z < chunks.GetLength(1); z++)
            {
                chunks[x, z] = GenerateChunk(x, z);
            }

        //chunks[0, 0].GetComponent<MeshRenderer>().sharedMaterial.SetTexture("Texture2D_AA075013", terrainTexture);

        waterPlane.transform.position = new Vector3(worldSize * tileSize / 2, -4.25f, worldSize * tileSize / 2);
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
        Color32[,] chunkColormap = new Color32[chunkSize, chunkSize];

        for (int x = 0; x < chunkSize; x++)
            for (int z = 0; z < chunkSize; z++)
            {
                chunkTilemap[x, z] = map[chunkSize * startX + x, chunkSize * startZ + z];
                chunkColormap[x, z] = colormap[chunkSize * startX + x, chunkSize * startZ + z];
            }

        chunk.Initialize(chunkTilemap, chunkColormap, tintFlippedTiles);
        return chunk;
    }

    public void FillCliffs(int x, int y)
    {
        //Vector2 tCliff = new Vector2(0, 0);
        Tile neighbor;
        Tile me;
        TileCliff cliff;

        me = map[x, y];

        //check left
        if (x > 0)
        {
            neighbor = map[x - 1, y];

            if (neighbor.face.topRight.y < me.face.topLeft.y || neighbor.face.botRight.y < me.face.botLeft.y)
            {
                //check if tile is already a cliff
                if (!(map[x, y].GetType() == typeof(TileCliff)))
                {
                    map[x, y] = new TileCliff(map[x, y]);
                }
                cliff = (TileCliff)map[x, y];

                cliff.leftCliff = new Face();
                cliff.leftCliff.topLeft = me.face.topLeft; //top left
                cliff.leftCliff.topRight = me.face.botLeft; //up right
                cliff.leftCliff.botRight = neighbor.face.botRight; //down right
                cliff.leftCliff.botLeft = neighbor.face.topRight; //down left

                map[x, y] = cliff;
            }
        }

        //check above
        if (y < map.GetLength(1) - 1)
        {
            neighbor = map[x, y + 1];

            if (neighbor.face.botLeft.y < me.face.topLeft.y || neighbor.face.botRight.y < me.face.topRight.y)
            {
                //check if tile is already a cliff
                if (!(map[x, y].GetType() == typeof(TileCliff)))
                {
                    map[x, y] = new TileCliff(map[x, y]);
                }
                cliff = (TileCliff)map[x, y];

                cliff.topCliff = new Face();
                cliff.topCliff.topLeft = me.face.topRight; //top left
                cliff.topCliff.topRight = me.face.topLeft; //up right
                cliff.topCliff.botRight = neighbor.face.botLeft; //down right
                cliff.topCliff.botLeft = neighbor.face.botRight; //down left

                map[x, y] = cliff;
            }
        }

        //check right
        if (x < map.GetLength(0) - 1)
        {
            neighbor = map[x + 1, y];

            if (neighbor.face.topLeft.y < me.face.topRight.y || neighbor.face.botLeft.y < me.face.botRight.y)
            {
                //check if tile is already a cliff
                if (!(map[x, y].GetType() == typeof(TileCliff)))
                {
                    map[x, y] = new TileCliff(map[x, y]);
                }
                cliff = (TileCliff)map[x, y];

                cliff.rightCliff = new Face();
                cliff.rightCliff.topLeft = me.face.botRight; //top left
                cliff.rightCliff.topRight = me.face.topRight; //up right
                cliff.rightCliff.botRight = neighbor.face.topLeft; //down right
                cliff.rightCliff.botLeft = neighbor.face.botLeft; //down left

                map[x, y] = cliff;
            }
        }

        //check below
        if (y > 0)
        {
            neighbor = map[x, y - 1];

            if (neighbor.face.topLeft.y < me.face.botLeft.y || neighbor.face.topRight.y < me.face.botRight.y)
            {
                //check if tile is already a cliff
                if (!(map[x, y].GetType() == typeof(TileCliff)))
                {
                    map[x, y] = new TileCliff(map[x, y]);
                }
                cliff = (TileCliff)map[x, y];

                cliff.botCliff = new Face();
                cliff.botCliff.topLeft = me.face.botLeft; //top left
                cliff.botCliff.topRight = me.face.botRight; //up right
                cliff.botCliff.botRight = neighbor.face.topRight; //down right
                cliff.botCliff.botLeft = neighbor.face.topLeft; //down left

                map[x, y] = cliff;
            }
        }
    }

    //flips the triangles of diagonal slopes in two directions
    public void FlipTriangleSmoothing(int x, int y)
    {
        Tile me;

        me = map[x, y];
        int tileType = me.GetTileType();

        //topLeft - flip
        if (tileType == 111 || tileType == 121 || tileType == 2111 || tileType == 2101)
        {
            if (!(me).flippedTriangles)
                (me).ToggleTriangleFlip();
        }

        //topRight - unflip
        if (tileType == 1011 || tileType == 1012 || tileType == 1211 || tileType == 1210)
        {
            if ((me).flippedTriangles)
                (me).ToggleTriangleFlip();
        }

        //botRight - flip
        if (tileType == 1101 || tileType == 2101 || tileType == 1121 || tileType == 121)
        {
            if (!(me).flippedTriangles)
                (me).ToggleTriangleFlip(); ;
        }

        //botLeft - unflip
        if (tileType == 1110 || tileType == 1210 || tileType == 1112 || tileType == 1012)
        {
            if ((me).flippedTriangles)
                (me).ToggleTriangleFlip();
        }

    }

    public void AdditiveSmoothing(int x, int y)
    {
        Tile Neighbor;
        bool tl = false;
        bool tr = false;
        bool bl = false;
        bool br = false;
        Tile me = map[x, y];

        #region direct
        //check left
        if (x > 0)
        {
            Neighbor = map[x - 1, y];

            if (Neighbor.face.topRight.y == me.face.topLeft.y + 1 && Neighbor.face.botRight.y == me.face.botLeft.y + 1)
            {
                tl = true;
                bl = true;
            }
        }

        //check above
        if (y < map.GetLength(1) - 1)
        {
            Neighbor = map[x, y + 1];

            if (Neighbor.face.botLeft.y == me.face.topLeft.y + 1 && Neighbor.face.botRight.y == me.face.topRight.y + 1)
            {
                tl = true;
                tr = true;
            }
        }

        //check right
        if (x < map.GetLength(0) - 1)
        {
            Neighbor = map[x + 1, y];

            if (Neighbor.face.topLeft.y == me.face.topRight.y + 1 && Neighbor.face.botLeft.y == me.face.botRight.y + 1)
            {
                tr = true;
                br = true;
            }
        }

        //check below
        if (y > 0)
        {
            Neighbor = map[x, y - 1];

            if (Neighbor.face.topLeft.y == me.face.botLeft.y + 1 && Neighbor.face.topRight.y == me.face.botRight.y + 1)
            {
                bl = true;
                br = true;
            }
        }
        #endregion

        #region diagonal
        //check topLeft
        if (x > 0 && y < map.GetLength(1) - 1)
        {
            Neighbor = map[x - 1, y + 1];

            if (Neighbor.face.botRight.y > me.face.topLeft.y)
            {
                tl = true;
            }
        }

        //check topRight
        if (y < map.GetLength(1) - 1 && x < map.GetLength(0) - 1)
        {
            Neighbor = map[x + 1, y + 1];

            if (Neighbor.face.botLeft.y > me.face.topRight.y)
            {
                tr = true;
            }
        }

        //check botRight
        if (y > 0 && x < map.GetLength(0) - 1)
        {
            Neighbor = map[x + 1, y - 1];

            if (Neighbor.face.topLeft.y > me.face.botRight.y)
            {
                br = true;
            }
        }

        //check botLeft
        if (y > 0 && x > 0)
        {
            Neighbor = map[x - 1, y - 1];

            if (Neighbor.face.topRight.y > me.face.botLeft.y)
            {
                bl = true;
            }
        }


        #endregion


        if (tl)
        {
            map[x, y].face.topLeft.y += 1;
        }
        if (tr)
        {
            map[x, y].face.topRight.y += 1;
        }
        if (br)
        {
            map[x, y].face.botRight.y += 1;
        }
        if (bl)
        {
            map[x, y].face.botLeft.y += 1;
        }


    }

    public void GenerateTerrainTypes(int x, int y)
    {
        TerrainType tileType;


        switch ((int)map[x, y].height)
        {
            case int n when (n < -5):
                tileType = TerrainType.WATER;
                break;
            case int n when (n < -0):
                tileType = TerrainType.SAND;
                break;
            case int n when (n < 5):
                tileType = TerrainType.GRASS;
                break;
            case int n when (n < 10):
                tileType = TerrainType.DIRT;
                break;
            default:
                tileType = TerrainType.ROCK;
                break;
        }
        map[x, y].terrainType = tileType;

    }

    public void RegenerateTexture()
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                GenerateTerrainTypes(x, y);

                //generate texture
                //terrainTexture.SetPixels(x * textureTilesize, y * textureTilesize, textureTilesize, textureTilesize, tileArray[(int)map[x, y].terrainType].GetPixels());
            }
        }
    }

    public Color32 GetColorForType(TerrainType t)
    {
        switch (t)
        {
            case TerrainType.WATER:
                return terrainColors[5];
            case TerrainType.SAND:
                return terrainColors[4];
            case TerrainType.GRASS:
                return terrainColors[1];
            case TerrainType.DIRT:
                return terrainColors[3];
            case TerrainType.ROCK:
                return terrainColors[2];
            default:
                return terrainColors[0];
        }
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


                map[dx, dy] = new Tile();
                map[dx, dy].face = new Face
                {
                    topLeft = new Vector3(dx - 0.5f, y, dy + 0.5f), //top left
                    topRight = new Vector3(dx + 0.5f, y, dy + 0.5f), //up right
                    botRight = new Vector3(dx + 0.5f, y, dy - 0.5f), //down right
                    botLeft = new Vector3(dx - 0.5f, y, dy - 0.5f) //down left
                };
                map[dx, dy].height = y;
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