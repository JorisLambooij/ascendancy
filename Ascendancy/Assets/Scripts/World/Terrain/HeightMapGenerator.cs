using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class HeightMapGenerator : MonoBehaviour
{
    public float[,] noise;

    public Vector2 perlinOffset;
    
    public int octaves = 3;
    public float lucanarity = 2;
    public float persistance = 0.5f;
    public int mapWidth, mapHeight;
    
    [Range(0, 1)]
    public float cliffIntensity = .1f;
    
    public Gradient terrainHeightGradient;

    public float[,] GenerateHeightMap()
    {
        return GenerateHeightMap(mapWidth, mapHeight);
    }
    public float[,] GenerateHeightMap(int width, int height)
    {
        mapWidth = width;
        mapHeight = height;

        noise = new float[width, height];

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                noise[i, j] = HeightAt((float)i / width, (float)j / height);

        //Parallel.ForEach<Color>(noise.GetPixels, )
        return noise;
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

                if (displayMode == World.DisplayMode.Color)
                    color = terrainHeightGradient.Evaluate(intensity);
                else
                    color = new Color(intensity, intensity, intensity);

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
    

    private float HeightAt(float x, float y)
    {
        float perlin = 0;
        for (int i = 0; i < octaves; i++)
        {
            float u = x * Mathf.Pow(lucanarity, i) + perlinOffset.x;
            float v = y * Mathf.Pow(lucanarity, i) + perlinOffset.y;

            float noise = Mathf.PerlinNoise(u, v) * 2 - 1;
            perlin += noise * Mathf.Pow(persistance, i);
            //Debug.Log(u + " " + v + ": " + perlin);
        }

        //perlin = Mathf.Clamp01(perlin);

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
        float[,] deriv2 = Derivative2();

        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
            {
                map[i, j] -= deriv2[i, j] * cliffIntensity;
            }

        return map;
    }

    public float[,] Derivative1()
    {
        float[,] map = new float[mapWidth, mapHeight];

        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
                map[i, j] = FirstDerivative((float)i / mapWidth, (float)j / mapHeight);
        
        return map;
    }

    public float[,] Derivative2()
    {
        float[,] map = new float[mapWidth, mapHeight];

        for (int i = 0; i < mapWidth; i++)
            for (int j = 0; j < mapHeight; j++)
                map[i, j] = SecondDerivative((float)i / mapWidth, (float)j / mapHeight);
        
        return map;
    }

    private float FirstDerivative(float x, float y)
    {
        float dx = (HeightAt(x - 1, y) - HeightAt(x + 1, y));
        float dy = (HeightAt(x, y - 1) - HeightAt(x, y + 1));

        return dx + dy;
        //return Mathf.Sqrt(dx * dx + dy * dy);
    }

    private float SecondDerivative(float x, float y)
    {
        float ddx = (HeightAt(x + 1, y) + HeightAt(x - 1, y) - 2 * HeightAt(x, y));
        float ddy = (HeightAt(x, y + 1) + HeightAt(x, y - 1) - 2 * HeightAt(x, y));

        return ddx + ddy;
        //return Mathf.Sqrt(ddx * ddx + ddy * ddy);
    }
    #endregion

}
