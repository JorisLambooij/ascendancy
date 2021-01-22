//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class Chunk_old : MonoBehaviour
//{
//    #region mesh creation variables
//    //mesh creation variables
//    private List<Vector3> verts = new List<Vector3>();
//    private List<int> tris = new List<int>();
//    private List<Vector2> uvs = new List<Vector2>();
//    private int vertCount = 0;
//    //cached objects
//    private Mesh mesh;
//    private MeshCollider col;
//    #endregion

//    //tweakables
//    public int chunkSize = 64;  //max tiles per side this chunk can handle
//    public float tileSize = 1.5f;
//    public Vector2Int chunkIndex;

//    // Use this for initialization
//    public void Initialize()
//    {
//        //fetch objects
//        //TODO: fix this for instantiation

//        mesh = GetComponent<MeshFilter>().mesh;
//        col = GetComponent<MeshCollider>();
//    }

//    #region mesh-altering methods
//    void DrawQuad(Vector3 UpperLeft, Vector3 UpperRight, Vector3 LowerRight, Vector3 LowerLeft)
//    {
//        //add verticies
//        verts.Add(UpperLeft);
//        verts.Add(UpperRight);
//        verts.Add(LowerRight);
//        verts.Add(LowerLeft);

//        //do triangles
//        tris.Add(vertCount);        //1
//        tris.Add(vertCount + 1);    //2
//        tris.Add(vertCount + 2);    //3

//        tris.Add(vertCount);        //1
//        tris.Add(vertCount + 2);    //3
//        tris.Add(vertCount + 3);    //4

//        //TODO: tweak this
//        uvs.Add(UVProjection(UpperLeft));
//        uvs.Add(UVProjection(UpperRight));
//        uvs.Add(UVProjection(LowerRight));
//        uvs.Add(UVProjection(LowerLeft));

//        vertCount += 4;
//    }

//    Vector2 UVProjection(Vector3 point)
//    {
//        int numberOfChunks = ((World)World.Instance).numberOfChunks;
//        Vector3 UV_projectionInChunk = Vector3.ProjectOnPlane(point, Vector3.up) / (chunkSize * tileSize * numberOfChunks);

//        float x = UV_projectionInChunk.x + chunkIndex.x * (1f / numberOfChunks);
//        float y = UV_projectionInChunk.z + chunkIndex.y * (1f / numberOfChunks);

//        return new Vector2(x, y);
//    }

//    void DrawTile(Tile t)
//    {

//        //fetch the corner points, transforming them to world-space in the process
//        Vector3 p1 = transform.InverseTransformPoint(t.topLeft);  // p1 -> 0, 0
//        Vector3 p2 = transform.InverseTransformPoint(t.topRight); // p2 -> 1, 0
//        Vector3 p3 = transform.InverseTransformPoint(t.botRight); // p3 -> 1, 1
//        Vector3 p4 = transform.InverseTransformPoint(t.botLeft);  // p4 -> 0, 1
//        //if we're just drawing on the flat
//        if (!t.isSlope)
//        {
//            verts.Add(p1);
//            verts.Add(p2);
//            verts.Add(p3);
//            verts.Add(p4);

//            //do triangles
//            tris.Add(vertCount);        //1
//            tris.Add(vertCount + 1);    //2
//            tris.Add(vertCount + 2);    //3

//            tris.Add(vertCount);        //1
//            tris.Add(vertCount + 2);    //3
//            tris.Add(vertCount + 3);    //4

//            vertCount += 4;

//            //TODO: tweak this
//            uvs.Add(UVProjection(p1));
//            uvs.Add(UVProjection(p2));
//            uvs.Add(UVProjection(p3));
//            uvs.Add(UVProjection(p4));

//            return;
//        }

//        //calculate how many points are at the reference height;
//        int pointsAtReference = 0;
//        if (t.topRight.y.Equals(t.height)) { pointsAtReference++; }
//        if (t.topLeft.y.Equals(t.height)) { pointsAtReference++; }
//        if (t.botRight.y.Equals(t.height)) { pointsAtReference++; }
//        if (t.botLeft.y.Equals(t.height)) { pointsAtReference++; }

//        //only one point of off-spec
//        if (pointsAtReference == 3)
//        {
//            #region is the NW or SE point below ref?
//            if (!t.upperLeft.y.Equals(t.height) || !t.lowerRight.y.Equals(t.height))
//            {
//                //Draw tile where the upper-left corner is dropped
//                verts.Add(p1);
//                verts.Add(p2);
//                verts.Add(p4);

//                tris.Add(vertCount);
//                tris.Add(vertCount + 1);
//                tris.Add(vertCount + 2);

//                //431
//                uvs.Add(UVProjection(p1));
//                uvs.Add(UVProjection(p2));
//                uvs.Add(UVProjection(p4));

//                vertCount += 3;

//                //test-draw a triangle
//                verts.Add(p2);
//                verts.Add(p3);
//                verts.Add(p4);

//                tris.Add(vertCount);
//                tris.Add(vertCount + 1);
//                tris.Add(vertCount + 2);

//                //421
//                uvs.Add(UVProjection(p2));
//                uvs.Add(UVProjection(p3));
//                uvs.Add(UVProjection(p4));

//                vertCount += 3;

//                return;
//            }
//            #endregion
//            #region is the NE or SW point below ref?
//            if (!t.topRight.y.Equals(t.height) || !t.botLeft.y.Equals(t.height))
//            {
//                verts.Add(p1);
//                verts.Add(p2);
//                verts.Add(p3);

//                tris.Add(vertCount);
//                tris.Add(vertCount + 1);
//                tris.Add(vertCount + 2);

//                //432
//                uvs.Add(UVProjection(p1));
//                uvs.Add(UVProjection(p2));
//                uvs.Add(UVProjection(p3));

//                vertCount += 3;

//                //test-draw a triangle
//                verts.Add(p1);
//                verts.Add(p3);
//                verts.Add(p4);

//                tris.Add(vertCount);
//                tris.Add(vertCount + 1);
//                tris.Add(vertCount + 2);

//                //421
//                uvs.Add(UVProjection(p1));
//                uvs.Add(UVProjection(p3));
//                uvs.Add(UVProjection(p4));

//                vertCount += 3;

//                return;
//            }
//            #endregion
//        }
//        //two points off-spec
//        if (pointsAtReference == 2)
//        {
//            #region draw a basic tile
//            verts.Add(p1);
//            verts.Add(p2);
//            verts.Add(p3);
//            verts.Add(p4);

//            //do triangles
//            tris.Add(vertCount);        //1
//            tris.Add(vertCount + 1);    //2
//            tris.Add(vertCount + 2);    //3

//            tris.Add(vertCount);        //1
//            tris.Add(vertCount + 2);    //3
//            tris.Add(vertCount + 3);    //4

//            vertCount += 4;

//            //TODO: tweak this
//            uvs.Add(UVProjection(p1));
//            uvs.Add(UVProjection(p2));
//            uvs.Add(UVProjection(p3));
//            uvs.Add(UVProjection(p4));

//            return;
//            #endregion
//        }

//        //only one point is ON-spec
//        if (pointsAtReference == 1)
//        {
//            #region is the NW or SE point below ref?
//            if (t.upperLeft.y.Equals(t.height) || t.lowerRight.y.Equals(t.height))
//            {
//                //Draw tile where the upper-left corner is dropped
//                verts.Add(p1);
//                verts.Add(p2);
//                verts.Add(p4);

//                tris.Add(vertCount);
//                tris.Add(vertCount + 1);
//                tris.Add(vertCount + 2);

//                // 431
//                uvs.Add(UVProjection(p1));
//                uvs.Add(UVProjection(p2));
//                uvs.Add(UVProjection(p4));

//                vertCount += 3;

//                //test-draw a triangle
//                verts.Add(p2);
//                verts.Add(p3);
//                verts.Add(p4);

//                tris.Add(vertCount);
//                tris.Add(vertCount + 1);
//                tris.Add(vertCount + 2);

//                //321
//                uvs.Add(UVProjection(p2));
//                uvs.Add(UVProjection(p3));
//                uvs.Add(UVProjection(p4));

//                vertCount += 3;

//                return;
//            }
//            #endregion
//            #region is the NE or SW point below ref?
//            if (t.upperRight.y.Equals(t.height) || t.lowerLeft.y.Equals(t.height))
//            {
//                verts.Add(p1);
//                verts.Add(p2);
//                verts.Add(p3);

//                tris.Add(vertCount);
//                tris.Add(vertCount + 1);
//                tris.Add(vertCount + 2);

//                //432
//                uvs.Add(UVProjection(p1));
//                uvs.Add(UVProjection(p2));
//                uvs.Add(UVProjection(p3));

//                vertCount += 3;

//                //test-draw a triangle
//                verts.Add(p1);
//                verts.Add(p3);
//                verts.Add(p4);

//                tris.Add(vertCount);
//                tris.Add(vertCount + 1);
//                tris.Add(vertCount + 2);

//                //421
//                uvs.Add(UVProjection(p1));
//                uvs.Add(UVProjection(p3));
//                uvs.Add(UVProjection(p4));

//                vertCount += 3;

//                return;
//            }
//            #endregion
//        }
//    }
//    public void DrawTiles(Tile[,] map)
//    {
//        //figure out where we should start!
//        int startX = (int)Mathf.Max(transform.position.x / tileSize, 0);
//        int startZ = (int)Mathf.Max(transform.position.z / tileSize, 0);

//        //find out where we should END
//        int endX = Mathf.Min(startX + chunkSize, map.GetLength(0));
//        int endZ = Mathf.Min(startZ + chunkSize, map.GetLength(1));

//        //iterate though the list
//        for (int x = startX; x < endX; x++)
//            for (int z = startZ; z < endZ; z++)
//            {
//                Tile t = map[x, z];
//                DrawTile(t);
//            }

//        //then comit to the changes
//        ComitMeshChanges();

//    }
//    #endregion
    
//    //method that comits all our changes to the mesh proper
//    void ComitMeshChanges()
//    {
//        //load data into the mesh
//        mesh.Clear();
//        mesh.vertices = verts.ToArray();
//        mesh.triangles = tris.ToArray();
//        mesh.uv = uvs.ToArray();

//        //clean up the data
//        mesh.Optimize();
//        mesh.RecalculateNormals();

//        //assign the data to the messh colider
//        col.sharedMesh = null;
//        col.sharedMesh = mesh;

//        //clear everything
//        verts.Clear();
//        tris.Clear();
//        uvs.Clear();

//        vertCount = 0;
//    }
//}