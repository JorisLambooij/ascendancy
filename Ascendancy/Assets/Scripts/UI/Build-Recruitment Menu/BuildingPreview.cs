using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    private bool valid;

    public Material material;
    public Color validColor;
    public Color invalidColor;

    Renderer renderer;


    public bool Valid 
    { 
        get => valid; 
        set
        {
            valid = value;
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer renderer in meshRenderers)
            {
                foreach(Material mat in renderer.materials)
                    if (valid)
                        mat.SetColor("_BaseColor", validColor);
                    else
                        mat.SetColor("_BaseColor", invalidColor);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i] = material;
                renderer.materials[i].SetColor("_BaseColor", invalidColor);
            }
        }
    }

    /*
    // Update is called once per frame
    void Update()
    {
        if (Valid)
            renderer.material.SetColor("_BaseColor", validColor);
        else
            renderer.material.SetColor("_BaseColor", invalidColor);
    }
    */
}
