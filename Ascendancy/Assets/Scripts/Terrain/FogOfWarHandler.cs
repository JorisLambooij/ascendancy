using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarHandler : MonoBehaviour
{
    public MeshFilter terrainMeshFilter;
    public MeshFilter fogMeshFilter;

    // Start is called before the first frame update
    void Start()
    {
        if (terrainMeshFilter == null)
            Debug.LogError("No terrain Mesh found. Fog of War will be disabled.");

        if (terrainMeshFilter == null)
            Debug.LogError("Fog of War MeshFilter not linked. Fog of War will be disabled.");

        fogMeshFilter.mesh = terrainMeshFilter.mesh;
        fogMeshFilter.transform.position += new Vector3(0, 5, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
