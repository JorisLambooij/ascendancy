using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class HeightMapGenerator : MonoBehaviour
{
    public float[,] noise;

    public bool useDebugHeightmap;
    public Texture2D debugHeightmap;

    public Vector2 perlinOffset;
    
    [Header("Hill Parameters")]
    [Tooltip("Number of Noise functions to sample.")]
    public int octaves = 3;
    [Tooltip("The relative scale of each subsequent function.")]
    public float lucanarity = 2;
    [Tooltip("The amount of influence each subsequent function has in the grand total.")]
    public float persistance = 0.5f;
    [Tooltip("The amount that the terrain is raised by.")]
    public float heightOffset = 1f;

    [Header("Misc")]
    
    [Range(0, 1)]
    public float cliffIntensity = .1f;
    
    public Gradient terrainHeightGradient;

    private int mapWidth, mapHeight;
    private float noiseScale;

    public float[,] GenerateHeightMap()
    {
        return GenerateHeightMap(mapWidth, mapHeight, noiseScale);
    }

    public float[,] GenerateHeightMap(int width, int height, float noiseScale)
    {
        mapWidth = width;
        mapHeight = height;
        this.noiseScale = noiseScale;

        if (useDebugHeightmap)
        {
            noise = new float[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    int u = (int)Mathf.Lerp(0, debugHeightmap.width, (float)x/width);
                    int v = (int)Mathf.Lerp(0, debugHeightmap.height, (float)y/height);
                    if (debugHeightmap.GetPixel(u, v) == null)
                        Debug.Log("UV: " + u + " " + v);
                    noise[x, y] = debugHeightmap.GetPixel(u, v).grayscale;
                }
            return noise;
        }

        noise = new float[width, height];

        // Hills
        noise = GenerateNoiseMap(width, height, perlinOffset, octaves, lucanarity, persistance, noiseScale);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                noise[x, y] = Mathf.Max(heightOffset, noise[x, y]);

        // Lakes
        float[,] lakemap = GenerateNoiseMap(width, height, perlinOffset, 2, 0.8f, 0.6f, noiseScale / 4);

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                lakemap[x, y] += 0.4f;
                if (noise[x, y] <= 0)
                    noise[x, y] -= noise[x, y] * lakemap[x, y];
            }

        // Mountains
        float[,] mountains = GenerateNoiseMap(width, height, perlinOffset, 3, 1, 1, noiseScale / 2);
        
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (mountains[x, y] > 0.1f && noise[x, y] > 0.2f)
                    noise[x, y] *= Mathf.Min(1.5f, 1 + mountains[x, y]);

        return noise;
    }

    public float[,] GenerateNoiseMap(int width, int height, Vector2 offset, int _octaves, float _lucanarity, float _persistance, float noiseScale)
    {
        float[,] noisemap = new float[width, height];

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                float x = (float)i * noiseScale / width + offset.x;
                float y = (float)j * noiseScale / height + offset.y;
                
                noisemap[i, j] = PerlinHeightAt(x, y, _octaves, _lucanarity, _persistance);
            }

        return noisemap;
    }

    public Texture2D WorldTexture(float[,] noiseMap, World.DisplayMode displayMode)
    {
        Texture2D texture = new Texture2D(mapWidth, mapHeight);

        for (int x = 0; x < mapWidth; x++)
            for (int y = 0; y < mapHeight; y++)
            {
                Color color;
                float intensity = noiseMap[x, y];

                intensity = (intensity + 1) / 2;


                switch (displayMode)
                {
                    case World.DisplayMode.Height:
                        color = new Color(intensity, intensity, intensity);
                        break;
                    case World.DisplayMode.Gradient:
                        float gradient = FirstDerivative(x, y, noiseMap);
                        gradient *= 8;
                        color = new Color(gradient, gradient, gradient);
                        break;
                    case World.DisplayMode.Color:
                        color = terrainHeightGradient.Evaluate(intensity);
                        break;
                    default:
                        color = new Color(.1f, .7f, .2f);
                        break;
                }

                texture.SetPixel(x, y, color);
            }

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        texture.Apply();
        return texture;
    }
    
    private void FillHeightmap()
    {
        
    }
    
    /// <summary>
    /// Samples a generic Perlin Noise map
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private float PerlinHeightAt(float x, float y)
    {
        return PerlinHeightAt(x, y, octaves, lucanarity, persistance);
    }
    private float PerlinHeightAt(float x, float y, int _octaves, float _lucanarity, float _persistance)
    {
        float perlin = 0;
        for (int i = 0; i < _octaves; i++)
        {
            float u = x * Mathf.Pow(_lucanarity, i) + perlinOffset.x;
            float v = y * Mathf.Pow(_lucanarity, i) + perlinOffset.y;

            float noise = Mathf.PerlinNoise(u, v) * 2 - 1;
            perlin += noise * Mathf.Pow(_persistance, i);
        }

        perlin = Mathf.Clamp(perlin, -1f, 1);
        if (perlin > 1 || perlin < -1)
            Debug.Log(x + " " + y + ": " + perlin);

        return perlin;
    }

    #region derivatives
    public float[,] AmplifyCliffs()
    {
        if (noise == null)
        {
            Debug.LogError("Generate the Heightmap first before amplifying cliffs.");
            return null;
        }

        float[,] map = noise;
        float[,] deriv2 = Derivative2(noise);
        float[,] derivWeights = GenerateNoiseMap(mapWidth, mapHeight, new Vector2(1.5f, 2.25f), 1, 1, 1, 5);

        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
            {
                //float cliffWeight = Mathf.Clamp01(cliffIntensity * (derivWeights[i, j] / 2 + 0.5f));
                float cliffWeight = 1;
                map[i, j] -= deriv2[i, j] * cliffWeight;
            }

        return map;
    }
    
    public float[,] Derivative1(float[,] heightmap)
    {
        float[,] map = new float[mapWidth, mapHeight];

        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
                map[i, j] = FirstDerivative(i, j, heightmap);
        
        return map;
    }

    public float[,] Derivative2(float[,] heightmap)
    {
        float[,] map = new float[mapWidth, mapHeight];

        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
                map[i, j] = SecondDerivative(i, j, heightmap);
        
        return map;
    }

    private float FirstDerivative(int x, int y, float[,] heightmap)
    {
        float dx = (HeightAt(x - 1, y, heightmap) - HeightAt(x + 1, y, heightmap));
        float dy = (HeightAt(x, y - 1, heightmap) - HeightAt(x, y + 1, heightmap));

        //return dy;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    private float HeightAt(int x, int y, float[,] heightmap)
    {
        if (x < 0 || x >= heightmap.GetLength(0))
            return 0;
        if (y < 0 || y >= heightmap.GetLength(1))
            return 0;

        return heightmap[x, y];
    }

    private float SecondDerivative(int x, int y, float[,] heightmap)
    {
        float ddx = (HeightAt(x + 1, y, heightmap) + HeightAt(x - 1, y, heightmap) - 2 * HeightAt(x, y, heightmap));
        float ddy = (HeightAt(x, y + 1, heightmap) + HeightAt(x, y - 1, heightmap) - 2 * HeightAt(x, y, heightmap));

        return ddx + ddy;
        //return Mathf.Sqrt(ddx * ddx + ddy * ddy);
    }
    #endregion

    public float[,] AddHeightmap(float[,] hm2)
    {
        Debug.Assert(noise.GetLength(0) == hm2.GetLength(0), "Heightmaps not of identical Width!");
        Debug.Assert(noise.GetLength(1) == hm2.GetLength(1), "Heightmaps not of identical Height!");

        for (int x = 0; x < noise.GetLength(0); x++)
            for (int y = 0; y < noise.GetLength(1); y++)
                noise[x, y] += hm2[x, y];
        return noise;
    }
}
