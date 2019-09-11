using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Chunk : MonoBehaviour
{
    #region mesh creation variables
    //mesh creation variables
    private List<Vector3> verts = new List<Vector3>();
    private List<int> tris = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private int vertCount = 0;
    //cached objects
    private Mesh mesh;
    private MeshCollider col;
    #endregion
    #region textures
    private float tunit = .25f;
    private Vector2 tGrass = new Vector2(0, 3);
    private Vector2 tSand = new Vector2(3, 2);
    private Vector2 tWildCard = new Vector2(3, 0);
    #endregion

    //tweakables
    public int chunkSize = 64;  //max tiles per side this chunk can handle
    public float tileSize = 1.5f;
    // Use this for initialization
    void Awake()
    {
        //fetch objects
        //TODO: fix this for instantiation
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider>();
    }

    #region mesh-altering methods
    void DrawQuad(Vector3 UpperLeft, Vector3 UpperRight, Vector3 LowerRight, Vector3 LowerLeft)
    {
        //add verticies
        verts.Add(UpperLeft);
        verts.Add(UpperRight);
        verts.Add(LowerRight);
        verts.Add(LowerLeft);

        //do triangles
        tris.Add(vertCount);        //1
        tris.Add(vertCount + 1);    //2
        tris.Add(vertCount + 2);    //3

        tris.Add(vertCount);        //1
        tris.Add(vertCount + 2);    //3
        tris.Add(vertCount + 3);    //4

        //TODO: tweak this
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 0));

        vertCount += 4;
    }
    void DrawTile(Tile t, Vector2 texture)
    {

        //fetch the corner points, transforming them to world-space in the process
        Vector3 p1 = transform.InverseTransformPoint(t.upperLeft);
        Vector3 p2 = transform.InverseTransformPoint(t.upperRight);
        Vector3 p3 = transform.InverseTransformPoint(t.lowerRight);
        Vector3 p4 = transform.InverseTransformPoint(t.lowerLeft);
        //if we're just drawing on the flat
        if (!t.isSlope)
        {
            verts.Add(p1);
            verts.Add(p2);
            verts.Add(p3);
            verts.Add(p4);

            //do triangles
            tris.Add(vertCount);        //1
            tris.Add(vertCount + 1);    //2
            tris.Add(vertCount + 2);    //3

            tris.Add(vertCount);        //1
            tris.Add(vertCount + 2);    //3
            tris.Add(vertCount + 3);    //4

            vertCount += 4;

            //TODO: tweak this
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 0));

            return;
        }

        //calculate how many points are at the reference height;
        int pointsAtReference = 0;
        if (t.upperRight.y.Equals(t.height)) { pointsAtReference++; }
        if (t.upperLeft.y.Equals(t.height)) { pointsAtReference++; }
        if (t.lowerRight.y.Equals(t.height)) { pointsAtReference++; }
        if (t.lowerLeft.y.Equals(t.height)) { pointsAtReference++; }

        //only one point of off-spec
        if (pointsAtReference == 3)
        {
            #region is the NW or SE point below ref?
            if (!t.upperLeft.y.Equals(t.height) || !t.lowerRight.y.Equals(t.height))
            {
                //Draw tile where the upper-left corner is dropped
                verts.Add(p1);
                verts.Add(p2);
                verts.Add(p4);

                tris.Add(vertCount);
                tris.Add(vertCount + 1);
                tris.Add(vertCount + 2);

                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(0, 0));

                vertCount += 3;

                //test-draw a triangle
                verts.Add(p2);
                verts.Add(p3);
                verts.Add(p4);

                tris.Add(vertCount);
                tris.Add(vertCount + 1);
                tris.Add(vertCount + 2);

                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 0));

                vertCount += 3;

                return;
            }
            #endregion
            #region is the NE or SW point below ref?
            if (!t.upperRight.y.Equals(t.height) || !t.lowerLeft.y.Equals(t.height))
            {
                verts.Add(p1);
                verts.Add(p2);
                verts.Add(p3);

                tris.Add(vertCount);
                tris.Add(vertCount + 1);
                tris.Add(vertCount + 2);

                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(1, 0));

                vertCount += 3;

                //test-draw a triangle
                verts.Add(p1);
                verts.Add(p3);
                verts.Add(p4);

                tris.Add(vertCount);
                tris.Add(vertCount + 1);
                tris.Add(vertCount + 2);

                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 0));

                vertCount += 3;

                return;
            }
            #endregion
        }
        //two points off-spec
        if (pointsAtReference == 2)
        {
            #region draw a basic tile
            verts.Add(p1);
            verts.Add(p2);
            verts.Add(p3);
            verts.Add(p4);

            //do triangles
            tris.Add(vertCount);        //1
            tris.Add(vertCount + 1);    //2
            tris.Add(vertCount + 2);    //3

            tris.Add(vertCount);        //1
            tris.Add(vertCount + 2);    //3
            tris.Add(vertCount + 3);    //4

            vertCount += 4;

            //TODO: tweak this
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(0, 0));

            return;
            #endregion
        }

        //only one point is ON-spec
        if (pointsAtReference == 1)
        {
            #region is the NW or SE point below ref?
            if (t.upperLeft.y.Equals(t.height) || t.lowerRight.y.Equals(t.height))
            {
                //Draw tile where the upper-left corner is dropped
                verts.Add(p1);
                verts.Add(p2);
                verts.Add(p4);

                tris.Add(vertCount);
                tris.Add(vertCount + 1);
                tris.Add(vertCount + 2);

                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(0, 0));

                vertCount += 3;

                //test-draw a triangle
                verts.Add(p2);
                verts.Add(p3);
                verts.Add(p4);

                tris.Add(vertCount);
                tris.Add(vertCount + 1);
                tris.Add(vertCount + 2);

                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 0));

                vertCount += 3;

                return;
            }
            #endregion
            #region is the NE or SW point below ref?
            if (t.upperRight.y.Equals(t.height) || t.lowerLeft.y.Equals(t.height))
            {
                verts.Add(p1);
                verts.Add(p2);
                verts.Add(p3);

                tris.Add(vertCount);
                tris.Add(vertCount + 1);
                tris.Add(vertCount + 2);

                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(1, 0));

                vertCount += 3;

                //test-draw a triangle
                verts.Add(p1);
                verts.Add(p3);
                verts.Add(p4);

                tris.Add(vertCount);
                tris.Add(vertCount + 1);
                tris.Add(vertCount + 2);

                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 0));

                vertCount += 3;

                return;
            }
            #endregion
        }
    }
    public void DrawTiles(Tile[,] map)
    {
        //figure out where we should start!
        int startX = (int)(transform.position.x / tileSize);
        int startZ = (int)(transform.position.z / tileSize);

        //find out where we should END
        int endX = Mathf.Min(startX + chunkSize, map.GetLength(0));
        int endZ = Mathf.Min(startZ + chunkSize, map.GetLength(1));

        //iterate though the list
        for (int x = startX; x < endX; x++)
        {
            for (int z = startZ; z < endZ; z++)
            {
                Tile t = map[x, z];
                DrawTile(t, tGrass);
            }
        }

        //then comit to the changes
        ComitMeshChanges();

    }
    #endregion

    //calculate the textures properlly
    Vector2 getUVCoordinates(Vector2 input, Vector2 texture)
    {
        Vector2 returnVect = new Vector2((input.x * tunit) + (texture.x * tunit), (input.y * tunit) + (texture.y * tunit));
        return returnVect;
    }

    //method that comits all our changes to the mesh proper
    void ComitMeshChanges()
    {
        //load data into the mesh
        mesh.Clear();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        //clean up the data
        mesh.Optimize();
        mesh.RecalculateNormals();

        //assign the data to the messh colider
        col.sharedMesh = null;
        col.sharedMesh = mesh;

        //clear everything
        verts.Clear();
        tris.Clear();
        uvs.Clear();

        vertCount = 0;
    }
}