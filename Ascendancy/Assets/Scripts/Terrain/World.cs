﻿//there is only ever one of this script, and it holds ALL the world data. Actuall meshes are drawn by chunks, of which there could be several

using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour
{
    public Texture2D heightmap;
    public GameObject chunkPrefab;
    public LocalNavMeshBuilder navMeshBuilder;

    //tweakables
    public int worldSize = 64;  //tiles per side of the world
    public float tileSize = 5f; //meters per side of each tiles
    public float noiseScale = .125f;
    public float heightScale = 1f;   //meters of elevation each new level gives us
    public float heightResolution = 1f;

    private Tile[,] map;    //set of all the tiles that make up the world
    private Chunk[] chunks; //set of all the chunks we're going to use to draw the world
                           // Use this for initialization

    void Start()
    {
        //initiate things
        map = new Tile[worldSize, worldSize];
        chunks = new Chunk[1];
        chunks[0] = GenerateChunk();

        for (int x = 0; x < worldSize; x++)
        {
            for (int z = 0; z < worldSize; z++)
            {
                map[x, z] = new Tile(x, z, 0f, tileSize);
            }
        }

        Debug.Log("Building Terrain");
        //generate the terrain!
        GenerateTerrain();

        //tell all the chunks to draw their share of the mesh
        for (int i = 0; i < chunks.GetLength(0); i++)
        {
            chunks[i].tileSize = tileSize;
            chunks[i].DrawTiles(map);
        }

        navMeshBuilder.UpdateNavMesh(false);
    }

    // Generate a chunk, fill it with necessary data and return the Chunk object
    Chunk GenerateChunk()
    {
        GameObject chunkGO = Instantiate(chunkPrefab, transform);
        Chunk chunk = chunkGO.GetComponent<Chunk>();
        chunk.tileSize = tileSize;
        chunk.chunkSize = worldSize;
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
                active.upperLeft = adjustVector(active.upperLeft);
                active.upperRight = adjustVector(active.upperRight);
                active.lowerRight = adjustVector(active.lowerRight);
                active.lowerLeft = adjustVector(active.lowerLeft);
                active.ReSetStats();
            }
        }
    }

    Vector3 adjustVector(Vector3 input)
    {
        float newHeight = input.y;
        newHeight = (int) height(input.x, input.z) * heightScale / heightResolution;
        return new Vector3(input.x, newHeight, input.z);
    }

    float height(float x, float z)
    {
        int texX = (int) (x / worldSize * heightmap.width);
        int texY = (int) (z / worldSize * heightmap.height);

        return heightmap.GetPixel(texX, texY).grayscale * heightResolution;
    }

    float perlin(float x, float z)
    {
        float perlinX = x * noiseScale;
        float perlinZ = z * noiseScale;

        Debug.Log("Perlin " + x + "," + z + " " + (Mathf.PerlinNoise(perlinX, perlinZ) * heightScale));
        return (Mathf.PerlinNoise(perlinX, perlinZ)) * heightScale;
    }
}