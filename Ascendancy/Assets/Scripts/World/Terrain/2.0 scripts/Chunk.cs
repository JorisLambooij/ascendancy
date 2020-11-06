using System.Collections;
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
        mesh.RecalculateTangents();

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

        newVertices.Add(face.topLeft);      //1
        newVertices.Add(face.topRight);     //2
        newVertices.Add(face.botRight);     //3
        newVertices.Add(face.botLeft);      //4


        newColors.Add(c);
        newColors.Add(c);
        newColors.Add(c);
        newColors.Add(c);
        newColors.Add(c);
        newColors.Add(c);

        if (!flipTriangles)
        {
            //need duplicate verts for better corners
            newVertices.Add(face.topLeft);          //1 = 5
            newVertices.Add(face.botRight);         //3 = 6

            newTriangles.Add(faceCount * 6);        //1
            newTriangles.Add(faceCount * 6 + 1);    //2
            newTriangles.Add(faceCount * 6 + 2);    //3
            newTriangles.Add(faceCount * 6 + 4);    //5
            newTriangles.Add(faceCount * 6 + 5);    //6
            newTriangles.Add(faceCount * 6 + 3);    //4

            newUV.Add(UVProjection(face.topLeft));
            newUV.Add(UVProjection(face.topRight));
            newUV.Add(UVProjection(face.botRight));
            newUV.Add(UVProjection(face.botLeft));

            newUV.Add(UVProjection(face.topLeft));
            newUV.Add(UVProjection(face.botRight));
        }
        else
        {
            //need duplicate verts for better corners
            newVertices.Add(face.topRight);         //2 = 5
            newVertices.Add(face.botLeft);          //4 = 6

            newTriangles.Add(faceCount * 6);        //1
            newTriangles.Add(faceCount * 6 + 1);    //2
            newTriangles.Add(faceCount * 6 + 3);    //4
            newTriangles.Add(faceCount * 6 + 4);    //5
            newTriangles.Add(faceCount * 6 + 2);    //3
            newTriangles.Add(faceCount * 6 + 5);    //6

            newUV.Add(UVProjection(face.topLeft));
            newUV.Add(UVProjection(face.topRight));
            newUV.Add(UVProjection(face.botRight));
            newUV.Add(UVProjection(face.botLeft));

            newUV.Add(UVProjection(face.topRight));
            newUV.Add(UVProjection(face.botLeft));
        }



        faceCount++;
    }    

    private Vector2 UVProjection(Vector3 point)
    {
        int numberOfChunks = ((World)World.Instance).numberOfChunks;
        Vector3 UV_projectionInChunk = Vector3.ProjectOnPlane(point, Vector3.up) / (chunkSize * ((World)World.Instance).tileSize * numberOfChunks);

        float x = UV_projectionInChunk.x;
        float y = UV_projectionInChunk.z;

        return new Vector2(x, y);
    }

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

                ////darker for cliffs
                //faceColor.r -= 10;
                //faceColor.g -= 10;
                //faceColor.b -= 10;

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
