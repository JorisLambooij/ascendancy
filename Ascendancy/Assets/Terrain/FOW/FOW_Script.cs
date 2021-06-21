using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FOW_Script : MonoBehaviour
{
    enum DownSampleMode { Off, Half, Quarter }

    [SerializeField]
    DownSampleMode _downSampleMode = DownSampleMode.Quarter;

    public RenderTexture visibilityRT;
    public RenderTexture discoveryMap;
    public RenderTexture discoveryMapBlurred;

    public Material discoveryBlendMat;

    [SerializeField, Range(0, 8)]
    int blurStrength;

    [SerializeField]
    Shader blurShader;
    Material _material;

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
        Graphics.Blit(visibilityRT, discoveryMap, discoveryBlendMat);

        Blur(discoveryMap, discoveryMapBlurred);
    }

    void Blur(RenderTexture source, RenderTexture destination)
    {
        if (_material == null)
        {
            _material = new Material(blurShader);
            _material.hideFlags = HideFlags.HideAndDontSave;
        }

        RenderTexture rt1, rt2;

        if (_downSampleMode == DownSampleMode.Half)
        {
            rt1 = RenderTexture.GetTemporary(source.width / 2, source.height / 2);
            rt2 = RenderTexture.GetTemporary(source.width / 2, source.height / 2);
            Graphics.Blit(source, rt1);
        }
        else if (_downSampleMode == DownSampleMode.Quarter)
        {
            rt1 = RenderTexture.GetTemporary(source.width / 4, source.height / 4);
            rt2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4);
            Graphics.Blit(source, rt1, _material, 0);
        }
        else
        {
            rt1 = RenderTexture.GetTemporary(source.width, source.height);
            rt2 = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(source, rt1);
        }

        for (var i = 0; i < blurStrength; i++)
        {
            Graphics.Blit(rt1, rt2, _material, 1);
            Graphics.Blit(rt2, rt1, _material, 2);
        }

        Graphics.Blit(rt1, destination);

        RenderTexture.ReleaseTemporary(rt1);
        RenderTexture.ReleaseTemporary(rt2);
    }
}
