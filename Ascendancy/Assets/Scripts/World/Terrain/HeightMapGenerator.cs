using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class HeightMapGenerator : MonoBehaviour
{
    public Texture2D sample;
    public Vector2 perlinOffset;

    public int numberOfPerlins;
    public float lucanarity = 2;
    public float persistance = 0.5f;

    public float[,] GenerateHeightMap(int width, int height)
    {
        float[,] noise = new float[width, height];

        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                noise[i, j] = HeightAt((float)i / width, (float)j / height);

        //Parallel.ForEach<Color>(noise.GetPixels, )
        return noise;
    }

    private void FillHeightmap()
    {

    }
    

    private float HeightAt(float x, float y)
    {
        float perlin = 0;
        for (int i = 0; i < numberOfPerlins; i++)
        {
            float u = x * Mathf.Pow(lucanarity, i) + perlinOffset.x;
            float v = y * Mathf.Pow(lucanarity, i) + perlinOffset.y;

            perlin += Mathf.PerlinNoise(u, v) * Mathf.Pow(persistance, i);
            //Debug.Log(u + " " + v + ": " + perlin);
        }

        if (perlin > 1 || perlin < 0)
            Debug.Log(x + " " + y + ": " + perlin);

        return perlin;
    }
}
