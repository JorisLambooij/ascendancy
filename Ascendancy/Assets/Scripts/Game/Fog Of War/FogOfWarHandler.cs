using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarHandler : MonoBehaviour
{
    public Texture2D heightmapTexture;

    // Start is called before the first frame update
    void Start()
    {
        World worldScript = World.Instance;
        Color[] terrainHeightmap = new Color[worldScript.worldSize * worldScript.worldSize];

        for (int x = 0; x < worldScript.worldSize; x++)
            for (int y = 0; y < worldScript.worldSize; y++)
            {
                float h = worldScript.GetTile(new Vector3(x, 0, y)).height / worldScript.heightScale;
                terrainHeightmap[x + worldScript.worldSize * y] = new Color(h, h, h);
            }

        heightmapTexture = new Texture2D(worldScript.worldSize, worldScript.worldSize);
        heightmapTexture.SetPixels(terrainHeightmap);
        heightmapTexture.Apply();

        GetComponent<MeshRenderer>().material.SetTexture("Texture2D_height", heightmapTexture);
    }

}
