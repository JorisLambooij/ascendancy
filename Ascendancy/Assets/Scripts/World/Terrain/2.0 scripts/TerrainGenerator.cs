using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    // This first list contains every vertex of the mesh that we are going to render
    public List<Vector3> newVertices = new List<Vector3>();

    // The triangles tell Unity how to build each section of the mesh joining
    // the vertices
    public List<int> newTriangles = new List<int>();

    // The UV list is unimportant right now but it tells Unity how the texture is
    // aligned on each polygon
    public List<Vector2> newUV = new List<Vector2>();


    // A mesh is made up of the vertices, triangles and UVs we are going to define,
    // after we make them up we'll save them as this mesh
    private Mesh mesh;
    // Start is called before the first frame update

    private float tUnit = 0.25f;
    private Vector2 tStone = new Vector2(0, 0);
    private Vector2 tGrass = new Vector2(0, 1);

    private int squareCount;

    // Used to store the block type
    public byte[,] blocktype;

    // Use this for initialization
    void Start()
    {

        mesh = GetComponent<MeshFilter>().mesh;

        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;        

        GenTerrain();

        BuildMesh();

        UpdateMesh();
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        squareCount = 0;
        newVertices.Clear();
        newTriangles.Clear();
        newUV.Clear();
    }

    void GenSquare(int x, int z, Vector2 texture)
    {

        newVertices.Add(new Vector3(x, 0, z));
        newVertices.Add(new Vector3(x + 1, 0, z));
        newVertices.Add(new Vector3(x + 1, 0 - 1, z));
        newVertices.Add(new Vector3(x, 0 - 0, z));

        newTriangles.Add(squareCount * 4);
        newTriangles.Add((squareCount * 4) + 1);
        newTriangles.Add((squareCount * 4) + 3);
        newTriangles.Add((squareCount * 4) + 1);
        newTriangles.Add((squareCount * 4) + 2);
        newTriangles.Add((squareCount * 4) + 3);

        newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y + tUnit));
        newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y + tUnit));
        newUV.Add(new Vector2(tUnit * texture.x + tUnit, tUnit * texture.y));
        newUV.Add(new Vector2(tUnit * texture.x, tUnit * texture.y));

        squareCount++;

    }

    void GenTerrain()
    {
        blocktype = new byte[10, 10];

        for (int px = 0; px < blocktype.GetLength(0); px++)
        {
            for (int pz = 0; pz < blocktype.GetLength(1); pz++)
            {
                if (pz == 5)
                {
                    blocktype[px, pz] = 2;
                }
                else if (pz < 5)
                {
                    blocktype[px, pz] = 1;
                }
            }
        }
    }

    void BuildMesh()
    {
        for (int px = 0; px < blocktype.GetLength(0); px++)
        {
            for (int pz = 0; pz < blocktype.GetLength(1); pz++)
            {

                if (blocktype[px, pz] == 1)
                {
                    GenSquare(px, pz, tStone);
                }
                else if (blocktype[px, pz] == 2)
                {
                    GenSquare(px, pz, tGrass);
                }

            }
        }
    }
}
