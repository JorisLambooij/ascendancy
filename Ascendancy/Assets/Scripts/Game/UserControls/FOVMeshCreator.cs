using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVMeshCreator : MonoBehaviour
{

    public Material mat;

    private Mesh mesh;
    private float segmentAngle;

    private Vector3[] verts;
    private Vector3[] normals;
    private int[] triangles;
    private Vector2[] uvs;

    private float actualAngle;


    private Camera cam;


    GameObject cameraMesh;

    void Start()
    {
        cam = Camera.main;
        
        cameraMesh = new GameObject("MainCameraBoundsMesh");

        cameraMesh.layer = 10;


        MeshFilter MeshF = cameraMesh.AddComponent<MeshFilter>();
        MeshRenderer MeshR = cameraMesh.AddComponent<MeshRenderer>();
        MeshCollider MeshMC = cameraMesh.AddComponent<MeshCollider>();

        cameraMesh.transform.parent = this.transform.parent;

        MeshR.material = mat;
        MeshR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        //MESH
        mesh = cameraMesh.GetComponent<MeshFilter>().mesh;

        //BUILD THE MESH
        BuildMesh();

        MeshMC.sharedMesh = mesh;


    }

    void BuildMesh()
    {

        Vector3[] vertices = {
            
            //front (viewport)
            cam.ViewportToWorldPoint(new Vector3(0,0,cam.nearClipPlane)),
            cam.ViewportToWorldPoint(new Vector3(1,0,cam.nearClipPlane)),
            cam.ViewportToWorldPoint(new Vector3(1,1,cam.nearClipPlane)),
            cam.ViewportToWorldPoint(new Vector3(0,1,cam.nearClipPlane)),

            cam.ViewportToWorldPoint(new Vector3(0,1,cam.farClipPlane)),
            cam.ViewportToWorldPoint(new Vector3(1,1,cam.farClipPlane)),
            cam.ViewportToWorldPoint(new Vector3(1,0,cam.farClipPlane)),
            cam.ViewportToWorldPoint(new Vector3(0,0,cam.farClipPlane)),

            //new Vector3 (0, 1, 1),
            //new Vector3 (1, 1, 1),
            //new Vector3 (1, 0, 1),
            //new Vector3 (0, 0, 1),
        };

        int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();

    }
}

