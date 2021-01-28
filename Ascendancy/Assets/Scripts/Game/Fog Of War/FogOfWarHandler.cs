using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FogOfWarHandler
{
    private Texture2D unseenTexture;
    private Material terrainMaterial;
    private Material waterMaterial;

    public FogOfWarHandler(int xSize, int ySize, Material terrainMaterial, Material waterMaterial)
    {
        #region initialize unseen texture
        unseenTexture = new Texture2D(xSize, ySize);
        Color[] blackPixels = Enumerable.Repeat(Color.black, xSize * ySize).ToArray();

        //// the following can be used to test the texture mask
        //for (int ix = 0; ix < 32; ix++)
        //{
        //    for (int iy = 0; iy < 32; iy++)
        //    {
        //        blackPixels[ix * iy] = Color.white;
        //    }
        //}

        unseenTexture.SetPixels(blackPixels);
        unseenTexture.Apply();
        unseenTexture.wrapMode = TextureWrapMode.Clamp;
        unseenTexture.filterMode = FilterMode.Point;
        #endregion

        this.terrainMaterial = terrainMaterial;
        this.waterMaterial = waterMaterial;


    }

    public Texture2D GetUnseenTexture()
    {
        return unseenTexture;
    }

    public void UpdateMaterial()
    {
        unseenTexture.Apply();
        terrainMaterial.SetTexture("_unseenMask", unseenTexture);
        waterMaterial.SetTexture("_fowMask", unseenTexture);
    }

    public void DiscoverTerrain(int x, int y, int rad)
    {
        float rSquared = rad * rad;

        for (int u = x - rad; u < x + rad + 1; u++)
            for (int v = y - rad; v < y + rad + 1; v++)
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                    unseenTexture.SetPixel(u, v, Color.white);
    }
}