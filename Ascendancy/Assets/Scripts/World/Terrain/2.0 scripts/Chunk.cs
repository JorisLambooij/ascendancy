using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chunk : MonoBehaviour
{
    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();

    private float tUnit = 0.25f;
    
    private Vector2 tStone = new Vector2(1, 3);
    private Vector2 tGrass = new Vector2(3, 3);

    private Vector2 tCliff = new Vector2(0, 0);

    private Mesh mesh;
    private MeshCollider col;

    private int faceCount;

    private FaceData[,] tilemap;
    private int[,] heightmap;
    
    public static int chunkSize = 64;
    public Vector2Int chunkIndex;

    // Start is called before the first frame update
    public void Initialize(float[,] globalHeightmap)
    {
        //mesh = GetComponent<MeshFilter>().mesh;
        mesh = new Mesh();
        col = GetComponent<MeshCollider>();
        
        GenerateTerrain(globalHeightmap);

        AdditiveSmoothing();

        GenerateMesh();
        FillCliffs();

        UpdateMesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        //col.sharedMesh=null;
        //col.sharedMesh=mesh;

        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();

        faceCount = 0;
    }

    public void GenerateTerrain(float[,] globalHeightmap)
    {

        tilemap = new FaceData[chunkSize, chunkSize];
        heightmap = new int[chunkSize, chunkSize];

        int startX = chunkIndex.x * chunkSize, startY = chunkIndex.y * chunkSize;

        for (int dx = 0; dx < chunkSize; dx++)
            for (int dy = 0; dy < chunkSize; dy++)
            {
                int u = Mathf.Min(globalHeightmap.GetLength(0) - 1, startX + dx);
                int v = Mathf.Min(globalHeightmap.GetLength(1) -1, startY + dy);
                //get y and insert to int heightmap for later
                int y = Mathf.RoundToInt(globalHeightmap[u, v] * 10);

                //Debug.Assert(wd < heightmap.GetLength(0) && hg < heightmap.GetLength(1), "Error. WD/HG: " + wd + " " + hg);
                heightmap[dx, dy] = y;


                tilemap[dx, dy] = new FaceData
                {
                    topLeft = new Vector3(dx - 0.5f, y, dy + 0.5f), //top left
                    topRight = new Vector3(dx + 0.5f, y, dy + 0.5f), //up right
                    botRight = new Vector3(dx + 0.5f, y, dy - 0.5f), //down right
                    botLeft = new Vector3(dx - 0.5f, y, dy - 0.5f) //down left
                };
            }
    }

    public void GenerateMesh()
    {
        for (int wd = 0; wd < tilemap.GetLength(0); wd++)
            for (int hg = 0; hg < tilemap.GetLength(1); hg++)
                GenerateFace(tilemap[wd, hg], tGrass);
    }

    public void AdditiveSmoothing()
    {
        FaceData Neighbor;
        bool tl = false;
        bool tr = false;
        bool bl = false;
        bool br = false;
        FaceData me;

        for (int wd = 0; wd < tilemap.GetLength(0); wd++)
        {
            for (int hg = 0; hg < tilemap.GetLength(1); hg++)
            {
                me = tilemap[wd, hg];
                tl = false;
                tr = false;
                bl = false;
                br = false;

                #region direct
                //check left
                if (wd > 0)
                {
                    Neighbor = tilemap[wd - 1, hg];

                    if (Neighbor.topRight.y > me.topLeft.y && Neighbor.botRight.y > me.botLeft.y)
                    {
                        tl = true;
                        bl = true;
                    }
                }

                //check above
                if (hg < tilemap.GetLength(1) - 1)
                {
                    Neighbor = tilemap[wd, hg + 1];

                    if (Neighbor.botLeft.y > me.topLeft.y && Neighbor.botRight.y > me.topRight.y)
                    {
                        tl = true;
                        tr = true;
                    }
                }

                //check right
                if (wd < tilemap.GetLength(0) - 1)
                {
                    Neighbor = tilemap[wd + 1, hg];

                    if (Neighbor.topLeft.y > me.topRight.y && Neighbor.botLeft.y > me.botRight.y)
                    {
                        tr = true;
                        br = true;
                    }
                }

                //check below
                if (hg > 0)
                {
                    Neighbor = tilemap[wd, hg - 1];

                    if (Neighbor.topLeft.y > me.botLeft.y && Neighbor.topRight.y > me.botRight.y)
                    {
                        bl = true;
                        br = true;
                    }
                }
                #endregion

                #region diagonal
                //check topLeft
                if (wd > 0&& hg < tilemap.GetLength(1) - 1)
                {
                    Neighbor = tilemap[wd - 1, hg +1];

                    if (Neighbor.botRight.y > me.topLeft.y)
                    {
                        tl = true;
                    }
                }

                //check topRight
                if (hg < tilemap.GetLength(1) - 1 && wd < tilemap.GetLength(0) - 1)
                {
                    Neighbor = tilemap[wd + 1, hg + 1];

                    if (Neighbor.botLeft.y > me.topRight.y)
                    {
                        tr = true;
                    }
                }

                //check botRight
                if (hg > 0 && wd < tilemap.GetLength(0) - 1)
                {
                    Neighbor = tilemap[wd + 1, hg - 1];

                    if (Neighbor.topLeft.y > me.botRight.y)
                    {
                        br = true;
                    }
                }

                //check botLeft
                if (hg > 0 && wd > 0)
                {
                    Neighbor = tilemap[wd - 1, hg - 1];

                    if (Neighbor.topRight.y > me.botLeft.y)
                    {
                        bl = true;
                    }
                }

                
                #endregion


                if (tl)
                {
                    tilemap[wd, hg].topLeft.y += 1;
                }
                if (tr)
                {
                    tilemap[wd, hg].topRight.y += 1;
                }
                if (br)
                {
                    tilemap[wd, hg].botRight.y += 1;
                }
                if (bl)
                {
                    tilemap[wd, hg].botLeft.y += 1;
                }

            }
        }
    }

    public void FillCliffs()
    {
        FaceData Neighbor;

        for (int wd = 0; wd < tilemap.GetLength(0); wd++)
        {
            for (int hg = 0; hg < tilemap.GetLength(1); hg++)
            {
                FaceData me = tilemap[wd, hg];

                //check left
                if (wd > 0)
                {
                    Neighbor = tilemap[wd - 1, hg];

                    if (Neighbor.topRight.y < me.topLeft.y || Neighbor.botRight.y < me.botLeft.y)
                    {
                        FaceData cliff = new FaceData
                        {
                            topLeft = me.topLeft, //top left
                            topRight = me.botLeft, //up right
                            botRight = Neighbor.botRight, //down right
                            botLeft = Neighbor.topRight //down left
                        };

                        GenerateFace(cliff, tCliff);
                    }
                }

                //check above
                if (hg < tilemap.GetLength(1) - 1)
                {
                    Neighbor = tilemap[wd, hg + 1];

                    if (Neighbor.botLeft.y < me.topLeft.y || Neighbor.botRight.y < me.topRight.y)
                    {
                        FaceData cliff = new FaceData
                        {
                            topLeft = me.topRight, //top left
                            topRight = me.topLeft, //up right
                            botRight = Neighbor.botLeft, //down right
                            botLeft = Neighbor.botRight //down left
                        };

                        GenerateFace(cliff, tCliff);
                    }
                }

                //check right
                if (wd < tilemap.GetLength(0) - 1)
                {
                    Neighbor = tilemap[wd + 1, hg];

                    if (Neighbor.topLeft.y < me.topRight.y || Neighbor.botLeft.y < me.botRight.y)
                    {
                        FaceData cliff = new FaceData
                        {
                            topLeft = me.botRight, //top left
                            topRight = me.topRight, //up right
                            botRight = Neighbor.topLeft, //down right
                            botLeft = Neighbor.botLeft //down left
                        };

                        GenerateFace(cliff, tCliff);
                    }
                }

                //check below
                if (hg > 0)
                {
                    Neighbor = tilemap[wd, hg - 1];

                    if (Neighbor.topRight.y < me.topLeft.y || Neighbor.botRight.y < me.botLeft.y)
                    {
                        FaceData cliff = new FaceData
                        {
                            topLeft = me.botLeft, //top left
                            topRight = me.botRight, //up right
                            botRight = Neighbor.topRight, //down right
                            botLeft = Neighbor.topLeft //down left
                        };

                        GenerateFace(cliff, tCliff);
                    }
                }

            }
        }
    }
    

    private void GenerateFace(FaceData face, Vector2 texture)
    {
        newVertices.Add(face.topLeft);
        newVertices.Add(face.topRight);
        newVertices.Add(face.botRight);
        newVertices.Add(face.botLeft);

        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 1); //2
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4 + 3); //4

        Vector2 texturePos;

        texturePos = texture;

        newUV.Add(UVProjection(face.topLeft));
        newUV.Add(UVProjection(face.topRight));
        newUV.Add(UVProjection(face.botRight));
        newUV.Add(UVProjection(face.botLeft));

        //newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
        //newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
        //newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
        //newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

        faceCount++;
    }

    private Vector2 UVProjection(Vector3 point)
    {
        int numberOfChunks = ((World)World.Instance).numberOfChunks;
        Vector3 UV_projectionInChunk = Vector3.ProjectOnPlane(point, Vector3.up) / (chunkSize * ((World)World.Instance).tileSize * numberOfChunks);

        float x = UV_projectionInChunk.x + chunkIndex.x * (1f / numberOfChunks);
        float y = UV_projectionInChunk.z + chunkIndex.y * (1f / numberOfChunks);

        return new Vector2(x, y);
    }
}
