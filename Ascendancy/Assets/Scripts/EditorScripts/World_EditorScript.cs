using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (HeightMapGenerator))]
public class World_EditorScript : Editor
{
    public override void OnInspectorGUI()
    {
        HeightMapGenerator mapGen = (HeightMapGenerator)target;

        DrawDefaultInspector();
    }
}
