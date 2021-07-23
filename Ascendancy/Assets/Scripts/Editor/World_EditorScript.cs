using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR) 
[CustomEditor (typeof (World))]
public class World_EditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        World world = (World)target;

        if (GUILayout.Button("Generate New World - 0 Seed"))
        {
            Generate(world, Vector2.zero);
        }
        if (GUILayout.Button("Generate New World - Random Seed"))
        {
            Vector2 seed = new Vector2(Random.Range(-1000, 1000), Random.Range(-1000, 1000));
            Generate(world, seed);
        }
        if (GUILayout.Button("Re-Generate Texture"))
        {
            ReGenerateTexture(world);
        }
        DrawDefaultInspector();
    }

    private void Generate(World world, Vector2 seed)
    {
        Chunk[] chunks = world.ChunkCollector.GetComponentsInChildren<Chunk>();
        for (int i = 0; i < chunks.Length; i++)
        {
            DestroyImmediate(chunks[i].gameObject);
        }

        HeightMapGenerator hmGen = world.transform.GetComponent<HeightMapGenerator>();
        hmGen.perlinOffset = seed;
        world.Awake();

        //chunks = world.ChunkCollector.GetComponentsInChildren<Chunk>();
        //for (int i = 0; i < chunks.Length; i++)
        //    chunks[i].GetComponent<MeshRenderer>().material.EnableKeyword("TOGGLEFOW_OFF");
    }

    private void ReGenerateTexture(World world)
    {
        //HeightMapGenerator hmGen = world.transform.GetComponent<HeightMapGenerator>();
        world.RegenerateTexture();
    }
}
#endif
