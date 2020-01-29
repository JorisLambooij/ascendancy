using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public bool valid;

    public Color validColor;
    public Color invalidColor;

    Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponentInChildren<Renderer>();
        renderer.material.SetColor("_BaseColor", invalidColor);
    }

    // Update is called once per frame
    void Update()
    {
        if (valid)
            renderer.material.SetColor("_BaseColor", validColor);
        else
            renderer.material.SetColor("_BaseColor", invalidColor);
    }
}
