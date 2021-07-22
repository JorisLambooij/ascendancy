//there is only ever one of this script, and it holds ALL the world data. Actuall meshes are drawn by chunks, of which there could be several

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    // Different debug display modes.
    public enum DisplayMode { Height, Color, Gradient, Monochrome, Water };
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
    private int parallelizationBatchSize = 1024;

    public float waterLevel = -1.2f;
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
    public List<int> highlightedTiles;


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

    /// <summary>
    /// Texture that functions as terrain alpha.
    /// </summary>
    private Texture2D terrainMaskTexture;

    /// <summary>
    /// Texture that functions as terrain alpha.
    /// </summary>
    private Material terrainMaterial;

    /// <summary>
    /// Used to control FOW.
    /// </summary>
    public FogOfWarHandler fowHandler;

    /// <summary>
    /// Terrain Color Texture mainly used for minimap
    /// </summary>
    public Texture2D TerrainColorTexture;


    public void Awake()
    {
        Instance = this;
        Chunk[] existingChunks = ChunkCollector.GetComponentsInChildren<Chunk>();
        foreach (Chunk c in existingChunks)
            Destroy(c.gameObject, 0.1f);

        #region initialize mask textures
        //terrainMask
        terrainMaskTexture = new Texture2D(worldSize, worldSize);
        Color[] whitePixels = Enumerable.Repeat(Color.white, worldSize * worldSize).ToArray();
        terrainMaskTexture.SetPixels(whitePixels);
        terrainMaskTexture.Apply();
        terrainMaskTexture.wrapMode = TextureWrapMode.Clamp;
        terrainMaskTexture.filterMode = FilterMode.Point;

        //waterAlpha
        Texture2D waterAlpha = new Texture2D(worldSize, worldSize);
        waterAlpha.SetPixels(whitePixels);
        waterAlpha.Apply();
        waterAlpha.wrapMode = TextureWrapMode.Clamp;
        waterAlpha.filterMode = FilterMode.Point;
        #endregion

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        CreateWorld();
        sw.Stop();
        Debug.Log("CreateWorld() finished in " + sw.ElapsedMilliseconds + " ms.");


        terrainMaterial = chunks[0, 0].GetComponent<Renderer>().sharedMaterial;
        terrainMaterial.SetTexture("_mask", terrainMaskTexture);
    }



    public void CreateWorld()
    {
        //First, delete old world
        DestroyWorld();


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
        GenerateTerrain();

        AdditiveSmoothing additiveSmoothing = new AdditiveSmoothing();
        map = additiveSmoothing.Run(map, parallelizationBatchSize);

        FlipTriangleSmoothing triangleSmoothing = new FlipTriangleSmoothing();
        map = triangleSmoothing.Run(map, parallelizationBatchSize);

        CliffFilling cliffFill = new CliffFilling();
        map = cliffFill.Run(map, parallelizationBatchSize);

        map = triangleSmoothing.Run(map, parallelizationBatchSize);


        //2nd map iteration with methods
        for (int x = 0; x < map.GetLength(0); x++)
            for (int y = 0; y < map.GetLength(1); y++)
            {
                GenerateTerrainTypes(x, y);
                //generate texture
                //terrainTexture.SetPixels(x * textureTilesize, y * textureTilesize, textureTilesize, textureTilesize, tileArray[(int)map[x, y].terrainType].GetPixels());
                //vertex colors
                //UpdateVertexColors(x, y);
                colormap[x, y] = GetColorForType(map[x, y].terrainType);

            }

        //apply texture
        //terrainTexture.Apply();

        //tell all the chunks to draw their share of the mesh
        for (int x = 0; x < chunks.GetLength(0); x++)
            for (int z = 0; z < chunks.GetLength(1); z++)
            {
                chunks[x, z] = GenerateChunk(x, z);
                chunks[x, z].GetComponent<Renderer>().material.SetTexture("_mask", terrainMaskTexture);
            }

        //chunks[0, 0].GetComponent<MeshRenderer>().sharedMaterial.SetTexture("Texture2D_AA075013", terrainTexture);

        waterPlane.transform.position = new Vector3(worldSize * tileSize / 2, waterLevel, worldSize * tileSize / 2);
        float size = worldSize / 9.86f;
        waterPlane.transform.localScale = new Vector3(size * tileSize, 1, size * tileSize);

        Color[] colors = new Color[colormap.GetLength(0) * colormap.GetLength(1)];

        float height = 0f;
        for (int x = 0; x < map.GetLength(0); x++)
            for (int y = 0; y < map.GetLength(1); y++)
            {
                height = map[x, y].height;
                colors[x + y * map.GetLength(0)] = colormap[x, y];
                if (height < -1f)
                {
                    colors[x + y * map.GetLength(0)] = Color.blue;
                }
                else //heightmap
                {
                    colors[x + y * map.GetLength(0)] = Color.Lerp(colors[x + y * map.GetLength(0)], Color.white, (height + 1f) / 10f);
                }

                //colors[x+y* map.GetLength(0)] = Color.Lerp(Color.white, Color.red, map[x, y].height - 1f);
            }

        TerrainColorTexture = new Texture2D(colormap.GetLength(0), colormap.GetLength(1));
        TerrainColorTexture.SetPixels(colors);
        TerrainColorTexture.Apply();
    }

    Chunk GenerateChunk(int startX, int startZ)
    {
        int chunkSize = Chunk.chunkSize;
        GameObject chunkGO = Instantiate(chunkPrefab, ChunkCollector);
        //chunkGO.transform.position = new Vector3(startX, 0, startZ) * 64 * tileSize;
        Chunk chunk = chunkGO.GetComponent<Chunk>();
        chunk.chunkIndex = new Vector2Int(startX, startZ);

        Tile[,] chunkTilemap = new Tile[chunkSize, chunkSize];
        Color32[,] chunkColormap = new Color32[chunkSize + 2, chunkSize + 2];

        for (int x = 0; x < chunkSize; x++)
            for (int z = 0; z < chunkSize; z++)
            {
                chunkTilemap[x, z] = map[chunkSize * startX + x, chunkSize * startZ + z];
            }

        // in order to fix the "corner tiles", we need some neighbor info. this gets tricky at a chunk border,
        // so each chunk gets the info from the first row of each of its neighbors
        for (int x = 0; x < chunkSize + 2; x++)
            for (int z = 0; z < chunkSize + 2; z++)
            {
                int u = Mathf.Clamp(chunkSize * startX + x - 1, 0, colormap.GetLength(0) - 1);
                int v = Mathf.Clamp(chunkSize * startZ + z - 1, 0, colormap.GetLength(1) - 1);
                chunkColormap[x, z] = colormap[u, v];
            }

        chunk.Initialize(chunkTilemap, chunkColormap, tintFlippedTiles, highlightedTiles);
        return chunk;
    }

    public void GenerateTerrainTypes(int x, int y)
    {
        TerrainType tileType;


        switch (Mathf.RoundToInt(map[x, y].height))
        {
            case int n when (n < -1):
                tileType = TerrainType.WATER;
                break;
            case int n when (n < 0):
                tileType = TerrainType.SAND;
                break;
            case int n when (n < 1):
                tileType = TerrainType.GRASS;
                break;
            case int n when (n < 2):
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
                int y = Mathf.RoundToInt(heightmap[u, v] * heightResolution);

                //Debug.Assert(wd < heightmap.GetLength(0) && hg < heightmap.GetLength(1), "Error. WD/HG: " + wd + " " + hg);
                heightmap[dx, dy] = y;


                map[dx, dy] = new Tile(dx, dy);
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


    #region auxiliary functions
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

    public float GetHeight(Vector2 pos)
    {
        Vector2Int v = IntVector(pos);

        Debug.Assert(v.x >= 0 && v.x < map.GetLength(0), "World.GetHeight: Tile Index out of range (X=" + v.x + ")");
        Debug.Assert(v.y >= 0 && v.y < map.GetLength(1), "World.GetHeight: Tile Index out of range (Y=" + v.y + ")");

        return map[v.x, v.y].height;
    }

    public bool IsAreaFlat(Vector2Int pos, Vector2Int dimensions)
    {
        int halfX = dimensions.x / 2;
        int halfY = dimensions.y / 2;

        // If a single tile is not flat, return false.
        for (int x = 0; x < dimensions.x; x++)
            for (int y = 0; y < dimensions.y; y++)
            {
                int finalX = pos.x + x - halfX, finalY = pos.y + y - halfY;

                if (finalX < 0 || finalX >= map.GetLength(0) || finalY < 0 || finalY >= map.GetLength(1))
                    return false;
                if (!map[finalX, finalY].FlatLand())
                    return false;
            }
        return true;
    }

    public Tile GetTile(Vector3 pos)
    {
        Vector2Int v = IntVector(pos);
        if (v.x < 0 || v.x >= map.GetLength(0) || v.y < 0 ||  v.y >= map.GetLength(1))
            return null;

        return map[v.x, v.y];
    }

    public Vector2Int IntVector(Vector3 v)
    {
        float x = v.x / tileSize;
        float y = v.z / tileSize;

        int x_int = Mathf.RoundToInt(x);
        int y_int = Mathf.RoundToInt(y);

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

        terrainMaterial.SetFloat("_grid", gridfloat);
    }

    public void SetTileVisible(int x, int y, bool visible)
    {
        if (visible)
            terrainMaskTexture.SetPixel(x, y, Color.white);
        else
            terrainMaskTexture.SetPixel(x, y, Color.black);

        terrainMaskTexture.Apply();

        terrainMaterial.SetTexture("_mask", terrainMaskTexture);

    }

    public void SetTileVisible(Vector3 pos, bool visible)
    {
        Vector2Int v = IntVector(pos);
        SetTileVisible(v.x, v.y, visible);
    }
    #endregion
}