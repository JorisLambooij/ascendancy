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

    private List<int> colorCorrectableTiles = new List<int> { 1110, 2221, 1101, 2212, 1011, 2122, 0111, 1222 };

    // Start is called before the first frame update
    public void Initialize(Tile[,] chunkTilemap, Color32[,] colormap, bool tintFlippedTiles, List<int> highlightedTiles)
    {
        newVertices = new List<Vector3>();
        newUV = new List<Vector2>();
        newTriangles = new List<int>();

        //mesh = GetComponent<MeshFilter>().mesh;
        mesh = new Mesh();
        col = GetComponent<MeshCollider>();

        //GenerateTerrain(chunkTilemap);

        //AdditiveSmoothing();

        GenerateMesh(chunkTilemap, colormap, tintFlippedTiles, highlightedTiles);
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

    private void GenerateFace(Face face, Color[] vertexColors, bool flipTriangles)
    {
        newVertices.Add(face.topLeft);      //1
        newVertices.Add(face.topRight);     //2
        newVertices.Add(face.botRight);     //3
        newVertices.Add(face.botLeft);      //4

        for (int i = 0; i < 6; i++)
            newColors.Add(vertexColors[i]);

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

    public void GenerateMesh(Tile[,] chunkTilemap, Color32[,] colormap, bool tintFlippedTiles, List<int> highlightedTiles)
    {
        for (int wd = 0; wd < chunkTilemap.GetLength(0); wd++)
            for (int hg = 0; hg < chunkTilemap.GetLength(1); hg++)
            {
                // colormap is shifted by one due to neighbor info, so correct for that
                Color32 faceColor = colormap[wd + 1, hg + 1];

                //See flipped Tiles in red
                if (tintFlippedTiles)
                    if (chunkTilemap[wd, hg].flippedTriangles)
                    {
                        faceColor = Color.red;
                    }

                if (highlightedTiles != null)
                    if (highlightedTiles.Count > 0)
                        if (highlightedTiles.Contains(chunkTilemap[wd, hg].GetTileType()))
                            faceColor = Color.magenta;

                Color[] vertexColors = { faceColor, faceColor, faceColor, faceColor, faceColor, faceColor };
                
                int tileType = chunkTilemap[wd, hg].GetTileType();
                // if this tile type doesn't need color correction, proceed as normal
                if (!colorCorrectableTiles.Contains(tileType))
                    GenerateFace(chunkTilemap[wd, hg].face, vertexColors, chunkTilemap[wd, hg].flippedTriangles);
                // otherwise, determine the orientation and color in as necessary
                else
                {
                    Color nbColor = Color.red;
                    switch (tileType)
                    {
                        case 0111:
                        case 1222:
                            nbColor = colormap[wd + 2, hg + 1];
                            vertexColors = new Color[] { faceColor, faceColor, nbColor, faceColor, nbColor, nbColor };
                            break;
                        case 1011:
                        case 2122:
                            nbColor = colormap[wd + 1, hg];
                            vertexColors = new Color[] { faceColor, faceColor, faceColor, nbColor, nbColor, nbColor };
                            break;
                        case 1101:
                        case 2212:
                            nbColor = colormap[wd, hg + 1];
                            vertexColors = new Color[] { nbColor, nbColor, faceColor, nbColor, faceColor, faceColor };
                            break;
                        case 1110:
                        case 2221:
                            nbColor = colormap[wd + 1, hg + 2];
                            vertexColors = new Color[] { nbColor, nbColor, nbColor, faceColor, faceColor, faceColor};
                            break;
                        default:
                            vertexColors = new Color[] { Color.white, Color.white, Color.white, Color.white, Color.white, Color.white };
                            break;
                    }

                    GenerateFace(chunkTilemap[wd, hg].face, vertexColors, chunkTilemap[wd, hg].flippedTriangles);
                }
                

                if (chunkTilemap[wd, hg] is TileCliff)
                {
                    TileCliff cliff = (TileCliff)chunkTilemap[wd, hg];

                    if (cliff.topCliff != null)
                        GenerateFace(cliff.topCliff, vertexColors, false);
                    
                    if (cliff.rightCliff != null)
                        GenerateFace(cliff.rightCliff, vertexColors, false);
                    
                    if (cliff.botCliff != null)
                        GenerateFace(cliff.botCliff, vertexColors, false);
                    
                    if (cliff.leftCliff != null)
                        GenerateFace(cliff.leftCliff, vertexColors, false);
                }
            }
    }


}
