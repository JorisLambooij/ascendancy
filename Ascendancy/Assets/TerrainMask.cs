using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType { Land, Water, Cliff }
public class TerrainMap
{
    public float[,] heightmap;
    public TerrainType[,] terrainTypeMap;
    public bool[,] slopiness;

    public TerrainMap(int size)
    {
        heightmap = new float[size, size];
        terrainTypeMap = new TerrainType[size, size];
        slopiness = new bool[size, size];
    }
    
    public float[,] AddHeightmap(float[,] hm2)
    {
        Debug.Assert(heightmap.GetLength(0) == hm2.GetLength(0), "Heightmaps not of identical Width!");
        Debug.Assert(heightmap.GetLength(1) == hm2.GetLength(1), "Heightmaps not of identical Height!");

        for (int x = 0; x < heightmap.GetLength(0); x++)
            for (int y = 0; y < heightmap.GetLength(1); y++)
            {
                heightmap[x, y] += hm2[x, y];
            }
        return heightmap;
    }
}

[RequireComponent(typeof(World))]
[RequireComponent(typeof(HeightMapGenerator))]
public class TerrainMask : MonoBehaviour
{
    public bool hills;
    public bool lakes;


    public Vector2 seed;

    TerrainMap map;

    World world;
    HeightMapGenerator hmGen;
    
    public void SetAll(bool active)
    {
        hills = active;
        lakes = active;
    }

    public void RandomizeSeed()
    {
        seed = new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000));
    }

    public void ResetTerrainmap()
    {
        world = GetComponent<World>();
        hmGen = GetComponent<HeightMapGenerator>();
        map = new TerrainMap(world.worldSize);
    }
    
    private TerrainMap AddHills()
    {
        //float[,] hillsmap = hmGen.GenerateNoiseMap(world.worldSize, world.worldSize, seed, 3, 0.3f, 1);

        //map.AddHeightmap(hillsmap);

        return map;
    }

}
