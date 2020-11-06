﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chunk : MonoBehaviour
{

    private Mesh mesh;
    private MeshCollider col;

    private int faceCount;

    //private Tile[,] chunkTilemap;
    private int[,] heightmap;

    public static int chunkSize = 64;
    public Vector2Int chunkIndex;

    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();
    private List<Color32> newColors = new List<Color32>();

    // Start is called before the first frame update
    public void Initialize(Tile[,] chunkTilemap, Color32[,] colormap, bool tintFlippedTiles)
    {
        newVertices = new List<Vector3>();
        newUV = new List<Vector2>();
        newTriangles = new List<int>();

        //mesh = GetComponent<MeshFilter>().mesh;
        mesh = new Mesh();
        col = GetComponent<MeshCollider>();

        //GenerateTerrain(chunkTilemap);

        //AdditiveSmoothing();

        GenerateMesh(chunkTilemap, colormap, tintFlippedTiles);
        //FillCliffs(chunkTilemap);

        UpdateMesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.colors32 = newColors.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        //col.sharedMesh=null;
        //col.sharedMesh=mesh;

        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();
        newColors.Clear();

        faceCount = 0;
    }

    private void GenerateFace(Face face, Color c, bool flipTriangles)
    {

        newVertices.Add(face.topLeft);
        newVertices.Add(face.topRight);
        newVertices.Add(face.botRight);
        newVertices.Add(face.botLeft);


        newColors.Add(c);
        newColors.Add(c);
        newColors.Add(c);
        newColors.Add(c);

        if (!flipTriangles)
        {
            newTriangles.Add(faceCount * 4);        //1
            newTriangles.Add(faceCount * 4 + 1);    //2
            newTriangles.Add(faceCount * 4 + 2);    //3
            newTriangles.Add(faceCount * 4);        //1
            newTriangles.Add(faceCount * 4 + 2);    //3
            newTriangles.Add(faceCount * 4 + 3);    //4
        }
        else
        {
            newTriangles.Add(faceCount * 4);        //1
            newTriangles.Add(faceCount * 4 + 1);    //2
            newTriangles.Add(faceCount * 4 + 3);    //4
            newTriangles.Add(faceCount * 4 + 1);    //2
            newTriangles.Add(faceCount * 4 + 2);    //3
            newTriangles.Add(faceCount * 4 + 3);    //4

        }

        newUV.Add(UVProjection(face.topLeft));
        newUV.Add(UVProjection(face.topRight));
        newUV.Add(UVProjection(face.botRight));
        newUV.Add(UVProjection(face.botLeft));

        faceCount++;
    }

    //public void FillCliffs(Tile[,] chunkTilemap)
    //{
    //    Vector2 tCliff = new Vector2(0, 0);
    //    Tile Neighbor;
    //    Tile me;
    //    Tile cliff;

    //    for (int wd = 0; wd < chunkTilemap.GetLength(0); wd++)
    //    {
    //        for (int hg = 0; hg < chunkTilemap.GetLength(1); hg++)
    //        {
    //            me = chunkTilemap[wd, hg];

    //            //check left
    //            if (wd > 0)
    //            {
    //                Neighbor = chunkTilemap[wd - 1, hg];

    //                if (Neighbor.topRight.y < me.topLeft.y || Neighbor.botRight.y < me.botLeft.y)
    //                {
    //                    cliff = new Tile
    //                    {
    //                        topLeft = me.topLeft, //top left
    //                        topRight = me.botLeft, //up right
    //                        botRight = Neighbor.botRight, //down right
    //                        botLeft = Neighbor.topRight //down left
    //                    };

    //                    GenerateFace(cliff, tCliff);
    //                }
    //            }

    //            //check above
    //            if (hg < chunkTilemap.GetLength(1) - 1)
    //            {
    //                Neighbor = chunkTilemap[wd, hg + 1];

    //                if (Neighbor.botLeft.y < me.topLeft.y || Neighbor.botRight.y < me.topRight.y)
    //                {
    //                    cliff = new Tile
    //                    {
    //                        topLeft = me.topRight, //top left
    //                        topRight = me.topLeft, //up right
    //                        botRight = Neighbor.botLeft, //down right
    //                        botLeft = Neighbor.botRight //down left
    //                    };

    //                    GenerateFace(cliff, tCliff);
    //                }
    //            }

    //            //check right
    //            if (wd < chunkTilemap.GetLength(0) - 1)
    //            {
    //                Neighbor = chunkTilemap[wd + 1, hg];

    //                if (Neighbor.topLeft.y < me.topRight.y || Neighbor.botLeft.y < me.botRight.y)
    //                {
    //                    cliff = new Tile
    //                    {
    //                        topLeft = me.botRight, //top left
    //                        topRight = me.topRight, //up right
    //                        botRight = Neighbor.topLeft, //down right
    //                        botLeft = Neighbor.botLeft //down left
    //                    };

    //                    GenerateFace(cliff, tCliff);
    //                }
    //            }

    //            //check below
    //            if (hg > 0)
    //            {
    //                Neighbor = chunkTilemap[wd, hg - 1];

    //                if (Neighbor.topLeft.y < me.botLeft.y || Neighbor.topRight.y < me.botRight.y)
    //                {
    //                    cliff = new Tile
    //                    {
    //                        topLeft = me.botLeft, //top left
    //                        topRight = me.botRight, //up right
    //                        botRight = Neighbor.topRight, //down right
    //                        botLeft = Neighbor.topLeft //down left
    //                    };

    //                    GenerateFace(cliff, tCliff);
    //                }
    //            }

    //        }
    //    }
    //}

    private Vector2 UVProjection(Vector3 point)
    {
        int numberOfChunks = ((World)World.Instance).numberOfChunks;
        Vector3 UV_projectionInChunk = Vector3.ProjectOnPlane(point, Vector3.up) / (chunkSize * ((World)World.Instance).tileSize * numberOfChunks);

        float x = UV_projectionInChunk.x;
        float y = UV_projectionInChunk.z;

        return new Vector2(x, y);
    }

    //public void GenerateTerrain()
    //{

    //    chunkTilemap = new FaceData[chunkSize, chunkSize];
    //    heightmap = new int[chunkSize, chunkSize];

    //    int startX = chunkIndex.x * chunkSize, startY = chunkIndex.y * chunkSize;

    //    for (int dx = 0; dx < chunkSize; dx++)
    //        for (int dy = 0; dy < chunkSize; dy++)
    //        {
    //            int u = Mathf.Min(globalHeightmap.GetLength(0) - 1, startX + dx);
    //            int v = Mathf.Min(globalHeightmap.GetLength(1) -1, startY + dy);
    //            //get y and insert to int heightmap for later
    //            int y = Mathf.RoundToInt(globalHeightmap[u, v] * 10);

    //            //Debug.Assert(wd < heightmap.GetLength(0) && hg < heightmap.GetLength(1), "Error. WD/HG: " + wd + " " + hg);
    //            heightmap[dx, dy] = y;


    //            chunkTilemap[dx, dy] = new FaceData
    //            {
    //                topLeft = new Vector3(dx - 0.5f, y, dy + 0.5f), //top left
    //                topRight = new Vector3(dx + 0.5f, y, dy + 0.5f), //up right
    //                botRight = new Vector3(dx + 0.5f, y, dy - 0.5f), //down right
    //                botLeft = new Vector3(dx - 0.5f, y, dy - 0.5f) //down left
    //            };
    //        }
    //}

    public void GenerateMesh(Tile[,] chunkTilemap, Color32[,] colormap, bool tintFlippedTiles)
    {
        for (int wd = 0; wd < chunkTilemap.GetLength(0); wd++)
            for (int hg = 0; hg < chunkTilemap.GetLength(1); hg++)
            {
                Color32 faceColor = colormap[wd, hg];

                //See flipped Tiles in red
                if (tintFlippedTiles)
                    if (chunkTilemap[wd, hg].flippedTriangles)
                    {
                        faceColor = Color.red;
                    }

                GenerateFace(chunkTilemap[wd, hg].face, faceColor, chunkTilemap[wd, hg].flippedTriangles);



                //darker for cliffs
                faceColor.r -= 10;
                faceColor.g -= 10;
                faceColor.b -= 10;

                if (chunkTilemap[wd, hg] is TileCliff)
                {
                    TileCliff cliff = (TileCliff)chunkTilemap[wd, hg];

                    if (cliff.topCliff != null)
                    {
                        GenerateFace(cliff.topCliff, colormap[wd, hg], false);
                    }

                    if (cliff.rightCliff != null)
                    {
                        GenerateFace(cliff.rightCliff, colormap[wd, hg], false);
                    }

                    if (cliff.botCliff != null)
                    {
                        GenerateFace(cliff.botCliff, colormap[wd, hg], false);
                    }

                    if (cliff.leftCliff != null)
                    {
                        GenerateFace(cliff.leftCliff, colormap[wd, hg], false);
                    }

                }
            }
    }


}
