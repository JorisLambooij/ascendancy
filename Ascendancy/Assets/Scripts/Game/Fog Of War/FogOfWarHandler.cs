using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogOfWarHandler
{
    public Texture2D heightmapTexture;

    public FogOfWarHandler(int xSize, int ySize, Material terrainMaterial, Material waterMaterial)
    {
        World worldScript = World.Instance;
        Color[] terrainHeightmap = new Color[worldScript.worldSize * worldScript.worldSize];

        for (int x = 0; x < worldScript.worldSize; x++)
            for (int y = 0; y < worldScript.worldSize; y++)
            {
                float h = worldScript.GetTile(new Vector3(x, 0, y)).Height / worldScript.heightScale;
                terrainHeightmap[x + worldScript.worldSize * y] = new Color(h, h, h);
            }

        heightmapTexture = new Texture2D(worldScript.worldSize, worldScript.worldSize);
        heightmapTexture.SetPixels(terrainHeightmap);
        heightmapTexture.Apply();

        //GetComponent<MeshRenderer>().material.SetTexture("Texture2D_height", heightmapTexture);
    }

}