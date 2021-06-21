using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FOW_Script : MonoBehaviour
{
    public RenderTexture visibilityRT;
    public RenderTexture discoveryMap;

    public Material shaderMat;

    // Start is called before the first frame update
    void Start()
    {
        // Reset any previous textures, so we start with a black surface.
        discoveryMap.Release();
    }

    // Update is called once per frame
    void Update()
    {
        // Applies a Max-Blend Shader, i.e. the current visibility is added to the already discovered areas.
        Graphics.Blit(visibilityRT, discoveryMap, shaderMat);
    }
}
