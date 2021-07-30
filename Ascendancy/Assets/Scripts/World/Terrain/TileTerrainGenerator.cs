using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTerrainGenerator : MonoBehaviour
{
    public Tile[,] tilemap;
    public float noiseScale;

    private World world;
    private HeightMapGenerator heightmapGen;
    private int worldSize;

    [Header("Terrain Features")]
    public Hills hills;
    public Lakes lakes;
    public Rivers rivers;
    public MountainRanges mountains;
    public TerrainTypeFinalization typeFinalization;


    public Tile[,] GenerateTileMap()
    {
        world = World.Instance;
        worldSize = world.worldSize;
        heightmapGen = GetComponent<HeightMapGenerator>();
        float[,] noisemap = heightmapGen.GenerateHeightMap(worldSize, worldSize);

        Debug.Assert(world != null, "World is null!");
        Debug.Assert(heightmapGen != null, "HeightmapGen is null!");

        TerrainFeature.heightMapGenerator = heightmapGen;


        tilemap = new Tile[worldSize, worldSize];

        for (int x = 0; x < worldSize; x++)
            for (int y = 0; y < worldSize; y++)
            {
                Tile t = new Tile(x, y, 0, noisemap[x, y]);
                t.terrainType = TerrainType.GRASS;
                tilemap[x, y] = t;
            }

        // add the hills
        mountains.AddFeature(ref tilemap);
        hills.AddFeature(ref tilemap);
        lakes.AddFeature(ref tilemap);
        rivers.AddFeature(ref tilemap);
        typeFinalization.AddFeature(ref tilemap);

        return tilemap;
        //float[,] noisemap = heightmapGen.GenerateNoiseMap(world.worldSize, world.worldSize, world.)
    }
}
